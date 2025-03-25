using System.Threading.Tasks;

namespace Nanover.Core.Async
{
    /// <summary>
    /// Provides a mechanism to close a resource asynchronously and allow
    /// awaiting its completion.
    /// </summary>
    public interface IAsyncClosable
    {
        /// <summary>
        /// Close the object asynchronously. The object should be allowed time
        /// to clear up cleanly, finishing any tasks it is currently doing as
        /// soon as possible.
        /// </summary>
        Task CloseAsync();
    }
}