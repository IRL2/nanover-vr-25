using System.Collections.Generic;
using NUnit.Framework;

namespace Nanover.Trajectory.Import.Tests.Cif
{
    public class SingleLineTableTests
    {
        internal IReadOnlyList<CifTestImport.DataTable> Import(string str)
        {
            return CifTestImport.Import(str);
        }

        [Test]
        public void SingleLine()
        {
            var data = Import("_abc.def value");

            Assert.AreEqual(1, data.Count);
            Assert.AreEqual("abc", data[0].Category);
            CollectionAssert.AreEqual(new[] { "def" }, data[0].Keywords);
            Assert.AreEqual(1, data[0].Values.Count);
            CollectionAssert.AreEqual(new[] { "value" }, data[0].Values[0]);
        }

        [Test]
        public void SingleLine_TrailingSpaces()
        {
            var data = Import("_abc.def value   ");

            Assert.AreEqual(1, data.Count);
            Assert.AreEqual("abc", data[0].Category);
            CollectionAssert.AreEqual(new[] { "def" }, data[0].Keywords);
            Assert.AreEqual(1, data[0].Values.Count);
            CollectionAssert.AreEqual(new[] { "value" }, data[0].Values[0]);
        }

        [Test]
        public void SingleLine_DoubleQuotes()
        {
            var data = Import("_abc.def \"value\"");

            Assert.AreEqual(1, data.Count);
            Assert.AreEqual("abc", data[0].Category);
            CollectionAssert.AreEqual(new[] { "def" }, data[0].Keywords);
            Assert.AreEqual(1, data[0].Values.Count);
            CollectionAssert.AreEqual(new[] { "value" }, data[0].Values[0]);
        }

        [Test]
        public void SingleLine_SingleQuotes()
        {
            var data = Import("_abc.def 'value'");

            Assert.AreEqual(1, data.Count);
            Assert.AreEqual("abc", data[0].Category);
            CollectionAssert.AreEqual(new[] { "def" }, data[0].Keywords);
            Assert.AreEqual(1, data[0].Values.Count);
            CollectionAssert.AreEqual(new[] { "value" }, data[0].Values[0]);
        }

        [Test]
        public void SingleLine_DoubleQuotes_NewLine()
        {
            var data = Import("_abc.def \n\"value\"");

            Assert.AreEqual(1, data.Count);
            Assert.AreEqual("abc", data[0].Category);
            CollectionAssert.AreEqual(new[] { "def" }, data[0].Keywords);
            Assert.AreEqual(1, data[0].Values.Count);
            CollectionAssert.AreEqual(new[] { "value" }, data[0].Values[0]);
        }

        [Test]
        public void SingleLine_SingleQuotes_NewLine()
        {
            var data = Import("_abc.def \n'value'");

            Assert.AreEqual(1, data.Count);
            Assert.AreEqual("abc", data[0].Category);
            CollectionAssert.AreEqual(new[] { "def" }, data[0].Keywords);
            Assert.AreEqual(1, data[0].Values.Count);
            CollectionAssert.AreEqual(new[] { "value" }, data[0].Values[0]);
        }

        [Test]
        public void SingleLine_TextField()
        {
            var data = Import("_abc.def \n;value\n; ");

            Assert.AreEqual(1, data.Count);
            Assert.AreEqual("abc", data[0].Category);
            CollectionAssert.AreEqual(new[] { "def" }, data[0].Keywords);
            Assert.AreEqual(1, data[0].Values.Count);
            CollectionAssert.AreEqual(new[] { "value" }, data[0].Values[0]);
        }

        [Test]
        public void SingleLine_TextFieldPrecedingSpace()
        {
            var data = Import("_abc.def \n; value\n; ");

            Assert.AreEqual(1, data.Count);
            Assert.AreEqual("abc", data[0].Category);
            CollectionAssert.AreEqual(new[] { "def" }, data[0].Keywords);
            Assert.AreEqual(1, data[0].Values.Count);
            CollectionAssert.AreEqual(new[] { " value" }, data[0].Values[0]);
        }

        [Test]
        public void SingleLine_TextFieldMultiline()
        {
            var data = Import("_abc.def \n;value\n;value2\n; ");

            Assert.AreEqual(1, data.Count);
            Assert.AreEqual("abc", data[0].Category);
            CollectionAssert.AreEqual(new[] { "def" }, data[0].Keywords);
            Assert.AreEqual(1, data[0].Values.Count);
            CollectionAssert.AreEqual(new[] { "value\nvalue2" }, data[0].Values[0]);
        }

        [Test]
        public void TwoLines()
        {
            var data = Import("_abc.def value1 \n_abc.ghi value2");

            Assert.AreEqual(1, data.Count);
            Assert.AreEqual("abc", data[0].Category);
            CollectionAssert.AreEqual(new[] { "def", "ghi" }, data[0].Keywords);
            Assert.AreEqual(1, data[0].Values.Count);
            CollectionAssert.AreEqual(new[] { "value1", "value2" }, data[0].Values[0]);
        }

        [Test]
        public void TwoLines_DifferentCategories()
        {
            var data = Import(@"_abc.def value1
_ghi.jkl value2");

            Assert.AreEqual(2, data.Count);
            Assert.AreEqual("abc", data[0].Category);
            CollectionAssert.AreEqual(new[] { "def" }, data[0].Keywords);
            Assert.AreEqual(1, data[0].Values.Count);
            CollectionAssert.AreEqual(new[] { "value1" }, data[0].Values[0]);
            Assert.AreEqual("ghi", data[1].Category);
            CollectionAssert.AreEqual(new[] { "jkl" }, data[1].Keywords);
            Assert.AreEqual(1, data[1].Values.Count);
            CollectionAssert.AreEqual(new[] { "value2" }, data[1].Values[0]);
        }
    }
}