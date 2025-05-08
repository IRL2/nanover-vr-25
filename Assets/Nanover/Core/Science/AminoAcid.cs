using System;
using System.Linq;
using JetBrains.Annotations;

namespace Nanover.Core.Science
{
    /// <summary>
    /// Definition of an amino acid.
    /// </summary>
    public sealed class AminoAcid
    {
        public static readonly AminoAcid Alanine = new AminoAcid("Alanine", "Ala", 'A');
        public static readonly AminoAcid Arginine = new AminoAcid("Arginine", "Arg", 'R');
        public static readonly AminoAcid Asparagine = new AminoAcid("Asparagine", "Asn", 'N');
        public static readonly AminoAcid AsparticAcid = new AminoAcid("Aspartic acid", "Asp", 'D');
        public static readonly AminoAcid Cysteine = new AminoAcid("Cysteine", "Cys", 'C');
        public static readonly AminoAcid Glutamine = new AminoAcid("Glutamine", "Gln", 'Q');
        public static readonly AminoAcid GlutamicAcid = new AminoAcid("Glutamic acid", "Glu", 'E');
        public static readonly AminoAcid Glycine = new AminoAcid("Glycine", "Gly", 'G');
        public static readonly AminoAcid Histidine = new AminoAcid("Histidine", "His", 'H');
        public static readonly AminoAcid Isoleucine = new AminoAcid("Isoleucine", "Ile", 'I');
        public static readonly AminoAcid Leucine = new AminoAcid("Leucine", "Leu", 'L');
        public static readonly AminoAcid Lysine = new AminoAcid("Lysine", "Lys", 'K');
        public static readonly AminoAcid Methionine = new AminoAcid("Methionine", "Met", 'M');
        public static readonly AminoAcid Phenylalanine = new AminoAcid("Phenylalanine", "Phe", 'F');
        public static readonly AminoAcid Proline = new AminoAcid("Proline", "Pro", 'P');
        public static readonly AminoAcid Serine = new AminoAcid("Serine", "Ser", 'S');
        public static readonly AminoAcid Threonine = new AminoAcid("Threonine", "Thr", 'T');
        public static readonly AminoAcid Tryptophan = new AminoAcid("Tryptophan", "Trp", 'W');
        public static readonly AminoAcid Tyrosine = new AminoAcid("Tyrosine", "Tyr", 'Y');
        public static readonly AminoAcid Valine = new AminoAcid("Valine", "Val", 'V');

        /// <summary>
        /// The 20 standard amino acids.
        /// </summary>
        public static readonly AminoAcid[] StandardAminoAcids =
        {
            Alanine,
            Arginine,
            Asparagine,
            AsparticAcid,
            Cysteine,
            Glutamine,
            GlutamicAcid,
            Glycine,
            Histidine,
            Isoleucine,
            Leucine,
            Lysine,
            Methionine,
            Phenylalanine,
            Proline,
            Serine,
            Threonine,
            Tryptophan,
            Tyrosine,
            Valine
        };

        /// <summary>
        /// Common name of the amino acid.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Three letter code of the amino acid.
        /// </summary>
        public string ThreeLetterCode { get; }

        /// <summary>
        /// Single letter code of the amino acid.
        /// </summary>
        public char SingleLetterCode { get; }

        private AminoAcid(string name, string threeLetterCode, char singleLetterCode)
        {
            Name = name;
            ThreeLetterCode = threeLetterCode;
            SingleLetterCode = singleLetterCode;
        }

        /// <summary>
        /// Is the provided residue name recognized as a standard amino acid?
        /// </summary>
        public static bool IsStandardAminoAcid(string residueName)
        {
            return GetAminoAcidFromResidue(residueName) != null;
        }

        /// <summary>
        /// Get the <see cref="AminoAcid" /> for the provided residue name, returning null
        /// if it is not a valid amino acid.
        /// </summary>
        [CanBeNull]
        public static AminoAcid GetAminoAcidFromResidue(string residueName)
        {
            return StandardAminoAcids.FirstOrDefault(
                aa => aa.ThreeLetterCode.Equals(residueName,
                                                StringComparison.InvariantCultureIgnoreCase));
        }
    }
}