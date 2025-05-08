using System;
using System.Collections.Generic;
using System.IO;
using Nanover.Frame.Import.CIF.Components;
using Nanover.Frame.Import.CIF.Structures;
using UnityEngine;

namespace Nanover.Frame.Import.CIF
{
    /// <summary>
    /// Importer for the mmCIF file format, used by the RCSB database.
    /// </summary>
    /// <remarks>
    /// Information on this file format can be found at
    /// http://mmcif.wwpdb.org/pdbx-mmcif-home-page.html
    /// </remarks>
    public class CifImport : CifBaseImport
    {
        /// <summary>
        /// Import a CIF file from the given source, with an optional chemical component
        /// dictionary
        /// </summary>
        public static Frame Import(TextReader reader, ChemicalComponentDictionary dictionary, IProgress<string> progress = null)
        {
            return ImportSystem(reader, dictionary, progress).GetFrame();
        }

        /// <summary>
        /// Import a mmCIF file into the internal <see cref="CifSystem" /> object.
        /// </summary>
        internal static CifSystem ImportSystem(TextReader reader, ChemicalComponentDictionary dictionary, IProgress<string> progress = null)
        {
            var parser = new CifImport(progress);
            parser.Parse(reader);

            parser.ResolveStructuralConnections();

            if (dictionary != null)
                parser.system.GenerateIntraResidueBonds(dictionary);

            parser.system.GeneratePeptideBackbone();

            parser.system.GenerateDnaBackbone();

            return parser.system;
        }

        private readonly CifSystem system = new CifSystem();

        protected override ParseTableRow GetTableHandler(string category, List<string> keywords)
        {
            if (category == "atom_site")
                return GetAtomHandler(keywords);
            if (category == "struct_asym")
                return GetStructAsymHandler(keywords);
            if (category == "entity")
                return GetEntityHandler(keywords);
            if (category == "struct_conn")
                return GetStructuralConnectionHandler(keywords);
            return null;
        }

        protected override bool ShouldParseCategory(string category)
        {
            return category == "atom_site"
                || category == "struct_asym"
                || category == "entity"
                || category == "struct_conn";
        }

        /// <summary>
        /// Handle the struct_conn table, which details disulphide bridges and covalent
        /// bonds
        /// </summary>
        /// <remarks>
        /// As this appears before the atom_site table, this must be cached until after
        /// parsing,
        /// when <see cref="ResolveStructuralConnections" /> is called.
        /// </remarks>
        private ParseTableRow GetStructuralConnectionHandler(IList<string> keywords)
        {
            var indexTypeId = keywords.IndexOf("conn_type_id");

            var indexAsym1 = keywords.IndexOf("ptnr1_label_asym_id");
            var indexId1 = keywords.IndexOf("ptnr1_label_atom_id");
            var indexComp1 = keywords.IndexOf("ptnr1_label_comp_id");
            var indexSeq1 = keywords.IndexOf("ptnr1_label_seq_id");

            var indexAsym2 = keywords.IndexOf("ptnr2_label_asym_id");
            var indexId2 = keywords.IndexOf("ptnr2_label_atom_id");
            var indexComp2 = keywords.IndexOf("ptnr2_label_comp_id");
            var indexSeq2 = keywords.IndexOf("ptnr2_label_seq_id");

            return data =>
            {
                var index = ParseString(data[indexTypeId]);

                if (index != "covale" && index != "disulf")
                    return;

                var asymA = ParseString(data[indexAsym1]);
                var atomIdA = ParseString(data[indexId1]);
                var compA = ParseString(data[indexComp1]);
                var seqA = ParseInt(data[indexSeq1], $"Failed to parse atom Id {data[indexId1]}");

                var asymB = ParseString(data[indexAsym2]);
                var atomIdB = ParseString(data[indexId2]);
                var compB = ParseString(data[indexComp2]);
                var seqB = ParseInt(data[indexSeq2], $"Failed to parse atom Id {data[indexId1]}");

                structuralConnections.Add(new StructuralConnection
                {
                    AsymIdA = asymA,
                    AtomIdA = atomIdA,
                    CompA = compA,
                    SeqA = seqA,
                    AsymIdB = asymB,
                    AtomIdB = atomIdB,
                    CompB = compB,
                    SeqB = seqB
                });
            };
        }

        /// <summary>
        /// Definition of a connection between two atoms.
        /// </summary>
        private struct StructuralConnection
        {
            public string AsymIdA;
            public string AtomIdA;
            public string CompA;
            public int? SeqA;
            public string AsymIdB;
            public string AtomIdB;
            public string CompB;
            public int? SeqB;
        }

        private readonly List<StructuralConnection> structuralConnections =
            new List<StructuralConnection>();

        private CifImport(IProgress<string> progress = null) : base(progress)
        {
        }

