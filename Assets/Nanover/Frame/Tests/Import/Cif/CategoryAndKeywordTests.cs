using Nanover.Frame.Import.CIF;
using NUnit.Framework;
using Assert = UnityEngine.Assertions.Assert;

namespace Nanover.Trajectory.Import.Tests.Cif
{
    public class CategoryAndKeywordTests
    {
        [Test]
        public void NoValue()
        {
            Assert.AreEqual(("abc", "def", null),
                            CifUtility.GetCategoryAndKeyword("_abc.def"));
        }

        [Test]
        public void Value()
        {
            Assert.AreEqual(("abc", "def", "value"),
                            CifUtility.GetCategoryAndKeyword("_abc.def value"));
        }

        [Test]
        public void Value_MultipleSpaces()
        {
            Assert.AreEqual(("abc", "def", "value"),
                            CifUtility.GetCategoryAndKeyword("_abc.def   value"));
        }

        [Test]
        public void Value_Tab()
        {
            Assert.AreEqual(("abc", "def", "value"),
                            CifUtility.GetCategoryAndKeyword("_abc.def\tvalue"));
        }
    }
}