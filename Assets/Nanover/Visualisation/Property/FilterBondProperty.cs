using System;
using Nanover.Core.Math;
using Nanover.Frame;

namespace Nanover.Visualisation.Property
{
    /// <summary>
    /// Filters a list of bonds by some index property.
    /// </summary>
    public class FilterBondProperty : IReadOnlyProperty<BondPair[]>
    {
        private readonly IReadOnlyProperty<BondPair[]> property;

        private readonly IReadOnlyProperty<int[]> filter;

        public FilterBondProperty(IReadOnlyProperty<BondPair[]> property,
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

        private BondPair[] filteredValues = new BondPair[0];

        public void Update()
        {
            hasValue = property.HasValue;
            hasFilter = filter.HasValue;

            if (hasFilter && hasValue)
            {
                Array.Resize(ref filteredValues, property.Value.Length);
                var j = 0;
                foreach (var bond in property.Value)
                {
                    var i1 = SearchAlgorithms.BinarySearchIndex(bond.A, filter.Value);
                    if (i1 < 0)
                        continue;

                    var i2 = SearchAlgorithms.BinarySearchIndex(bond.B, filter.Value);
                    if (i2 < 0)
                        continue;

                    filteredValues[j] = new BondPair(i1, i2);
                    j++;
                }

                Array.Resize(ref filteredValues, j);
            }

            ValueChanged?.Invoke();
        }

        public bool HasValue => hasValue;

        public event Action ValueChanged;
        public Type PropertyType => typeof(BondPair[]);
        object IReadOnlyProperty.Value => Value;

        public BondPair[] Value
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