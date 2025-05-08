using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Grpc.Core;
using Nanover.Core.Async;
using Channel = System.Threading.Channels.Channel;

namespace Nanover.Grpc.Stream
{
    /// <summary>
    /// Delegate for a gRPC server streaming call which takes a number of
    /// TOutgoing messages then replies with a single TIncoming message.
    /// </summary>
    public delegate AsyncClientStreamingCall<TOutgoing, TIncoming> ClientStreamingCall<TOutgoing,
        TIncoming>(
        Metadata headers = null,
        DateTime? deadline = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Wraps the outgoing request stream of a gRPC call.
    /// </summary>
    /// <remarks>
    /// Currently provides no mechanism to deal with the final response message
    /// from the server.
    /// </remarks>
    public sealed class OutgoingStream<TOutgoing, TIncoming> : Cancellable, IAsyncClosable
    {
        /// <summary>
        /// Create a stream from a gRPC call.
        /// </summary>
        public static OutgoingStream<TOutgoing, TIncoming> CreateStreamFromClientCall(
            ClientStreamingCall<TOutgoing, TIncoming> grpcCall,
            params CancellationToken[] externalTokens)
        {
            var stream = new OutgoingStream<TOutgoing, TIncoming>(externalTokens);

            stream.streamingCall = grpcCall(Metadata.Empty,
                                            null,
                                            stream.GetCancellationToken());

            return stream;
        }

        /// <summary>
        /// Is the stream currently active (A call to <see cref="StartSending" /> has
        /// occured).
        /// </summary>
        public bool IsSendingStarted => sendingTask != null;

        private AsyncClientStreamingCall<TOutgoing, TIncoming> streamingCall;
        private Task sendingTask;

        private readonly Channel<TOutgoing> messageQueue;

        private OutgoingStream(params CancellationToken[] externalTokens) : base(externalTokens)
        {
            messageQueue = Channel.CreateUnbounded<TOutgoing>(new UnboundedChannelOptions
            {
                SingleWriter = true,
                SingleReader = true
            });
        }

        /// <summary>
        /// Send a message asynchronously over the stream.
        /// </summary>
        public async Task QueueMessageAsync(TOutgoing message)
        {
            if (!IsSendingStarted)
                throw new InvalidOperationException("Stream has not started yet.");
            if (message == null)
                throw new ArgumentNullException(nameof(message), "Message cannot be null.");

            await messageQueue.Writer.WriteAsync(message);
        }

        /// <inheritdoc cref="IAsyncClosable.CloseAsync" />
        public async Task CloseAsync()
        {
            try
            {
                messageQueue.Writer.TryComplete();

                if (IsSendingStarted)
                    await sendingTask;

                Cancel();
            }
            catch (TaskCanceledException)
            {
            }

            streamingCall.Dispose();
        }

        /// <summary>
        /// Start the stream, allowing it to accept input.
        /// </summary>
        public Task StartSending()
        {
            if (IsSendingStarted)
                throw new InvalidOperationException("Streaming has already started.");

            if (IsCancelled)
                throw new InvalidOperationException("Stream has already been closed.");

            sendingTask = StartSendingLoop();

            return sendingTask;
        }

        private async Task StartSendingLoop()
        {
            var token = GetCancellationToken();

            try
            {
                while (await messageQueue.Reader.WaitToReadAsync(token))
                {
                    var message = await messageQueue.Reader.ReadAsync(token);
                    await streamingCall.RequestStream.WriteAsync(message);
                }

                await streamingCall.RequestStream.CompleteAsync();
            }
            catch (OperationCanceledException)
            {
            }
        }
    }
}