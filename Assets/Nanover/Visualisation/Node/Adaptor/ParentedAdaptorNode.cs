using System;
using Nanover.Core;
using Nanover.Visualisation.Components;
using Nanover.Visualisation.Properties;
using Nanover.Visualisation.Property;
using UnityEngine;

namespace Nanover.Visualisation.Node.Adaptor
{
    /// <summary>
    /// An <see cref="BaseAdaptorNode"/> which is linked to another adaptor. This adaptor contains its own properties, but links them to the parent. This means that the parent can be changed without listeners to this adaptor needing to change their links.
    /// </summary>
    [Serializable]
    public class ParentedAdaptorNode : BaseAdaptorNode
    {
        /// <inheritdoc cref="ParentAdaptor"/>
        [SerializeField]
        private FrameAdaptorProperty adaptor = new FrameAdaptorProperty();

        /// <summary>
        /// The adaptor that this adaptor inherits from.
        /// </summary>
        public IProperty<IDynamicPropertyProvider> ParentAdaptor => adaptor;

        /// <inheritdoc cref="BaseAdaptorNode.OnCreateProperty{T}"/>
        protected override void OnCreateProperty<T>(string key, IProperty<T> property)
        {
            base.OnCreateProperty(key, property);
            if (adaptor.HasNonNullValue())
                property.LinkedProperty = adaptor.Value.GetOrCreateProperty<T>(key);
        }

        /// <inheritdoc cref="BaseAdaptorNode.Refresh"/>
        public override void Refresh()
        {
            if (adaptor.IsDirty)
            {
                if (adaptor.HasNonNullValue())
                {
                    foreach (var (key, property) in Properties)
                        property.TrySetLinkedProperty(adaptor.Value.GetOrCreateProperty(key, property.PropertyType));
                }
                else
                {
                    foreach (var (key, property) in Properties)
                        property.TrySetLinkedProperty(null);
                }

                adaptor.IsDirty = false;
            }
        }
    }
}