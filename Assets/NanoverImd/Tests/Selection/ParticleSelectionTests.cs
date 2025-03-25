using System.Collections.Generic;
using NanoverImd.Selection;
using NUnit.Framework;
using Struct = System.Collections.Generic.Dictionary<string, object>;

namespace NanoverImd.Tests.Selection
{
    public class ParticleSelectionTests
    {
        [Test]
        public void ConstructorId()
        {
            var selection = new ParticleSelection("selection.some_id");
            Assert.AreEqual("selection.some_id", selection.ID);
        }

        [Test]
        public void DefaultsToNullSelection()
        {
            var selection = new ParticleSelection("selection.some_id");
            Assert.IsNull(selection.Selection);
        }

        [Test]
        public void DefaultsToEmptyProperties()
        {
            var selection = new ParticleSelection("selection.some_id");
            Assert.IsNotNull(selection.Properties);
            CollectionAssert.IsEmpty(selection.Properties);
        }

        [Test]
        public void GetIdFromConstructor()
        {
            var selection = new ParticleSelection(new Struct
            {
                [ParticleSelection.KeyId] = "selection.some_id"
            });
            Assert.AreEqual("selection.some_id", selection.ID);
        }

        [Test]
        public void NoIdInConstructor()
        {
            Assert.Throws<KeyNotFoundException>(() =>
            {
                var s = new ParticleSelection(new Struct());
            });
        }

        [Test]
        public void GetNameFromConstructor()
        {
            var selection = new ParticleSelection(new Struct
            {
                [ParticleSelection.KeyId] = "selection.some_id",
                [ParticleSelection.KeyName] = "Selection A"
            });
            Assert.AreEqual("Selection A", selection.Name);
        }

        [Test]
        public void GetNameFromUpdate()
        {
            var selection = new ParticleSelection(new Struct
            {
                [ParticleSelection.KeyId] = "selection.some_id"
            });
            selection.UpdateFromObject(new Struct()
            {
                [ParticleSelection.KeyName] = "Selection A"
            });
            Assert.AreEqual("Selection A", selection.Name);
        }

        [Test]
        public void GetSelectionFromConstructor()
        {
            var selection = new ParticleSelection(new Struct
            {
                [ParticleSelection.KeyId] = "selection.some_id",
                [ParticleSelection.KeySelected] = new Struct
                {
                    [ParticleSelection.KeyParticleIds] = new object[]
                    {
                        0.0, 1.0, 2.0
                    }
                }
            });
            CollectionAssert.AreEqual(new[]
            {
                0, 1, 2
            }, selection.Selection);
        }

        [Test]
        public void GetSelectionFromUpdate()
        {
            var selection = new ParticleSelection(new Struct
            {
                [ParticleSelection.KeyId] = "selection.some_id"
            });
            selection.UpdateFromObject(new Struct()
            {
                [ParticleSelection.KeySelected] = new Struct
                {
                    [ParticleSelection.KeyParticleIds] = new object[]
                    {
                        0.0, 1.0, 2.0
                    }
                }
            });
            CollectionAssert.AreEqual(new[]
            {
                0, 1, 2
            }, selection.Selection);
        }

        [Test]
        public void RootSelection()
        {
            var selection = ParticleSelection.CreateRootSelection();
            Assert.AreEqual(ParticleSelection.RootSelectionId, selection.ID);
            Assert.AreEqual(ParticleSelection.RootSelectionName, selection.Name);
            Assert.IsNull(selection.Selection);
            CollectionAssert.IsEmpty(selection.Properties);
        }

        [Test]
        public void HidePropertyDefault()
        {
            var selection = new ParticleSelection(new Struct
            {
                [ParticleSelection.KeyId] = "selection.some_id"
            });
            Assert.IsFalse(selection.HideRenderer);
        }

        [Test]
        public void HidePropertyConstructor()
        {
            var selection = new ParticleSelection(new Struct
            {
                [ParticleSelection.KeyId] = "selection.some_id",
                [ParticleSelection.KeyProperties] = new Struct
                {
                    [ParticleSelection.KeyHideProperty] = true
                }
            });
            Assert.IsTrue(selection.HideRenderer);
        }

