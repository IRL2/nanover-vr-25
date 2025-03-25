using System;
using System.Collections.Generic;
using System.Linq;
using Nanover.Core.Collections;
using Nanover.Core.Science;
using Nanover.Frame.Import.CIF.Components;
using UnityEngine;

namespace Nanover.Frame.Import.CIF.Structures
{
    /// <summary>
    /// Represents a complete molecular system read from a mmCIF file.
    /// </summary>
    internal class CifSystem
    {
        private readonly List<CifBond> bonds = new List<CifBond>();

        private readonly List<CifAtom> atoms = new List<CifAtom>();

        private readonly List<CifResidue> residues = new List<CifResidue>();

        private readonly List<CifAsymmetricUnit> asymmetricUnits = new List<CifAsymmetricUnit>();

        private readonly List<CifEntity> entities = new List<CifEntity>();

        private readonly Dictionary<string, CifAsymmetricUnit> asymmetricUnitsById =
            new Dictionary<string, CifAsymmetricUnit>();

        /// <summary>
        /// Add an atom to the system, and set its absolute index in the system.
        /// </summary>
        private void AddAtom(CifAtom atom)
        {
            atom.AbsoluteIndex = atoms.Count;
            atoms.Add(atom);
        }

        /// <summary>
        /// Add a residue to the system, and set its absolute index in the system.
        /// </summary>
        private void AddResidue(CifResidue residue)
        {
            residue.AbsoluteIndex = residues.Count;
            residues.Add(residue);
        }

        /// <summary>
        /// Add an asymmetric unit to the system, and set its absolute index in the system.
        /// </summary>
        private void AddAsymmetricUnit(CifAsymmetricUnit unit)
        {
            if (asymmetricUnitsById.ContainsKey(unit.AsymId))
                throw new InvalidOperationException(
                    $"Asymmetric unit already exists with id {unit.AsymId}");
            unit.AbsoluteIndex = asymmetricUnits.Count;
            asymmetricUnits.Add(unit);
            asymmetricUnitsById[unit.AsymId] = unit;
        }

        /// <summary>
        /// Add an entity to the system, and set its absolute index in the system.
        /// </summary>
        private void AddEntity(CifEntity entity)
        {
            entity.AbsoluteIndex = entities.Count;
            entities.Add(entity);
        }

        /// <summary>
        /// Convert the imported molecular system to a <see cref="Nanover.Frame" /> for use in
        /// Nanover.
        /// </summary>
        public virtual Frame GetFrame()
        {
            return new Frame
            {
                ParticleElements = atoms.Select(atom => atom.Element).ToArray(),
                ParticlePositions = atoms.Select(atom => atom.Position).ToArray(),
                ParticleNames = atoms.Select(atom => atom.AtomId).ToArray(),
                ParticleResidues = atoms.Select(atom => atom.Residue.AbsoluteIndex).ToArray(),
                ResidueNames = residues.Select(residue => residue.ComponentId).ToArray(),
                ResidueEntities = residues.Select(residue => residue.AsymmetricUnit.AbsoluteIndex).ToArray(),
                BondPairs = bonds.Select(e => new BondPair(e.A.AbsoluteIndex, e.B.AbsoluteIndex))
                                 .ToArray(),
                ParticleCount = atoms.Count,
                ResidueCount = residues.Count,
                EntityCount = entities.Count
            };
        }


        /// <summary>
        /// Add an atom to the system.
        /// </summary>
        public CifAtom AddAtom(CifResidue residue,
                               int id,
                               Element element,
                               string atomName,
                               Vector3 position,
                               string altId)
        {
            var atom = new CifAtom
            {
                Id = id,
                Element = element,
                AtomId = atomName,
                Position = position,
                AltId = altId
            };
            AddAtom(atom);
            residue.AddAtom(atom);
            return atom;
        }

        /// <summary>
        /// Add a residue to the system.
        /// </summary>
        public CifResidue AddResidue(CifAsymmetricUnit unit, int? seqId, string compId)
        {
            var residue = new CifResidue
            {
                AsymmetricUnit = unit,
                SequenceId = seqId,
                ComponentId = compId
            };
            AddResidue(residue);
            unit.AddResidue(residue);
            return residue;
        }

