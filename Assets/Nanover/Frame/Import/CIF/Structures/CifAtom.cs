using Nanover.Core.Science;
using UnityEngine;

namespace Nanover.Frame.Import.CIF.Structures
{
    /// <summary>
    /// An atom read from the atom_site table of a mmCIF file.
    /// </summary>
    internal class CifAtom
    {
        /// <summary>
        /// Absolute index of the atom in the <see cref="CifSystem" />.
        /// </summary>
        public int AbsoluteIndex { get; set; }

        /// <summary>
        /// ID used to identify the atom_site of this atom.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// ID defining the unique name of this atom within the residue.
        /// </summary>
        public string AtomId { get; set; }

        /// <summary>
        /// An alternate location ID, used to mark multiple possible positions.
        /// </summary>
        public string AltId { get; set; }

        /// <summary>
        /// Position of the atom in nanometers.
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// The residue this atom belongs to.
        /// </summary>
        public CifResidue Residue { get; set; }

        /// <summary>
        /// The atomic element of this atom.
        /// </summary>
        public Element Element { get; set; }
    }
}