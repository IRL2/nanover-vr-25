using System.Collections.Generic;
using System.Linq;
using Nanover.Frontend.Input;
using Nanover.Testing;
using NUnit.Framework;
using UnityEngine;

namespace Nanover.Frontend.Tests.Input
{
    internal class DirectButtonTests
    {
        private static IEnumerable<bool> PressReleaseSequence =>
            RandomTestData.SeededRandom(() => Random.value > 0.5f).Take(64);

        [Test]
        public void Pressed_WithSinglePress_InvokedAfterPress()
        {
            var button = new DirectButton();
            var wasPressed = false;

            button.Pressed += () => wasPressed = true;
            button.Press();

            Assert.IsTrue(wasPressed);
        }

        [Test]
        public void Released_WithSingleRelease_InvokedAfterPressRelease()
        {
            var button = new DirectButton();
            var wasReleased = false;

            button.Released += () => wasReleased = true;

            button.Press();
            button.Release();

            Assert.IsTrue(wasReleased);
        }

        [Test]
        public void IsHeldDown_WithSinglePress_IsTrueWhenPressed()
        {
            var button = new DirectButton();
            button.Pressed += () => Assert.IsTrue(button.IsPressed);
            button.Press();
        }

        [Test]
        public void IsHeldDown_WithSingleRelease_IsFalseWhenReleased()
        {
            var button = new DirectButton();
            button.Released += () => Assert.IsFalse(button.IsPressed);
            button.Release();
        }

        [Test]
        public void PressedReleased_WithPressReleaseSequence_NeverFiresWithNoChange()
        {
            var button = new DirectButton();

            var expectEvent = false;

            button.Pressed += () => Assert.IsTrue(expectEvent);
            button.Released += () => Assert.IsTrue(expectEvent);

            foreach (bool willPress in PressReleaseSequence)
            {
                expectEvent = (willPress != button.IsPressed);

                if (willPress)
                {
                    button.Press();
                }
                else
                {
                    button.Release();
                }
            }
        }
    }
}