        /// <summary>
        /// Add an asymmetric unit to the system.
        /// </summary>
        public CifAsymmetricUnit AddAsymmetricUnit(CifEntity entity, string asymId)
        {
            var unit = new CifAsymmetricUnit()
            {
                Entity = entity,
                AsymId = asymId
            };
            AddAsymmetricUnit(unit);
            return unit;
        }

        /// <summary>
        /// Add an entity to the system.
        /// </summary>
        public CifEntity AddEntity(int entityId)
        {
            var entity = new CifEntity()
            {
                System = this,
                AbsoluteIndex = entities.Count,
                EntityId = entityId
            };
            AddEntity(entity);
            return entity;
        }

        /// <summary>
        /// Find an entity by its entity id.
        /// </summary>
        public CifEntity FindEntityById(int entityId)
        {
            return entities.FirstOrDefault(e => e.EntityId == entityId);
        }

        /// <summary>
        /// Find a asymmetric unit by its asym id
        /// </summary>
        public CifAsymmetricUnit FindAsymmetricUnitById(string asymId)
        {
            return asymmetricUnitsById.TryGetValue(asymId, out var value) ? value : null;
        }

        /// <summary>
        /// Find an atom by its atom_site identifier.
        /// </summary>
        public CifAtom FindAtomById(string atomId, string compId, int? seqId, string asymId)
        {
            var unit = FindAsymmetricUnitById(asymId);

            var res = unit.FindResidue(compId, seqId);
            if (res?.ComponentId == compId)
            {
                var atom = res?.FindAtomWithAtomId(atomId);
                return atom;
            }

            return null;
        }

        /// <summary>
        /// Get the bond between these two atoms
        /// </summary>
        public CifBond GetBond(CifAtom atom1, CifAtom atom2)
        {
            foreach (var bond in bonds)
            {
                if (bond.A == atom1 && bond.B == atom2)
                    return bond;
                if (bond.B == atom1 && bond.A == atom2)
                    return bond;
            }

            return null;
        }

        public void AddBond(CifAtom atom1, CifAtom atom2, int order = 1)
        {
            bonds.Add(new CifBond
            {
                A = atom1,
                B = atom2,
                Order = order
            });
        }


        /// <summary>
        /// Generate internal residue bonds using monomers provided by an
        /// <see cref="IChemicalComponentDictionary" />.
        /// </summary>
        internal void GenerateIntraResidueBonds(ChemicalComponentDictionary provider)
        {
            foreach (var entity in asymmetricUnits)
            foreach (var residue in entity.Residues)
            {
                var id = residue.ComponentId;
                var definition = provider.GetResidue(id);
                if (definition != null)
                    foreach (var bond in definition.Bonds)
                    {
                        var atom1 = residue.FindAtomWithAtomId(bond.a);
                        var atom2 = residue.FindAtomWithAtomId(bond.b);
                        if (atom1 != null && atom2 != null)
                            AddBond(atom1, atom2);
                    }
            }
        }

        /// <summary>
        /// Generate a DNA backbone.
        /// </summary>
        internal void GenerateDnaBackbone()
        {
            foreach (var entity in asymmetricUnits)
            foreach (var (first, second) in entity.Residues.GetPairs())
            {
                var phosphorus = second.FindAtomWithAtomId("P");
                var oxygen = first.FindAtomWithAtomId("O3'");
                if (phosphorus != null && oxygen != null)
                    AddBond(phosphorus, oxygen);
            }
        }

        /// <summary>
        /// Generate a peptide backbone.
        /// </summary>
        internal void GeneratePeptideBackbone()
        {
            foreach (var entity in asymmetricUnits)
            foreach (var (first, second) in entity.Residues.GetPairs())
            {
                if (!AminoAcid.IsStandardAminoAcid(first.ComponentId))
                    continue;
                if (!AminoAcid.IsStandardAminoAcid(second.ComponentId))
                    continue;
                if (first.AsymmetricUnit != second.AsymmetricUnit)
                    continue;
                var carbon = first.FindAtomWithAtomId("C");
                var nitrogen = second.FindAtomWithAtomId("N");
                if (Vector3.Distance(carbon.Position, nitrogen.Position) < 0.2f)
                    AddBond(carbon, nitrogen);
            }
        }
    }
}