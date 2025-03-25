using System;
using Nanover.Core.Math;

namespace Nanover.Visualisation.Property
{
    /// <summary>
    /// Take an array that represents indices in some data set, and set of indices in
    /// the same data set as a filter, and return the set of indices in the filtered
    /// data.
    /// </summary>
    public class IndexFilteredProperty : IReadOnlyProperty<int[]>, IFilteredProperty
    {
        IReadOnlyProperty IFilteredProperty.SourceProperty => property;
        IReadOnlyProperty<int[]> IFilteredProperty.FilterProperty => filter;

        private readonly IReadOnlyProperty<int[]> property;

        private readonly IReadOnlyProperty<int[]> filter;

        public IndexFilteredProperty(IReadOnlyProperty<int[]> property,
                                     IReadOnlyProperty<int[]> filter)
        {
            this.property = property;
            this.filter = filter;
            this.property.ValueChanged += Update;
            this.filter.ValueChanged += Update;
            Update();
        }

        private bool hasValue;
        private bool hasFilter;

        private int[] filteredValues = new int[0];

        public void Update()
        {
            hasValue = property.HasValue;
            hasFilter = filter.HasValue;

            if (hasFilter && hasValue)
            {
                Array.Resize(ref filteredValues, property.Value.Length);
                var j = 0;
                foreach (var f in property.Value)
                {
                    var indexInFilter = SearchAlgorithms.BinarySearchIndex(f, filter.Value);
                    if (indexInFilter >= 0)
                        filteredValues[j++] = indexInFilter;
                }

                Array.Resize(ref filteredValues, j);
            }

            ValueChanged?.Invoke();
        }

        public bool HasValue => hasValue;

        public event Action ValueChanged;

        public Type PropertyType => typeof(int[]);

        object IReadOnlyProperty.Value => Value;

        public int[] Value
        {
            get
            {
                if (!hasValue)
                    throw new InvalidOperationException(
                        "Tried accessing value of property when it is not defined");
                if (!hasFilter)
                    return property.Value;
                return filteredValues;
            }
        }
    }
}