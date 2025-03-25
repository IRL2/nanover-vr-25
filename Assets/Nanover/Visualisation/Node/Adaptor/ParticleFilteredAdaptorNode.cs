using System;
using System.Collections.Generic;
using Nanover.Core;
using Nanover.Frame;
using Nanover.Visualisation.Properties;
using Nanover.Visualisation.Property;
using UnityEngine;

namespace Nanover.Visualisation.Node.Adaptor
{
    /// <summary>
    /// A variation of a <see cref="FrameAdaptorNode"/> which applies a filter to any key of the form 'particle.*'.
    /// </summary>
    [Serializable]
    public class ParticleFilteredAdaptorNode : ParentedAdaptorNode
    {
        /// <summary>
        /// A filter which will affect all fields of the form 'particle.*'.
        /// </summary>
        public IProperty<int[]> ParticleFilter => particleFilter;

        [SerializeField]
        private IntArrayProperty particleFilter = new IntArrayProperty();

        private readonly Dictionary<string, IReadOnlyProperty> filteredProperties =
            new Dictionary<string, IReadOnlyProperty>();

        /// <inheritdoc cref="BaseAdaptorNode.GetProperties"/>
        public override IEnumerable<(string name, IReadOnlyProperty property)> GetProperties()
        {
            foreach (var (key, value) in filteredProperties)
                yield return (key, value);
        }

        /// <inheritdoc cref="BaseAdaptorNode.GetProperty"/>
        public override IReadOnlyProperty GetProperty(string key)
        {
            return filteredProperties.TryGetValue(key, out var value)
                       ? value
                       : null;
        }

        /// <inheritdoc cref="BaseAdaptorNode.GetOrCreateProperty{T}"/>
        public override IReadOnlyProperty<T> GetOrCreateProperty<T>(string name)
        {
            if (GetProperty(name) is IReadOnlyProperty<T> existing)
                return existing;
            
            var property = base.GetOrCreateProperty<T>(name);

            if (property is IReadOnlyProperty<BondPair[]> bondPairProperty
             && name.Equals(StandardFrameProperties.Bonds.Key))
            {
                var bondFiltered = new FilterBondProperty(bondPairProperty, particleFilter);
                filteredProperties[name] = bondFiltered;
                return bondFiltered as IReadOnlyProperty<T>;
            }

            if (!typeof(T).IsArray)
            {
                filteredProperties[name] = property;
                return property;
            }
            
            if (name.Contains(".particles") && property is IReadOnlyProperty<int[]> indexProp)
            {
                var filtered = new IndexFilteredProperty(indexProp, particleFilter);
                
                filteredProperties[name] = filtered;
                return filtered as IReadOnlyProperty<T>;
            }


            if (name.Contains("particle."))
            {
                var elementType = typeof(T).GetElementType();
                var filteredType = typeof(FilteredProperty<>).MakeGenericType(elementType);

                var filtered =
                    Activator.CreateInstance(filteredType, property, particleFilter) as
                        IReadOnlyProperty<T>;

                filteredProperties[name] = filtered;
                return filtered;
            }
            
            filteredProperties[name] = property;
            return property;
        }
    }
}