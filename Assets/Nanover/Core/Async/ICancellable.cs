using System.Threading;

namespace Nanover.Core.Async
{
    /// <summary>
    /// An object which may be cancelled, providing access to its cancellation
    /// state (using <see cref="IsCancelled" />), the ability to cancel it (using
    /// <see cref="Cancel" />) and the ability to get a
    /// <see cref="CancellationToken" /> using
    /// <see cref="ICancellationTokenSource.GetCancellationToken" />, which allows
    /// related things to be cancelled when this object is.
    /// </summary>
    public interface ICancellable : ICancellationTokenSource
    {
        /// <summary>
        /// Check if this object has been cancelled.
        /// </summary>
        bool IsCancelled { get; }

        /// <summary>
        /// Cancel this object as soon as possible, ending any tasks prematurely.
        /// </summary>
        void Cancel();
    }
}