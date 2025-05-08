using System;
using Nanover.Core.Science;
using Nanover.Visualisation.Properties.Collections;
using Nanover.Visualisation.Property;
using UnityEngine;

namespace Nanover.Visualisation.Node.Color
{
    /// <summary>
    /// Base code for a Visualiser node which generates colors based upon atomic
    /// elements.
    /// </summary>
    [Serializable]
    public abstract class PerElementColorNode : VisualiserColorNode
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
            var colorArray = colors.HasValue ? colors.Value : new UnityEngine.Color[0];
            Array.Resize(ref colorArray, elements.Value.Length);
            for (var i = 0; i < elements.Value.Length; i++)
                colorArray[i] = GetColor(elementArray[i]);

            colors.Value = colorArray;
        }

        /// <inheritdoc cref="GenericOutputNode.ClearOutput"/>
        protected override void ClearOutput()
        {
            colors.UndefineValue();
        }

        /// <summary>
        /// Get the color for the given atomic element.
        /// </summary>
        protected abstract UnityEngine.Color GetColor(Element element);
    }
}