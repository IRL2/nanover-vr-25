using System.Collections.Generic;

namespace Nanover.Frame.Import.CIF.Structures
{
    /// <summary>
    /// A residue inferred from an mmCIF file by a component ID and a sequence ID.
    /// </summary>
    internal class CifResidue
    {
        /// <summary>
        /// The absolute index of the residue in the <see cref="CifSystem" />
        /// </summary>
        public int AbsoluteIndex { get; set; }

        private readonly List<CifAtom> atoms = new List<CifAtom>();

        private readonly Dictionary<string, CifAtom> atomsByAtomId =
            new Dictionary<string, CifAtom>();

        /// <summary>
        /// The atoms that are in this residue.
        /// </summary>
        public IReadOnlyCollection<CifAtom> Atoms => atoms;

        /// <summary>
        /// Add an atom to this residue, assigning its residue to this.
        /// </summary>
        public void AddAtom(CifAtom atom)
        {
            atom.Residue = this;
            atoms.Add(atom);
            atomsByAtomId.Add(atom.AtomId, atom);
        }

        /// <summary>
        /// The component ID read from comp_id.
        /// </summary>
        public string ComponentId { get; set; }

        /// <summary>
        /// Find the atom with the given atom_id
        /// </summary>
        public CifAtom FindAtomWithAtomId(string atomId)
        {
            return atomsByAtomId.TryGetValue(atomId, out var value) ? value : null;
        }

        /// <summary>
        /// The sequence ID read from seq_id.
        /// </summary>
        public int? SequenceId { get; set; }

        /// <summary>
        /// The asymmetric unit this residue belongs to.
        /// </summary>
        public CifAsymmetricUnit AsymmetricUnit { get; set; }
    }
}