        /// <summary>
        /// Resolve all of the <see cref="StructuralConnection" /> records into bonds.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        private void ResolveStructuralConnections()
        {
            foreach (var connection in structuralConnections)
            {
                var atom1 = system.FindAtomById(connection.AtomIdA,
                                                connection.CompA,
                                                connection.SeqA,
                                                connection.AsymIdA);
                var atom2 = system.FindAtomById(connection.AtomIdB,
                                                connection.CompB,
                                                connection.SeqB,
                                                connection.AsymIdB);

                if (atom1 == null || atom2 == null)
                    throw new ArgumentException("Cannot form bond between missing atoms!");

                system.AddBond(atom1, atom2);
            }
        }

        /// <summary>
        /// Parse the struct_asym records, which detail the asymmetric units.
        /// </summary>
        private ParseTableRow GetStructAsymHandler(IList<string> keywords)
        {
            var indexId = keywords.IndexOf("id");
            var indexEntityId = keywords.IndexOf("entity_id");

            return data =>
            {
                var entity_id = ParseNonNullInt(data[indexEntityId],
                                                $"Failed to parse entity id {data[indexId]}");
                var id = ParseString(data[indexId]);
                var entity = system.FindEntityById(entity_id);
                if (entity == null)
                    throw new InvalidOperationException($"Cannot find entity with id {entity_id}");

                system.AddAsymmetricUnit(entity, id);
            };
        }

        /// <summary>
        /// Parse the entity records, which list the entities in the system.
        /// </summary>
        private ParseTableRow GetEntityHandler(IList<string> keywords)
        {
            var indexId = keywords.IndexOf("id");

            return data =>
            {
                var entityId = ParseNonNullInt(data[indexId],
                                               $"Failed to parse entity id {data[indexId]}");
                system.AddEntity(entityId);
            };
        }


        /// <summary>
        /// Parse the atom_site records, which list the atoms of the system.
        /// </summary>
        private ParseTableRow GetAtomHandler(IList<string> keywords)
        {
            var indexId = keywords.IndexOf("id");
            var indexElement = keywords.IndexOf("type_symbol");
            var indexAtomId = keywords.IndexOf("label_atom_id");
            var indexResidueId = keywords.IndexOf("label_comp_id");
            var indexX = keywords.IndexOf("Cartn_x");
            var indexY = keywords.IndexOf("Cartn_y");
            var indexZ = keywords.IndexOf("Cartn_z");
            var indexEntityId = keywords.IndexOf("label_entity_id");
            var indexSeqId = keywords.IndexOf("label_seq_id");
            var indexAsymId = keywords.IndexOf("label_asym_id");
            var indexAltId = keywords.IndexOf("label_alt_id");
            var indexModelNum = keywords.IndexOf("pdbx_PDB_model_num");

            return data =>
            {
                // Ignore atoms with PDB model numbers greater than 1.
                if (indexModelNum > 0)
                {
                    var modelNum = ParseInt(data[indexModelNum], "Failed to parse model number");
                    if (modelNum.HasValue && modelNum > 1)
                        return;
                }

                var id = ParseNonNullInt(data[indexId],
                                         "Failed to parse atom index");

                var element = ParseElementSymbol(data[indexElement]);
                if (!element.HasValue)
                    throw new ArgumentException("Failed to parse element.");

                var atomId = data[indexAtomId].Trim();
                var residueName = data[indexResidueId].Trim();

                var entityId = ParseNonNullInt(data[indexEntityId], "Failed to parse entity ID");

                var seqId = data[indexSeqId] == "."
                                ? null
                                : ParseInt(data[indexSeqId], "Failed to parse seq id");

                var x = ParseFloat(data[indexX],
                                   "Failed to parse x coordinate");
                var y = ParseFloat(data[indexY],
                                   "Failed to parse y coordinate");
                var z = ParseFloat(data[indexZ],
                                   "Failed to parse z coordinate");


                var asymId = ParseString(data[indexAsymId]);


                var altId = ParseString(data[indexAltId]);


                if (!x.HasValue || !y.HasValue || !z.HasValue)
                    throw new ArgumentException("Missing Coordinate");


                var entity = system.FindEntityById(entityId);
                if (entity == null)
                    throw new InvalidOperationException($"Cannot find entity with id {entityId}");

                var unit = asymId != null ? system.FindAsymmetricUnitById(asymId) : null;
                if (unit == null)
                    throw new InvalidOperationException(
                        $"Cannot find asymmetric unit with id {asymId}");


                var residue = unit.FindResidue(residueName, seqId);
                if (residue == null)
                {
                    residue = system.AddResidue(unit, seqId, residueName);
                }
                else
                {
                    var existing = residue.FindAtomWithAtomId(atomId);
                    if (existing != null)
                    {
                        if (existing.AltId != null && altId != null && existing.AltId != altId)
                        {
                            return;
                        }
                        else
                        {
                            // Duplicate atom name - must be multiple residues
                            residue = system.AddResidue(unit, seqId, residueName);
                        }
                    }
                }


                system.AddAtom(residue, id, element.Value, atomId,
                               0.1f * new Vector3(x.Value, y.Value, z.Value), altId);
            };
        }
    }
}