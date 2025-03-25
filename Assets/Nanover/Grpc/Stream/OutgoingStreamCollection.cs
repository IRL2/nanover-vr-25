using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Nanover.Core.Async;

namespace Nanover.Grpc.Stream
{
    /// <summary>
    /// Grouping of several outgoing streams to the same location, each with a string
    /// identifier. Instead of having to manage an
    /// <see cref="OutgoingStream{TOutgoing,TReply}" />, a user can use
    /// <see cref="StartStream" />, <see cref="QueueMessageAsync" /> and
    /// <see cref="EndStreamAsync" />
    /// to update a uniquely identified stream without maintaining a reference to it.
    /// </summary>
    public sealed class OutgoingStreamCollection<TOutgoing, TReply> : Cancellable, IAsyncClosable
    {
        public delegate OutgoingStream<TOutgoing, TReply> CreateStreamCall(CancellationToken token);

        private readonly Dictionary<string, OutgoingStream<TOutgoing, TReply>> streams
            = new Dictionary<string, OutgoingStream<TOutgoing, TReply>>();

        private readonly CreateStreamCall createStream;

        /// <summary>
        /// Create an collection which starts new streams using
        /// <paramref name="createStream" />
        /// </summary>
        public OutgoingStreamCollection(CreateStreamCall createStream,
                                        CancellationToken externalToken = default)
            : base(externalToken)
        {
            this.createStream = createStream;
        }

        /// <summary>
        /// Returns whether there is an open stream with the given id.
        /// </summary>
        public bool HasStream(string id) => streams.ContainsKey(id);

        /// <summary>
        /// Start a new outgoing stream with the provided ID and return the
        /// task representing the sending of messages over that stream.
        /// </summary>
        /// <exception cref="InvalidOperationException">The stream already exists</exception>
        public Task StartStream(string id)
        {
            if (IsCancelled)
                throw new InvalidOperationException("Stream collection has been closed");
            if (streams.ContainsKey(id))
                throw new InvalidOperationException(
                    $"Stream collection already contains stream with id {id}");

            var stream = createStream(GetCancellationToken());
            var sendingTask = stream.StartSending();

            streams[id] = stream;

            return sendingTask;
        }

        /// <summary>
        /// Send a message using the outgoing stream with the provided ID
        /// </summary>
        /// <exception cref="InvalidOperationException">The stream does not exist</exception>
        public async Task QueueMessageAsync(string id, TOutgoing message)
        {
            if (IsCancelled)
                throw new InvalidOperationException("Stream collection has been closed");
            if (!streams.ContainsKey(id))
                throw new KeyNotFoundException(
                    $"Stream collection does not contain stream with id {id}");

            await streams[id].QueueMessageAsync(message);
        }

        /// <summary>
        /// Close an outgoing stream with the provided ID
        /// </summary>
        /// <exception cref="InvalidOperationException">The stream does not exist</exception>
        public async Task EndStreamAsync(string id)
        {
            if (IsCancelled)
                throw new InvalidOperationException("Stream collection has been closed");
            if (!streams.ContainsKey(id))
                throw new KeyNotFoundException(
                    $"Stream collection does not contain stream with id {id}");

            var stream = streams[id];
            streams.Remove(id);

            await stream.CloseAsync();
        }

        /// <summary>
        /// Close all managed streams and then dispose them.
        /// </summary>
        public async Task CloseAsync()
        {
            Cancel();

            await Task.WhenAll(streams.Values.Select(streams => streams.CloseAsync()));

            foreach (var stream in streams.Values)
                stream.Dispose();

            streams.Clear();
        }

        /// <inheritdoc cref="IDisposable.Dispose" />
        public override void Dispose()
        {
            foreach (var stream in streams.Values)
                stream.Dispose();

            streams.Clear();

            base.Dispose();
        }
    }
}