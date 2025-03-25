using System.Collections.Generic;
using Nanover.Core.Science;
using NanoverImd.Selection;
using NUnit.Framework;
using UnityEngine;

namespace NanoverImd.Tests
{
    internal class TryParseElementTests
    {
        private static IEnumerable<(int value, Element element)> AtomicNumberParameters()
        {
            yield return (1, Element.Hydrogen);
            yield return (8, Element.Oxygen);
            yield return (26, Element.Iron);
        }
        
        private static IEnumerable<int> InvalidAtomicNumberParameters()
        {
            yield return -1;
            yield return 242;
            yield return int.MaxValue;
            yield return int.MinValue;
        }

        [Test]
        public void TestAtomicNumber(
            [ValueSource(nameof(AtomicNumberParameters))] (int value, Element element) parameter)
        {
            Assert.IsTrue(VisualiserFactory.TryParseElement(parameter.value,
                                                          out var element));
            Assert.AreEqual(parameter.element, element);
        }
        
        [Test]
        public void TestAtomicNumberInvalid(
            [ValueSource(nameof(InvalidAtomicNumberParameters))] int value)
        {
            Assert.IsFalse(VisualiserFactory.TryParseElement(value,
                                                            out var element));
        }
        
        private static IEnumerable<(string value, Element element)> AtomicSymbolParameters()
        {
            yield return ("h", Element.Hydrogen);
            yield return ("o", Element.Oxygen);
            yield return ("fe", Element.Iron);
            
            yield return ("H", Element.Hydrogen);
            yield return ("O", Element.Oxygen);
            yield return ("Fe", Element.Iron);
            yield return ("FE", Element.Iron);

            yield return (" H", Element.Hydrogen);
            yield return ("O ", Element.Oxygen);
            yield return (" Fe ", Element.Iron);
        }
        
        private static IEnumerable<string> InvalidAtomicSymbolParameters()
        {
            yield return null;
            yield return string.Empty;
            yield return "X";
            yield return "\u4592\u1829\u0241";
        }

        [Test]
        public void TestAtomicSymbol(
            [ValueSource(nameof(AtomicSymbolParameters))] (string value, Element element) parameter)
        {
            Assert.IsTrue(VisualiserFactory.TryParseElement(parameter.value,
                                                            out var element));
            Assert.AreEqual(parameter.element, element);
        }
        
        [Test]
        public void TestAtomicNumberInvalid(
            [ValueSource(nameof(InvalidAtomicSymbolParameters))] string value)
        {
            Assert.IsFalse(VisualiserFactory.TryParseElement(value,
                                                             out var element));
        }
    }
}