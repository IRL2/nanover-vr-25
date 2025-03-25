using Nanover.Visualisation.Properties;
using Nanover.Visualisation.Properties.Collections;
using Nanover.Visualisation.Property;
using UnityEngine;

namespace Nanover.Visualisation.Tests.Property
{
    internal class IntPropertyTests : PropertyTests<IntProperty, int>
    {
        protected override int ExampleNonNullValue => 1;
        protected override int DifferentNonNullValue => -4;
    }
    
    internal class IntArrayPropertyTests : ArrayPropertyTests<IntArrayProperty, int>
    {
        protected override int[] ExampleNonNullValue => new [] {1, 3};
        protected override int[] DifferentNonNullValue => new[] {-2, 0};
    }

    internal class FloatPropertyTests : PropertyTests<FloatProperty, float>
    {
        protected override float ExampleNonNullValue => 1.1f;
        protected override float DifferentNonNullValue => -22013.3223f;
    }
    
    internal class FloatArrayPropertyTests : ArrayPropertyTests<FloatArrayProperty, float>
    {
        protected override float[] ExampleNonNullValue => new [] {1f, 2.5f};
        protected override float[] DifferentNonNullValue => new[] {-0.5f, 42425.332f};
    }

    internal class Vector3PropertyTests : PropertyTests<Vector3Property, Vector3>
    {
        protected override Vector3 ExampleNonNullValue => Vector3.zero;
        protected override Vector3 DifferentNonNullValue => Vector3.left * 2f;
    }
    
    internal class Vector3ArrayPropertyTests : ArrayPropertyTests<Vector3ArrayProperty, Vector3>
    {
        protected override Vector3[] ExampleNonNullValue => new[] {Vector3.zero, Vector3.right};
        protected override Vector3[] DifferentNonNullValue => new[] {Vector3.left * 2f, 
            new Vector3(0.5f, -24.2f, 53.64f) };
    }

    internal class ColorPropertyTests : PropertyTests<ColorProperty, Color>
    {
        protected override Color ExampleNonNullValue => Color.red;
        protected override Color DifferentNonNullValue => Color.black;
    }
    
    internal class ColorArrayPropertyTests : ArrayPropertyTests<ColorArrayProperty, Color>
    {
        protected override Color[] ExampleNonNullValue => new[] {Color.red, Color.green};
        protected override Color[] DifferentNonNullValue => new[] {Color.yellow, Color.black};
    }
}