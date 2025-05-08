using Nanover.Visualisation.Properties;
using Nanover.Visualisation.Properties.Collections;
using Nanover.Visualisation.Property;
using NUnit.Framework;

namespace Nanover.Visualisation.Tests.Property
{
    internal class CountPropertyTests
    {
        [Test]
        public void CountProperty()
        {
            var output = new FloatArrayProperty()
            {
                Value = new[]
                {
                    1.0f, 2.0f
                }
            };

            var input = output.Count();

            Assert.AreEqual(2, input.Value);
        }

        [Test]
        public void CountProperty_Updated()
        {
            var output = new FloatArrayProperty()
            {
                Value = new[]
                {
                    1.0f, 2.0f
                }
            };

            var input = new IntProperty
            {
                LinkedProperty = output.Count()
            };

            Assert.AreEqual(2, input.Value);
            Assert.IsTrue(input.IsDirty);

            input.IsDirty = false;

            output.Value = new[]
            {
                2.0f, -1.0f, 3.0f
            };

            Assert.AreEqual(3, input.Value);
            Assert.IsTrue(input.IsDirty);
        }
    }
}