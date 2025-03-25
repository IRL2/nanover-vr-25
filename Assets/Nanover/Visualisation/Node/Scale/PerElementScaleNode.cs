using System;
using Nanover.Core.Science;
using Nanover.Visualisation.Properties.Collections;
using Nanover.Visualisation.Property;
using UnityEngine;

namespace Nanover.Visualisation.Node.Scale
{
    /// <summary>
    /// Base code for visualiser node which generates scales based upon atomic
    /// elements.
    /// </summary>
    [Serializable]
    public abstract class PerElementScaleNode : VisualiserScaleNode
    {
        [SerializeField]
        private ElementArrayProperty elements = new ElementArrayProperty();

        /// <summary>
        /// Atomic element array input.
        /// </summary>
        public IProperty<Element[]> Elements => elements;

        /// <inheritdoc cref="GenericOutputNode.IsInputDirty"/>
        protected override bool IsInputDirty => elements.IsDirty;

        /// <inheritdoc cref="GenericOutputNode.IsInputValid"/>
        protected override bool IsInputValid => elements.HasNonEmptyValue();

        /// <inheritdoc cref="GenericOutputNode.ClearDirty"/>
        protected override void ClearDirty()
        {
            elements.IsDirty = false;
        }
        
        /// <inheritdoc cref="GenericOutputNode.UpdateOutput"/>
        protected override void UpdateOutput()
        {
            var elementArray = elements.Value;
            var scaleArray = scales.HasValue ? scales.Value : new float[0];
            Array.Resize(ref scaleArray, elements.Value.Length);
            for (var i = 0; i < elements.Value.Length; i++)
                scaleArray[i] = GetScale(elementArray[i]);

            scales.Value = scaleArray;
        }
        
        /// <inheritdoc cref="GenericOutputNode.ClearOutput"/>
        protected override void ClearOutput()
        {
            scales.UndefineValue();
        }

        /// <summary>
        /// Get the scale for the given atomic element.
        /// </summary>
        protected abstract float GetScale(Element element);
    }
}