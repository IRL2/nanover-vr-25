using System;

namespace Nanover.Visualisation.Property
{
    public interface IFilteredProperty
    {
        IReadOnlyProperty SourceProperty { get; }

        IReadOnlyProperty<int[]> FilterProperty { get; }
    }

    /// <summary>
    /// Filters an array property by some index property.
    /// </summary>
    public class FilteredProperty<T> : IReadOnlyProperty<T[]>, IFilteredProperty
    {
        IReadOnlyProperty IFilteredProperty.SourceProperty => property;
        IReadOnlyProperty<int[]> IFilteredProperty.FilterProperty => filter;

        private readonly IReadOnlyProperty<T[]> property;

        private readonly IReadOnlyProperty<int[]> filter;

        public FilteredProperty(IReadOnlyProperty<T[]> property, IReadOnlyProperty<int[]> filter)
        {
            this.property = property;
            this.filter = filter;
            this.property.ValueChanged += Update;
            this.filter.ValueChanged += Update;
            Update();
        }

        private bool hasValue;
        private bool hasFilter;

        private T[] filteredValues = new T[0];

        public void Update()
        {
            hasValue = property.HasValue;
            hasFilter = filter.HasValue;

            if (hasFilter && hasValue)
            {
                Array.Resize(ref filteredValues, filter.Value.Length);
                var j = 0;
                foreach (var f in filter.Value)
                {
                    if (f < property.Value.Length)
                        filteredValues[j++] = property.Value[f];
                }

                Array.Resize(ref filteredValues, j);
            }

            ValueChanged?.Invoke();
        }

        public bool HasValue => hasValue;

        public event Action ValueChanged;
        public Type PropertyType => typeof(T[]);
        object IReadOnlyProperty.Value => Value;

        public T[] Value
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