using System;
using Nanover.Core;
using Nanover.Core.Science;
using Nanover.Visualisation.Properties;
using Nanover.Visualisation.Property;
using UnityEngine;

namespace Nanover.Visualisation.Node.Color
{
    /// <summary>
    /// Visualiser node that generates atomic colors from atomic positions, based
    /// upon an <see cref="ElementColorMapping" />
    /// </summary>
    [Serializable]
    public class ElementColorMappingNode : PerElementColorNode
    {
        [SerializeField]
        private ElementColorMappingProperty mapping = new ElementColorMappingProperty();

        /// <summary>
        /// The color mapping between elements and colors.
        /// </summary>
        public IProperty<IMapping<Element, UnityEngine.Color>> Mapping => mapping;

        /// <inheritdoc cref="GenericOutputNode.IsInputDirty"/>
        protected override bool IsInputDirty => base.IsInputDirty || mapping.IsDirty;

        /// <inheritdoc cref="GenericOutputNode.IsInputValid"/>
        protected override bool IsInputValid => base.IsInputValid && mapping.HasNonNullValue();

        /// <inheritdoc cref="GenericOutputNode.ClearDirty"/>
        protected override void ClearDirty()
        {
            base.ClearDirty();
            mapping.IsDirty = false;
        }

        /// <inheritdoc cref="PerElementColorNode.GetColor" />
        protected override UnityEngine.Color GetColor(Element element)
        {
            return mapping.HasNonNullValue()
                       ? mapping.Value.Map(element)
                       : UnityEngine.Color.white;
        }
    }
}