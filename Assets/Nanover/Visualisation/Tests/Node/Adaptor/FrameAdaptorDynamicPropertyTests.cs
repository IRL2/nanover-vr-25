using Nanover.Visualisation.Node.Adaptor;
using Nanover.Visualisation.Property;
using NUnit.Framework;

namespace Nanover.Visualisation.Tests.Node.Adaptor
{
    public class FrameAdaptorDynamicPropertyTests
    {
        [Test]
        public void InitialNoProperties()
        {
            var adaptor = new FrameAdaptorNode();

            CollectionAssert.IsEmpty(adaptor.GetProperties());
        }

        [Test]
        public void GetExistingNullProperty()
        {
            var adaptor = new FrameAdaptorNode();

            Assert.IsNull(adaptor.GetProperty("missing.property"));
        }

        [Test]
        public void GetExistingProperty_WrongName()
        {
            var adaptor = new FrameAdaptorNode();

            var property = adaptor.GetOrCreateProperty<int>("property");

            Assert.IsNull(adaptor.GetProperty("missing.property"));
        }

        [Test]
        public void GetExistingProperty()
        {
            var adaptor = new FrameAdaptorNode();

            var property = adaptor.GetOrCreateProperty<int>("property");

            Assert.IsNotNull(property);
            Assert.AreEqual(property, adaptor.GetProperty("property"));
        }

        [Test]
        public void GetExistingProperties()
        {
            var adaptor = new FrameAdaptorNode();

            var property1 = adaptor.GetOrCreateProperty<int>("property1");
            var property2 = adaptor.GetOrCreateProperty<bool>("property2");

            var expected = new (string, IReadOnlyProperty)[]
            {
                ("property1", property1), ("property2", property2)
            };

            CollectionAssert.AreEquivalent(expected, adaptor.GetProperties());
        }
    }
}