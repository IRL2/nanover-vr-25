using System.Collections.Generic;
using NanoverImd.Selection;
using NUnit.Framework;
using UnityEngine;

namespace NanoverImd.Tests
{
    internal class TryParseColorTests
    {
        private static IEnumerable<(object value, Color color)> List3Parameters()
        {
            yield return (new object[]
                             {
                                 1.0, 0.0, 0.0
                             }, Color.red);
            yield return (new object[]
                             {
                                 0.0, 1.0, 0.0
                             }, Color.green);
            yield return (new object[]
                             {
                                 0.0, 0.0, 1.0
                             }, Color.blue);
            yield return (new object[]
                             {
                                 1.0, 1.0, 1.0
                             }, Color.white);
        }

        [Test]
        public void TestList3(
            [ValueSource(nameof(List3Parameters))] (object value, Color color) parameter)
        {
            Assert.IsTrue(VisualiserFactory.TryParseColor(parameter.value,
                                                          out var color));
            Assert.AreEqual(parameter.color, color);
        }

        private static IEnumerable<(string value, Color color)> HexParameters()
        {
            yield return ("0xff0000", Color.red);
            yield return ("0x00ff00", Color.green);
            yield return ("0x0000ff", Color.blue);
            yield return ("0xffffff", Color.white);
        }

        [Test]
        public void TestHex(
            [ValueSource(nameof(HexParameters))] (string value, Color color) parameter)
        {
            Assert.IsTrue(VisualiserFactory.TryParseColor(parameter.value,
                                                          out var color));
            Assert.AreEqual(parameter.color, color);
        }

        private static IEnumerable<(string value, Color color)> NameParameters()
        {
            yield return ("red", Color.red);
            yield return ("lime", Color.green);
            yield return ("blue", Color.blue);
            yield return ("white", Color.white);

            yield return ("Red", Color.red);
            yield return ("Lime", Color.green);
            yield return ("Blue", Color.blue);
            yield return ("White", Color.white);

            yield return ("RED", Color.red);
            yield return ("LIME", Color.green);
            yield return ("BLUE", Color.blue);
            yield return ("WHITE", Color.white);
        }

        [Test]
        public void TestName(
            [ValueSource(nameof(NameParameters))] (string value, Color color) parameter)
        {
            Assert.IsTrue(VisualiserFactory.TryParseColor(parameter.value,
                                                          out var color));

            Assert.AreEqual(parameter.color, color);
        }
    }
}