using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Nanover.Visualisation.Property;

namespace Nanover.Visualisation.Components
{
    /// <summary>
    /// Extension methods for <see cref="IPropertyProvider" />.
    /// </summary>
    public static class PropertyProviderExtensions
    {
        /// <inheritdoc cref="IDynamicPropertyProvider.GetOrCreateProperty{T}"/>
        public static IReadOnlyProperty<T> GetOrCreateProperty<T>(
            this IPropertyProvider provider,
            string key)
        {
            if (provider is IDynamicPropertyProvider dynamic)
                return dynamic.GetOrCreateProperty<T>(key);
            return provider.GetProperty<T>(key);
        }

        /// <inheritdoc cref="IDynamicPropertyProvider.GetPotentialProperties"/>
        public static IEnumerable<(string name, Type type)> GetPotentialProperties(
            this IPropertyProvider provider)
        {
            if (provider is IDynamicPropertyProvider dynamic)
                return dynamic.GetPotentialProperties();
            return provider.GetProperties().Select(e => (e.name, e.property.PropertyType));
        }

        /// <inheritdoc cref="IPropertyProvider.GetProperty"/>
        public static IReadOnlyProperty<T> GetProperty<T>(this IPropertyProvider provider,
                                                          string key)
        {
            return provider.GetProperty(key) as IReadOnlyProperty<T>;
        }

        /// <inheritdoc cref="IDynamicPropertyProvider.CanDynamicallyProvideProperty{T}"/>
        public static bool CanDynamicallyProvideProperty<T>(this IPropertyProvider provider,
                                                            string name)
        {
            if (provider is IDynamicPropertyProvider dynamic)
                return dynamic.CanDynamicallyProvideProperty<T>(name);
            return false;
        }
       
        /// <inheritdoc cref="IDynamicPropertyProvider.CanDynamicallyProvideProperty{T}"/>
        public static bool CanDynamicallyProvideProperty(this IPropertyProvider provider,
                                                            string name,
                                                            Type type)
        {
            if (provider is IDynamicPropertyProvider dynamic)
                return dynamic.CanDynamicallyProvideProperty(name, type);
            return false;
        }

        /// <inheritdoc cref="IDynamicPropertyProvider.CanDynamicallyProvideProperty{T}"/>
        public static bool CanDynamicallyProvideProperty(this IDynamicPropertyProvider provider,
                                              string name,
                                              Type type)
        {
            // Check for null method to prevent NullReferenceException during dynamic method invocation.
            var method = typeof(IPropertyProvider)
                .GetMethod(nameof(provider.CanDynamicallyProvideProperty),
                    BindingFlags.Public
                    | BindingFlags.NonPublic
                    | BindingFlags.Instance);

            if (method == null)
                return false;

            return (bool)method
                .MakeGenericMethod(type)
                .Invoke(provider, new object[]
                {
                    name
                });
        }

        /// <inheritdoc cref="IDynamicPropertyProvider.GetOrCreateProperty{T}"/>
        public static IReadOnlyProperty GetOrCreateProperty(this IDynamicPropertyProvider provider,
                                                            string name,
                                                            Type type)
        {
            return typeof(IDynamicPropertyProvider)
                   .GetMethod(nameof(provider.GetOrCreateProperty),
                              BindingFlags.Public
                            | BindingFlags.NonPublic
                            | BindingFlags.Instance)
                   .MakeGenericMethod(type)
                   .Invoke(provider, new object[]
                   {
                       name
                   }) as IReadOnlyProperty;
        }
    }
}