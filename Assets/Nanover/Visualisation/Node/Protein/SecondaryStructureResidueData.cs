using UnityEngine;

namespace Nanover.Visualisation.Node.Protein
{
    /// <summary>
    /// Utility class for storing information about protein residues for use with DSSP.
    /// </summary>
    public class SecondaryStructureResidueData
    {
        public bool HasEssentialNamedAtoms => AlphaCarbonIndex >= 0
                                           && CarbonIndex >= 0
                                           && NitrogenIndex >= 0
                                           && OxygenIndex >= 0;

        /// <summary>
        /// Index of the alpha carbon.
        /// </summary>
        public int AlphaCarbonIndex { get; set; }

        /// <summary>
        /// Position of the alpha carbon.
        /// </summary>
        public Vector3 AlphaCarbonPosition { get; set; }

        /// <summary>
        /// Index of the carbonyl carbon.
        /// </summary>
        public int CarbonIndex { get; set; }

        /// <summary>
        /// Position of the carbonyl carbon.
        /// </summary>
        public Vector3 CarbonPosition { get; set; }

        /// <summary>
        /// Index of the amine hydrogen.
        /// </summary>
        public int HydrogenIndex { get; set; }

        /// <summary>
        /// Position of the amine hydrogen.
        /// </summary>
        public Vector3 HydrogenPosition { get; set; }

        /// <summary>
        /// Index of the amine nitrogen.
        /// </summary>
        public int NitrogenIndex { get; set; }

        /// <summary>
        /// Position of the amine nitrogen.
        /// </summary>
        public Vector3 NitrogenPosition { get; set; }

        /// <summary>
        /// Index of the carbonyl oxygen.
        /// </summary>
        public int OxygenIndex { get; set; }

        /// <summary>
        /// Position of the carbonyl oxygen.
        /// </summary>
        public Vector3 OxygenPosition { get; set; }

        /// <summary>
        /// Secondary structure patterns that this residue exhibits.
        /// </summary>
        public SecondaryStructurePattern Pattern = SecondaryStructurePattern.None;

        /// <summary>
        /// Assignment for this residue's secondary structure.
        /// </summary>
        public SecondaryStructureAssignment SecondaryStructure { get; set; }

        /// <summary>
        /// The lowest energy hydrogen bond formed by this residue's carbonyl.
        /// </summary>
        public double AcceptorHydrogenBondEnergy { get; set; } = 1e10;

        /// <summary>
        /// The lowest energy hydrogen bond formed by this residue's amine.
        /// </summary>
        public double DonorHydrogenBondEnergy { get; set; } = 1e10;

        /// <summary>
        /// The residue to which this carbonyl is bonded to.
        /// </summary>
        public SecondaryStructureResidueData AcceptorHydrogenBondResidue { get; set; }

        /// <summary>
        /// The residue to which this amine is bonded to.
        /// </summary>
        public SecondaryStructureResidueData DonorHydrogenBondResidue { get; set; }

        /// <summary>
        /// The ordinal of this residue in the sequence.
        /// </summary>
        public int ordinal { get; set; }

        /// <summary>
        /// The index of this residue in the system.
        /// </summary>
        public int ResidueIndex { get; set; }
    }
}