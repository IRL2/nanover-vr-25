using System.Collections.Generic;
using NUnit.Framework;

namespace Nanover.Trajectory.Import.Tests.Cif
{
    public class MultiLineTableTests
    {
        internal IReadOnlyList<CifTestImport.DataTable> Import(string str)
        {
            return CifTestImport.Import(str);
        }

        [Test]
        public void SingleLine()
        {
            var data = Import(@"loop_
_abc.def 
_abc.ghi
_abc.jkl
value1 value2 value3");

            Assert.AreEqual(1, data.Count);
            Assert.AreEqual("abc", data[0].Category);
            CollectionAssert.AreEqual(new[] { "def", "ghi", "jkl" }, data[0].Keywords);
            Assert.AreEqual(1, data[0].Values.Count);
            CollectionAssert.AreEqual(new[] { "value1", "value2", "value3" }, data[0].Values[0]);
        }

        [Test]
        public void MultipleLines()
        {
            var data = Import(@"loop_
_abc.def 
_abc.ghi
_abc.jkl
value1 value2 value3
value4 value5 value6");

            Assert.AreEqual(1, data.Count);
            Assert.AreEqual("abc", data[0].Category);
            CollectionAssert.AreEqual(new[] { "def", "ghi", "jkl" }, data[0].Keywords);
            Assert.AreEqual(2, data[0].Values.Count);
            CollectionAssert.AreEqual(new[] { "value1", "value2", "value3" }, data[0].Values[0]);
            CollectionAssert.AreEqual(new[] { "value4", "value5", "value6" }, data[0].Values[1]);
        }

        [Test]
        public void Delimited_DoubleQuotes()
        {
            var data = Import(@"loop_
_abc.def 
_abc.ghi
_abc.jkl
value1 "" value2 value2 "" value3");

            Assert.AreEqual(1, data.Count);
            Assert.AreEqual("abc", data[0].Category);
            CollectionAssert.AreEqual(new[] { "def", "ghi", "jkl" }, data[0].Keywords);
            Assert.AreEqual(1, data[0].Values.Count);
            CollectionAssert.AreEqual(new[] { "value1", " value2 value2 ", "value3" },
                                      data[0].Values[0]);
        }

        [Test]
        public void Delimited_SingleQuotes()
        {
            var data = Import(@"loop_
_abc.def 
_abc.ghi
_abc.jkl
value1 ' value2 value2 ' value3");

            Assert.AreEqual(1, data.Count);
            Assert.AreEqual("abc", data[0].Category);
            CollectionAssert.AreEqual(new[] { "def", "ghi", "jkl" }, data[0].Keywords);
            Assert.AreEqual(1, data[0].Values.Count);
            CollectionAssert.AreEqual(new[] { "value1", " value2 value2 ", "value3" },
                                      data[0].Values[0]);
        }
        
        [Test]
        public void SplitOverMultipleLines()
        {
            var data = Import(@"loop_
_abc.def 
_abc.ghi
_abc.jkl
value1
value2
value3");

            Assert.AreEqual(1, data[0].Values.Count);
            CollectionAssert.AreEqual(new[] { "value1", "value2", "value3" },
                                      data[0].Values[0]);
        }
          
        [Test]
        public void TextBlock()
        {
            var data = Import(@"loop_
_abc.def 
_abc.ghi
_abc.jkl
value1 value2 value3
value1
;value 2
and more
;
value3");

            Assert.AreEqual(2, data[0].Values.Count);
            CollectionAssert.AreEqual(new[] { "value1", "value2", "value3" },
                                      data[0].Values[0]);
            CollectionAssert.AreEqual(new[] { "value1", "value 2\nand more", "value3" },
                                      data[0].Values[1]);
        }

    }
}