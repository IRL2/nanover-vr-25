using System;
using System.Collections.Generic;

namespace Nanover.Core
{
    /// <summary>
    /// Defines an interface which provides a mapping between two types. It is
    /// essentially a <see cref="Converter{TInput,TOutput}" /> as an interface.
    /// </summary>
    public interface IMapping<in TFrom, out TTo>
    {
        /// <summary>
        /// Map an input value to an output value.
        /// </summary>
        TTo Map(TFrom from);
    }

    internal class DictionaryAsMapping<TFrom, TTo> : IMapping<TFrom, TTo>
    {
        private IReadOnlyDictionary<TFrom, TTo> dictionary;

        private TTo defaultValue;

        internal DictionaryAsMapping(IReadOnlyDictionary<TFrom, TTo> dict,
                                     TTo defaultValue = default)
        {
            dictionary = dict;
            this.defaultValue = defaultValue;
        }

        /// <inheritdoc cref="IMapping{TFrom,TTo}.Map"/>
        public TTo Map(TFrom from)
        {
            return dictionary.TryGetValue(from, out var value) ? value : defaultValue;
        }
    }

    /// <summary>
    /// Extensions methods for <see cref="IMapping{TFrom,TTo}"/>.
    /// </summary>
    public static class MappingExtensions
    {
        /// <summary>
        /// Convert a dictionary into an <see cref="IMapping{TFrom,TTo}"/>.
        /// </summary>
        public static IMapping<TFrom, TTo> AsMapping<TFrom, TTo>(
            this IReadOnlyDictionary<TFrom, TTo> dict,
            TTo defaultValue = default)
        {
            return new DictionaryAsMapping<TFrom, TTo>(dict, defaultValue);
        }
    }
}