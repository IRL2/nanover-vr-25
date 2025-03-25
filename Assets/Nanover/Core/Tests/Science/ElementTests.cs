using Nanover.Core.Science;
using NUnit.Framework;

namespace Nanover.Core.Tests.Science
{
    internal class ElementTests
    {
        [Test]
        public void IntToElement()
        {
            Assert.AreEqual(Element.Carbon, (Element) 6);
        }

        [Test]
        public void ElementToInt()
        {
            Assert.AreEqual(8, (int) Element.Oxygen);
        }

        [Test]
        public void Element_Symbol()
        {
            Assert.AreEqual("C", Element.Carbon.GetSymbol());
        }

        [Test]
        public void Element_Vdw()
        {
            Assert.AreEqual(0.17f, Element.Carbon.GetVdwRadius());
        }

        [Test]
        public void Element_AtomicWeight()
        {
            Assert.AreEqual(12.01f, Element.Carbon.GetStandardAtomicWeight());
        }

        [Test]
        public void FromSymbol()
        {
            Assert.AreEqual(Element.Carbon, ElementSymbols.GetFromSymbol("C"));
        }

        [Test]
        public void FromSymbol_MultipleCharacters()
        {
            Assert.AreEqual(Element.Chlorine, ElementSymbols.GetFromSymbol("Cl"));
        }

        [Test]
        public void FromSymbol_LowerCase()
        {
            Assert.AreEqual(Element.Chlorine, ElementSymbols.GetFromSymbol("cl"));
        }

        [Test]
        public void FromSymbol_UpperCase()
        {
            Assert.AreEqual(Element.Chlorine, ElementSymbols.GetFromSymbol("CL"));
        }

        [Test]
        public void FromSymbol_Whitespace()
        {
            Assert.AreEqual(Element.Chlorine, ElementSymbols.GetFromSymbol(" Cl  "));
        }

        [Test]
        public void FromSymbol_Missing()
        {
            Assert.AreEqual(null, ElementSymbols.GetFromSymbol("Kp"));
        }

        [Test]
        public void FromSymbol_Null()
        {
            Assert.AreEqual(null, ElementSymbols.GetFromSymbol(null));
        }

        [Test]
        public void FromSymbol_Blank()
        {
            Assert.AreEqual(null, ElementSymbols.GetFromSymbol(""));
        }
    }
}