using Nanover.Core.Science;
using NUnit.Framework;
using Assert = UnityEngine.Assertions.Assert;

namespace Nanover.Core.Tests.Science
{
    internal class AminoAcidTests
    {
        [Test]
        public void Name()
        {
            Assert.AreEqual("Alanine", AminoAcid.Alanine.Name);
        }

        [Test]
        public void ThreeLetterCode()
        {
            StringAssert.AreEqualIgnoringCase("Ala", AminoAcid.Alanine.ThreeLetterCode);
        }

        [Test]
        public void SingleLetterCode()
        {
            Assert.AreEqual('A', AminoAcid.Alanine.SingleLetterCode);
        }

        [Test]
        public void AsAminoAcid_Valid()
        {
            Assert.IsNotNull(AminoAcid.GetAminoAcidFromResidue("Ala"));
        }

        [Test]
        public void AsAminoAcid_WrongCase()
        {
            Assert.IsNotNull(AminoAcid.GetAminoAcidFromResidue("aLa"));
        }

        [Test]
        public void AsAminoAcid_Invalid()
        {
            Assert.IsNull(AminoAcid.GetAminoAcidFromResidue("xyz"));
        }

        [Test]
        public void IsAminoAcid_Valid()
        {
            Assert.IsTrue(AminoAcid.IsStandardAminoAcid("Ala"));
        }

        [Test]
        public void IsAminoAcid_WrongCase()
        {
            Assert.IsTrue(AminoAcid.IsStandardAminoAcid("aLa"));
        }

        [Test]
        public void IsAminoAcid_Invalid()
        {
            Assert.IsNull(AminoAcid.GetAminoAcidFromResidue("xyz"));
        }
    }
}