        [Test]
        public void HidePropertyUpdate()
        {
            var selection = new ParticleSelection(new Struct
            {
                [ParticleSelection.KeyId] = "selection.some_id"
            });
            selection.UpdateFromObject(new Struct
            {
                [ParticleSelection.KeyProperties] = new Struct
                {
                    [ParticleSelection.KeyHideProperty] = true
                }
            });
            Assert.IsTrue(selection.HideRenderer);
        }

        [Test]
        public void ResetVelocityDefault()
        {
            var selection = new ParticleSelection(new Struct
            {
                [ParticleSelection.KeyId] = "selection.some_id"
            });
            Assert.IsFalse(selection.ResetVelocities);
        }

        [Test]
        public void ResetVelocityConstructor()
        {
            var selection = new ParticleSelection(new Struct
            {
                [ParticleSelection.KeyId] = "selection.some_id",
                [ParticleSelection.KeyProperties] = new Struct
                {
                    [ParticleSelection.KeyResetVelocities] = true
                }
            });
            Assert.IsTrue(selection.ResetVelocities);
        }

        [Test]
        public void ResetVelocityUpdate()
        {
            var selection = new ParticleSelection(new Struct
            {
                [ParticleSelection.KeyId] = "selection.some_id"
            });
            selection.UpdateFromObject(new Struct
            {
                [ParticleSelection.KeyProperties] = new Struct
                {
                    [ParticleSelection.KeyResetVelocities] = true
                }
            });
            Assert.IsTrue(selection.ResetVelocities);
        }

        [Test]
        public void InteractionMethodDefault()
        {
            var selection = new ParticleSelection(new Struct
            {
                [ParticleSelection.KeyId] = "selection.some_id"
            });
            Assert.AreEqual(ParticleSelection.InteractionMethodSingle, selection.InteractionMethod);
        }

        [Test]
        public void InteractionMethodConstructor()
        {
            var selection = new ParticleSelection(new Struct
            {
                [ParticleSelection.KeyId] = "selection.some_id",
                [ParticleSelection.KeyProperties] = new Struct
                {
                    [ParticleSelection.KeyInteractionMethod] =
                        ParticleSelection.InteractionMethodGroup
                }
            });
            Assert.AreEqual(ParticleSelection.InteractionMethodGroup, selection.InteractionMethod);
        }

        [Test]
        public void InteractionMethodUpdate()
        {
            var selection = new ParticleSelection(new Struct
            {
                [ParticleSelection.KeyId] = "selection.some_id"
            });
            selection.UpdateFromObject(new Struct
            {
                [ParticleSelection.KeyProperties] = new Struct
                {
                    [ParticleSelection.KeyInteractionMethod] =
                        ParticleSelection.InteractionMethodNone
                }
            });
            Assert.AreEqual(ParticleSelection.InteractionMethodNone, selection.InteractionMethod);
        }

        [Test]
        public void RendererDefault()
        {
            var selection = new ParticleSelection(new Struct
            {
                [ParticleSelection.KeyId] = "selection.some_id"
            });
            Assert.IsNull(selection.Renderer);
        }

        [Test]
        public void RendererConstructor()
        {
            var selection = new ParticleSelection(new Struct
            {
                [ParticleSelection.KeyId] = "selection.some_id",
                [ParticleSelection.KeyProperties] = new Struct
                {
                    [ParticleSelection.KeyRendererProperty] = "ball and stick"
                }
            });
            Assert.AreEqual("ball and stick", selection.Renderer);
        }

        [Test]
        public void RendererUpdate()
        {
            var selection = new ParticleSelection(new Struct
            {
                [ParticleSelection.KeyId] = "selection.some_id"
            });
            selection.UpdateFromObject(new Struct
            {
                [ParticleSelection.KeyProperties] = new Struct
                {
                    [ParticleSelection.KeyRendererProperty] = "ball and stick"
                }
            });
            Assert.AreEqual("ball and stick", selection.Renderer);
        }
    }
}