using System;
using Nanover.Visualisation.Property;
using NSubstitute;
using NUnit.Framework;

namespace Nanover.Visualisation.Tests.Property
{
    public abstract class ArrayPropertyTests<TProperty, TElement> : PropertyTests<TProperty, TElement[]>
        where TProperty : SerializableProperty<TElement[]>, new()
    {
        protected TElement[] EmptyArray => new TElement[0];

        protected override bool IsReferenceType => true;

        [Test]
        public void InitialProperty_HasNonEmptyValue()
        {
            var property = new TProperty();

            Assert.IsFalse(property.HasNonEmptyValue());
        }
        
        [Test]
        public void HasNonEmptyValue_Empty()
        {
            var property = new TProperty()
            {
                Value = EmptyArray
            };

            Assert.IsFalse(property.HasNonEmptyValue());
        }
        
        [Test]
        public void HasNonEmptyValue_NonEmpty()
        {
            var property = new TProperty()
            {
                Value = ExampleNonNullValue
            };

            Assert.IsTrue(property.HasNonEmptyValue());
        }

    }
}