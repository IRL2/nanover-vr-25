using System;
using Nanover.Core.Science;
using Nanover.Visualisation.Properties;
using Nanover.Visualisation.Property;
using UnityEngine;

namespace Nanover.Visualisation.Node.Scale
{
    /// <summary>
    /// Visualiser node which scales each particle by its covalent radius
    /// </summary>
    [Serializable]
    public class CovalentScaleNode : PerElementScaleNode
    {
        /// <summary>
        /// Multiplier for each radius.
        /// </summary>
        public IProperty<float> Scale => scale;

        [SerializeField]
        private FloatProperty scale = new FloatProperty
        {
            Value = 1f
        };

        /// <inheritdoc cref="PerElementScaleNode.ClearDirty"/>
        protected override void ClearDirty()
        {
            base.ClearDirty();
            scale.IsDirty = false;
        }

        /// <inheritdoc cref="PerElementScaleNode.IsInputDirty"/>
        protected override bool IsInputDirty => base.IsInputDirty || scale.IsDirty;

        /// <inheritdoc cref="PerElementScaleNode.IsInputValid"/>
        protected override bool IsInputValid => base.IsInputValid && scale.HasNonNullValue();

        /// <summary>
        /// Get the scale of the provided atomic element.
        /// </summary>
        protected override float GetScale(Element element)
        {
            return (element.GetCovalentRadius() ?? 1f) * scale.Value;
        }
    }
}