namespace Nanover.Visualisation.Property
{
    /// <summary>
    /// An property (see <see cref="IReadOnlyProperty" />) which can have its
    /// value altered.
    /// </summary>
    public interface IProperty : IReadOnlyProperty
    {
        /// <summary>
        /// Remove the value from this property.
        /// </summary>
        void UndefineValue();
        
        /// <summary>
        /// Attempt to set the value without knowing the types involved.
        /// </summary>
        void TrySetValue(object value);

        /// <summary>
        /// Is this property linked to another?
        /// </summary>
        bool HasLinkedProperty { get; }
        
        /// <summary>
        /// Attempt to set the linked property without knowing the types involved.
        /// </summary>
        void TrySetLinkedProperty(object property);
        
        /// <summary>
        /// Linked property that will override this value.
        /// </summary>
        IReadOnlyProperty LinkedProperty { get; }
    }
    
    /// <inheritdoc cref="IProperty" />
    public interface IProperty<TValue> : IReadOnlyProperty<TValue>, IProperty
    {
        /// <inheritdoc cref="IProperty.Value" />
        new TValue Value { get; set; }

        /// <inheritdoc cref="IProperty.Value" />
        new IReadOnlyProperty<TValue> LinkedProperty { set; }
    }
}