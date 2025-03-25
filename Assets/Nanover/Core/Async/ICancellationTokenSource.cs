using System.Threading;

namespace Nanover.Core.Async
{
    /// <summary>
    /// An object which provides a <see cref="CancellationToken" /> using
    /// <see cref="GetCancellationToken" />, which allows related things to be
    /// cancelled when this object is.
    /// </summary>
    public interface ICancellationTokenSource
    {
        /// <summary>
        /// Get a unique token which can be passed to most asynchronous operations.
        /// </summary>
        CancellationToken GetCancellationToken();
    }
}