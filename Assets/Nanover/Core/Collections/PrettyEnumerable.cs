using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Nanover.Core.Collections
{
    /// <summary>
    /// Wrapper around an <see cref="IEnumerable" /> that overrides the
    /// <see cref="ToString()" /> method to provide a list of its contents, optionally
    /// with a custom function to generate the names of each element.
    /// </summary>
    /// <remarks>
    /// Useful for Unit Tests which accept a generated <see cref="IEnumerable" />
    /// parameter, so that the test log is more clear on the contents of the parameter.
    /// </remarks>
    internal class PrettyEnumerable<T> : IEnumerable<T>
    {
        private readonly IEnumerable<T> enumerable;
        private readonly Func<T, string> namer;

        /// <summary>
        /// Create a wrapper around an <see cref="IEnumerable{T}" />, using the items built
        /// in <see cref="ToString" /> function.
        /// </summary>
        public PrettyEnumerable(IEnumerable<T> enumerable) : this(
            enumerable, t => t.ToString())
        {
        }

        /// <summary>
        /// Create a wrapper around an <see cref="IEnumerable{T}" />, with a function which
        /// generates the display name for each element.
        /// </summary>
        public PrettyEnumerable(IEnumerable<T> enumerable, Func<T, string> namer)
        {
            this.enumerable = enumerable;
            this.namer = namer;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return enumerable.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            return string.Join(", ", enumerable.Select(namer));
        }
    }
}