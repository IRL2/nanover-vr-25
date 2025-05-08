using System;
using Nanover.Visualisation.Property;
using NSubstitute;
using NUnit.Framework;

namespace Nanover.Visualisation.Tests.Property
{
    public abstract class PropertyTests<TProperty, TValue>
        where TProperty : SerializableProperty<TValue>, new()
    {
        protected virtual bool IsReferenceType => typeof(TValue).IsByRef;

        protected abstract TValue ExampleNonNullValue { get; }

        protected abstract TValue DifferentNonNullValue { get; }

        [Test]
        public void Initial_HasValue()
        {
            var property = new TProperty();

            Assert.IsFalse(property.HasValue);
        }

        [Test]
        public void Initial_HasNonNullValue()
        {
            var property = new TProperty();

            Assert.IsFalse(property.HasNonNullValue());
        }

        [Test]
        public void Initial_DefaultValue_HasValue()
        {
            var property = new TProperty()
            {
                Value = default
            };

            Assert.IsTrue(property.HasValue);
        }
        
        [Test]
        public void Initial_DefaultValue_GetValue()
        {
            var property = new TProperty()
            {
                Value = default
            };

            Assert.AreEqual(default(TValue), property.Value);
        }

        [Test]
        public void Initial_DefaultValue_HasNonNullValue()
        {
            var property = new TProperty()
            {
                Value = default
            };

            Assert.AreEqual(!IsReferenceType, property.HasNonNullValue());
        }

        [Test]
        public void Initial_ExampleValue_HasValue()
        {
            var property = new TProperty()
            {
                Value = ExampleNonNullValue
            };

            Assert.IsTrue(property.HasValue);
        }
        
        [Test]
        public void Initial_ExampleValue_GetValue()
        {
            var property = new TProperty()
            {
                Value = ExampleNonNullValue
            };

            Assert.AreEqual(ExampleNonNullValue, property.Value);
        }

        [Test]
        public void Initial_ExampleValue_HasNonNullValue()
        {
            var property = new TProperty()
            {
                Value = ExampleNonNullValue
            };

            Assert.IsTrue(property.HasNonNullValue());
        }
        
        [Test]
        public void Initial_GetValue_ThrowsException()
        {
            var property = new TProperty();

            TValue value;
            Assert.Throws<InvalidOperationException>(() => value = property.Value);
        }

        [Test]
        public void UndefineValue_HasValue()
        {
            var property = new TProperty()
            {
                Value = ExampleNonNullValue
            };

            property.UndefineValue();

            Assert.IsFalse(property.HasValue);
        }

        [Test]
        public void UndefineValue_HasNonNullValue()
        {
            var property = new TProperty()
            {
                Value = ExampleNonNullValue
            };

            property.UndefineValue();

            Assert.IsFalse(property.HasNonNullValue());
        }

        [Test]
        public void UndefineValue_Value_Exception()
        {
            var property = new TProperty()
            {
                Value = ExampleNonNullValue
            };

            property.UndefineValue();

            TValue value;
            Assert.Throws<InvalidOperationException>(() => value = property.Value);
        }

        [Test]
        public void ValueChanged_WhenDifferentValueSet()
        {
            var property = new TProperty()
            {
                Value = ExampleNonNullValue
            };

            var callback = Substitute.For<Action>();

            property.ValueChanged += callback;

            property.Value = DifferentNonNullValue;

            callback.Received(1);
        }

        [Test]
        public void ValueChanged_WhenValueSetTwice()
        {
            var property = new TProperty()
            {
                Value = ExampleNonNullValue
            };

            var callback = Substitute.For<Action>();

            property.ValueChanged += callback;

            property.Value = ExampleNonNullValue;

            callback.Received(1);
        }

        [Test]
        public void ValueChanged_WhenUndefined()
        {
            var property = new TProperty()
            {
                Value = ExampleNonNullValue
            };

            var callback = Substitute.For<Action>();

            property.ValueChanged += callback;

            property.UndefineValue();

            callback.Received(1);
        }

        [Test]
        public void ValueChanged_WhenUndefinedTwice()
        {
            var property = new TProperty()
            {
                Value = ExampleNonNullValue
            };

            var callback = Substitute.For<Action>();

            property.ValueChanged += callback;

            property.UndefineValue();

            property.UndefineValue();

            callback.Received(1);
        }

        [Test]
        public void ValueSetter()
        {
            var property = new TProperty()
            {
                Value = ExampleNonNullValue
            };

            property.Value = DifferentNonNullValue;

            Assert.AreEqual(DifferentNonNullValue, property.Value);
        }

        [Test]
        public void Initial_HasLinkedProperty()
        {
            var input = new TProperty();
            Assert.IsFalse(input.HasLinkedProperty);
        }

        [Test]
        public void Initial_LinkedProperty_IsNull()
        {
            var input = new TProperty();
            Assert.IsNull(input.LinkedProperty);
        }

        [Test]
        public void Linked_HasLinkedProperty()
        {
            var first = new TProperty();
            var second = new TProperty();
            second.LinkedProperty = first;

            Assert.IsTrue(second.HasLinkedProperty);
        }

        [Test]
        public void Linked_LinkedProperty()
        {
            var first = new TProperty();
            var second = new TProperty();
            second.LinkedProperty = first;

            Assert.AreEqual(first, second.LinkedProperty);
        }

        [Test]
        public void Linked_NoValue_HasValue()
        {
            var first = new TProperty();
            var second = new TProperty();
            second.LinkedProperty = first;

            Assert.IsFalse(second.HasValue);
        }

        [Test]
        public void Linked_NoValue_Value_Exception()
        {
            var first = new TProperty();
            var second = new TProperty();
            second.LinkedProperty = first;

            TValue value;
            Assert.Throws<InvalidOperationException>(() => value = second.Value);
        }

        [Test]
        public void Linked_ValueBeforeLinking_HasValue()
        {
            var first = new TProperty();
            var second = new TProperty();
            first.Value = ExampleNonNullValue;
            second.LinkedProperty = first;

            Assert.IsTrue(second.HasValue);
        }
        
        [Test]
        public void Linked_ValueBeforeLinking_GetValue()
        {
            var first = new TProperty();
            var second = new TProperty();
            first.Value = ExampleNonNullValue;
            second.LinkedProperty = first;

            Assert.AreEqual(first.Value, second.Value);
        }

        [Test]
        public void Linked_ValueAfterLinking_HasValue()
        {
            var first = new TProperty();
            var second = new TProperty();
            second.LinkedProperty = first;

            first.Value = ExampleNonNullValue;

            Assert.IsTrue(second.HasValue);
        }
        
        [Test]
        public void Linked_ValueAfterLinking_GetValue()
        {
            var first = new TProperty();
            var second = new TProperty();
            second.LinkedProperty = first;

            first.Value = ExampleNonNullValue;

            Assert.AreEqual(first.Value, second.Value);
        }

        [Test]
        public void ValueOverridesLink_HasLinkedProperty()
        {
            var input = new TProperty();
            var output = new TProperty()
            {
                Value = ExampleNonNullValue
            };
            input.LinkedProperty = output;

            input.Value = DifferentNonNullValue;

            Assert.IsFalse(input.HasLinkedProperty);
        }

        [Test]
        public void ValueOverridesLink_LinkedProperty()
        {
            var input = new TProperty();
            var output = new TProperty()
            {
                Value = ExampleNonNullValue
            };
            input.LinkedProperty = output;

            input.Value = DifferentNonNullValue;

            Assert.IsNull(input.LinkedProperty);
        }

        [Test]
        public void ValueOverridesLink_HasValue()
        {
            var input = new TProperty();
            var output = new TProperty()
            {
                Value = ExampleNonNullValue
            };
            input.LinkedProperty = output;

            input.Value = DifferentNonNullValue;

            Assert.IsTrue(input.HasValue);
        }

        [Test]
        public void ValueOverridesLink_Value()
        {
            var input = new TProperty();
            var output = new TProperty()
            {
                Value = ExampleNonNullValue
            };
            input.LinkedProperty = output;

            input.Value = DifferentNonNullValue;

            Assert.AreEqual(DifferentNonNullValue, input.Value);
        }

        [Test]
        public void ValueOverridesLink_OutputUnchanged()
        {
            var input = new TProperty();
            var output = new TProperty()
            {
                Value = ExampleNonNullValue
            };
            input.LinkedProperty = output;

            input.Value = DifferentNonNullValue;

            Assert.AreEqual(ExampleNonNullValue, output.Value);
        }

        [Test]
        public void ValueOverridesLink_DirtyNoLongerOccurs()
        {
            var second = new TProperty();
            var first = new TProperty()
            {
                Value = ExampleNonNullValue
            };
            second.LinkedProperty = first;

            second.Value = DifferentNonNullValue;
            second.IsDirty = false;

            first.Value = DifferentNonNullValue;

            Assert.IsFalse(second.IsDirty);
        }

        [Test]
        public void UndefineValueOverridesLink_HasLinkedProperty()
        {
            var input = new TProperty();
            var output = new TProperty()
            {
                Value = ExampleNonNullValue
            };
            input.LinkedProperty = output;

            input.UndefineValue();

            Assert.IsFalse(input.HasLinkedProperty);
        }

        [Test]
        public void UndefineValueOverridesLink_LinkedProperty()
        {
            var input = new TProperty();
            var output = new TProperty()
            {
                Value = ExampleNonNullValue
            };
            input.LinkedProperty = output;

            input.UndefineValue();

            Assert.IsNull(input.LinkedProperty);
        }

        [Test]
        public void UndefineValueOverridesLink_HasValue()
        {
            var input = new TProperty();
            var output = new TProperty()
            {
                Value = ExampleNonNullValue
            };
            input.LinkedProperty = output;

            input.UndefineValue();

            Assert.IsFalse(input.HasValue);
        }

        [Test]
        public void UndefineValueOverridesLink_Value()
        {
            var input = new TProperty();
            var output = new TProperty()
            {
                Value = ExampleNonNullValue
            };
            input.LinkedProperty = output;

            input.UndefineValue();

            TValue value;
            Assert.Throws<InvalidOperationException>(() => value = input.Value);
        }

        [Test]
        public void UndefineValueOverridesLink_OutputUnchanged()
        {
            var input = new TProperty();
            var output = new TProperty()
            {
                Value = ExampleNonNullValue
            };
            input.LinkedProperty = output;

            input.UndefineValue();

            Assert.AreEqual(ExampleNonNullValue, output.Value);
        }

        [Test]
        public void UndefineValueOverridesLink_DirtyNoLongerOccurs()
        {
            var input = new TProperty();
            var output = new TProperty()
            {
                Value = ExampleNonNullValue
            };
            input.LinkedProperty = output;

            input.UndefineValue();
            input.IsDirty = false;

            output.Value = DifferentNonNullValue;

            Assert.IsFalse(input.IsDirty);
        }


        [Test]
        public void LinkToSelf_Exception()
        {
            var input = new TProperty();
            Assert.Throws<ArgumentException>(() => input.LinkedProperty = input);
        }

        [Test]
        public void CyclicLink_TwoProperties()
        {
            var input = new TProperty();
            var input2 = new TProperty();
            input2.LinkedProperty = input;
            Assert.Throws<ArgumentException>(() => input.LinkedProperty = input2);
        }

        [Test]
        public void CyclicLink_ThreeProperties()
        {
            var input = new TProperty();
            var input2 = new TProperty();
            var input3 = new TProperty();
            input2.LinkedProperty = input;
            input3.LinkedProperty = input2;
            Assert.Throws<ArgumentException>(() => input.LinkedProperty = input3);
        }

        [Test]
        public void CyclicLink_FourProperties()
        {
            var input = new TProperty();
            var input2 = new TProperty();
            var input3 = new TProperty();
            var input4 = new TProperty();
            input2.LinkedProperty = input;
            input3.LinkedProperty = input2;
            input4.LinkedProperty = input3;
            Assert.Throws<ArgumentException>(() => input.LinkedProperty = input4);
        }

        [Test]
        public void ChangeLink_NoLongerDirtyIfOldLinkChanges()
        {
            var input = new TProperty();
            var output1 = new TProperty();
            var output2 = new TProperty();

            input.LinkedProperty = output1;

            input.LinkedProperty = output2;
            input.IsDirty = false;

            output1.Value = DifferentNonNullValue;

            Assert.IsFalse(input.IsDirty);
        }

        [Test]
        public void Initial_IsDirty()
        {
            var input = new TProperty();

            Assert.IsTrue(input.IsDirty);
        }

        [Test]
        public void IsDirty_SetTrue()
        {
            var input = new TProperty
            {
                IsDirty = true
            };


            Assert.IsTrue(input.IsDirty);
        }

        [Test]
        public void IsDirty_SetFalse()
        {
            var input = new TProperty
            {
                IsDirty = false
            };

            Assert.IsFalse(input.IsDirty);
        }
        
        [Test]
        public void ImplicitConversion()
        {
            var property = new TProperty
            {
                Value = ExampleNonNullValue
            };

            TValue value = property;
            
            Assert.AreEqual(ExampleNonNullValue, value);
        }
        
        [Test]
        public void ImplicitConversion_NullValue_ThrowsException()
        {
            var property = new TProperty();

            Assert.Throws<InvalidOperationException>(() =>
            {
                TValue value = property;
            });
        }
        
        [Test]
        public void PropertyType()
        {
            var property = new TProperty();

           Assert.AreEqual(typeof(TValue), property.PropertyType);
        }

        [Test]
        public void Chain_HasValue()
        {
            var first = new TProperty();
            var second = new TProperty();
            var third = new TProperty();

            first.Value = ExampleNonNullValue;
            second.LinkedProperty = first;
            third.LinkedProperty = second;
            
            Assert.AreEqual(ExampleNonNullValue, third.Value);
        }
        
        [Test]
        public void Chain_ReceivesUpdate()
        {
            var first = new TProperty();
            var second = new TProperty();
            var third = new TProperty();

            second.LinkedProperty = first;
            third.LinkedProperty = second;

            var callback = Substitute.For<Action>();

            third.ValueChanged += callback;

            first.Value = ExampleNonNullValue;
            
            callback.Received(1);
        }
    }
}