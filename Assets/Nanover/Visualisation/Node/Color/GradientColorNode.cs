using System;
using Nanover.Visualisation.Properties;
using Nanover.Visualisation.Properties.Collections;
using Nanover.Visualisation.Property;
using UnityEngine;

namespace Nanover.Visualisation.Node.Color
{
    [Serializable]
    public class GradientColorNode : VisualiserColorNode
    {
        [SerializeField]
        private GradientProperty gradient;

        public IProperty<Gradient> Gradient => gradient;

        [SerializeField]
        private FloatArrayProperty values;

        public IProperty<float[]> Values => values;

        [SerializeField]
        private FloatProperty minimumValue = new FloatProperty
        {
            Value = 0
        };

        [SerializeField]
        private FloatProperty maximumValue = new FloatProperty
        {
            Value = 1
        };

        protected override bool IsInputValid => gradient.HasNonNullValue() &&
                                                values.HasNonEmptyValue() &&
                                                minimumValue.HasNonNullValue() &&
                                                maximumValue.HasNonNullValue();

        protected override bool IsInputDirty => gradient.IsDirty
                                             || values.IsDirty
                                             || minimumValue.IsDirty
                                             || maximumValue.IsDirty;

        protected override void ClearDirty()
        {
            gradient.IsDirty = false;
            values.IsDirty = false;
            minimumValue.IsDirty = false;
            maximumValue.IsDirty = false;
        }

        protected override void UpdateOutput()
        {
            var arr = colors.HasValue ? colors.Value : new UnityEngine.Color[0];
            var values = this.values.Value;
            var minimumValue = this.minimumValue.Value;
            var maximumValue = this.maximumValue.Value;
            var gradient = this.gradient.Value;
            var count = values.Length;
            Array.Resize(ref arr, count);
            for (var i = 0; i < count; i++)
                arr[i] = gradient.Evaluate((values[i] - minimumValue) /
                                           (maximumValue - minimumValue));
            colors.Value = arr;
        }

        protected override void ClearOutput()
        {
            colors.UndefineValue();
        }
    }
}