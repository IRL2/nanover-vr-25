using Nanover.Core.Science;
using NUnit.Framework;
using UnityEngine;

namespace Nanover.Frame.Tests
{
    public class FrameTests
    {
        [Test]
        public void ShallowCopy()
        {
            var originalFrame = new Frame()
            {
                ParticlePositions = new[] { Vector3.one, Vector3.left },
                ParticleElements = new[] { Element.Hydrogen, Element.Oxygen }
            };
            originalFrame.Data["customdata"] = 1.4f;

            var clonedFrame = Frame.ShallowCopy(originalFrame);

            CollectionAssert.AreEqual(originalFrame.ParticlePositions,
                                      clonedFrame.ParticlePositions);
            CollectionAssert.AreEqual(originalFrame.ParticleElements, clonedFrame.ParticleElements);
            Assert.AreEqual(originalFrame.Data["customdata"], clonedFrame.Data["customdata"]);
        }

        [Test]
        public void ParticlesIteration()
        {
            var positions = new[] { Vector3.one, Vector3.left };
            var frame = new Frame()
            {
                ParticlePositions = positions
            };

            var index = 0;
            foreach (var particle in frame.Particles)
            {
                Assert.AreEqual(positions[index], particle.Position);
                index++;
            }
        }

        [Test]
        public void SetFloatValue()
        {
            var frame = new Frame();
            frame.Data["id"] = 0.5f;
            Assert.AreEqual(0.5f, frame.Data["id"]);
        }

        [Test]
        public void SetStringValue()
        {
            var frame = new Frame();
            frame.Data["id"] = "test";
            Assert.AreEqual("test", frame.Data["id"]);
        }

        [Test]
        public void HasFloatValue()
        {
            var frame = new Frame();
            frame.Data["id"] = 0.5f;
            Assert.IsTrue(frame.Data.ContainsKey("id"));
        }

        [Test]
        public void Element_Provided()
        {
            var frame = new Frame()
            {
                ParticlePositions = new[] { Vector3.zero },
                ParticleElements = new[] { Element.Carbon }
            };
            Assert.AreEqual(Element.Carbon, frame.Particles[0].Element);
        }

        [Test]
        public void Element_Missing()
        {
            var frame = new Frame()
            {
                ParticlePositions = new[] { Vector3.zero }
            };
            Assert.IsNull(frame.Particles[0].Element);
        }

        [Test]
        public void Index()
        {
            var frame = new Frame()
            {
                ParticlePositions = new[] { Vector3.zero }
            };
            Assert.AreEqual(0, frame.Particles[0].Index);
        }

        [Test]
        public void Type_Provided()
        {
            var frame = new Frame()
            {
                ParticlePositions = new[] { Vector3.zero },
                ParticleTypes = new[] { "type" }
            };
            Assert.AreEqual("type", frame.Particles[0].Type);
        }

        [Test]
        public void Type_Missing()
        {
            var frame = new Frame()
            {
                ParticlePositions = new[] { Vector3.zero }
            };
            Assert.IsNull(frame.Particles[0].Type);
        }


        [Test]
        public void Particles_AsIEnumerable()
        {
            var frame = new Frame()
            {
                ParticlePositions = new[] { Vector3.zero, Vector3.one },
            };
            CollectionAssert.IsNotEmpty(frame.Particles);
        }
        
        [Test]
        public void ParticleNames_Assignment()
        {
            var frame = new Frame
            {
                ParticleNames = new[] {"abc", "def"}
            };
            CollectionAssert.AreEqual(new[] {"abc", "def"}, frame.ParticleNames);
        }
        
        [Test]
        public void ResidueNames_Assignment()
        {
            var frame = new Frame
            {
                ResidueNames = new[] {"abc", "def"}
            };
            CollectionAssert.AreEqual(new[] {"abc", "def"}, frame.ResidueNames);
        }
        
        [Test]
        public void ParticleResidues_Assignment()
        {
            var frame = new Frame
            {
                ParticleResidues = new[] {1, 3}
            };
            CollectionAssert.AreEqual(new[] {1, 3}, frame.ParticleResidues);
        }
        
        [Test]
        public void BondOrders_Assignment()
        {
            var frame = new Frame
            {
                BondOrders = new[] {1, 3}
            };
            CollectionAssert.AreEqual(new[] {1, 3}, frame.BondOrders);
        }
        
        [Test]
        public void Bonds_Assignment()
        {
            var frame = new Frame
            {
                BondPairs = new[] { new BondPair(0, 1), new BondPair(1,2)}  
            };
            CollectionAssert.AreEqual( new[] { new BondPair(0, 1), new BondPair(1,2)}  , frame.Bonds);
        }
    }
}