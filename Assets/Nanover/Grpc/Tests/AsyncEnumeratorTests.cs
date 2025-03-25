using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Nanover.Grpc.Stream;
using Nanover.Testing.Async;
using NSubstitute;
using NUnit.Framework;

namespace Nanover.Grpc.Tests
{
    internal class AsyncEnumeratorTests
    {
        private static IEnumerable<AsyncUnitTests.AsyncTestInfo> GetTests()
        {
            return AsyncUnitTests.FindAsyncTestsInClass<AsyncEnumeratorTests>();
        }

        [Test]
        public void TestAsync([ValueSource(nameof(GetTests))] AsyncUnitTests.AsyncTestInfo test)
        {
            AsyncUnitTests.RunAsyncTest(this, test);
        }

        /// <summary>
        /// IAsyncEnumerator which throws error on first call to <see cref="MoveNext" />
        /// </summary>
        private class ExceptionEnumerator : IAsyncStreamReader<int>
        {
            public Task<bool> MoveNext(CancellationToken cancellationToken)
            {
                throw new Exception();
            }

            public int Current { get; }
        }

        /// <summary>
        /// IAsyncEnumerator which iterates over a range, with a given delay
        /// </summary>
        private class CountEnumerator : IAsyncStreamReader<int>
        {
            private int Count { get; set; }
            private int Delay { get; set; }

            public CountEnumerator(int count, int delay = 100)
            {
                Count = count;
                Delay = delay;
            }

            public async Task<bool> MoveNext(CancellationToken cancellationToken)
            {
                if (Current >= 0)
                    await Task.Delay(Delay, cancellationToken);
                Current++;
                return Current < Count;
            }

            public int Current { get; private set; } = -1;
        }

        [AsyncTest]
        public async Task CountEnumerator_ZeroCount()
        {
            var enumerator = new CountEnumerator(0);

            var hasNext = await enumerator.MoveNext(CancellationToken.None);

            Assert.IsFalse(hasNext);
        }

        [AsyncTest]
        public async Task CountEnumerator_SingleCount()
        {
            var enumerator = new CountEnumerator(1);

            var hasNext = await enumerator.MoveNext(CancellationToken.None);

            Assert.IsTrue(hasNext);

            Assert.AreEqual(enumerator.Current, 0);

            hasNext = await enumerator.MoveNext(CancellationToken.None);

            Assert.IsFalse(hasNext);
        }

        [AsyncTest]
        public async Task CountEnumerator_MultipleCount()
        {
            var enumerator = new CountEnumerator(3);

            var hasNext = await enumerator.MoveNext(CancellationToken.None);

            Assert.IsTrue(hasNext);
            Assert.AreEqual(enumerator.Current, 0);

            hasNext = await enumerator.MoveNext(CancellationToken.None);

            Assert.IsTrue(hasNext);
            Assert.AreEqual(enumerator.Current, 1);

            hasNext = await enumerator.MoveNext(CancellationToken.None);

            Assert.IsTrue(hasNext);
            Assert.AreEqual(enumerator.Current, 2);

            hasNext = await enumerator.MoveNext(CancellationToken.None);

            Assert.IsFalse(hasNext);
        }

        [AsyncTest]
        public async Task AsyncEnumerable_ForEach()
        {
            var enumerator = new CountEnumerator(3, 20);

            var callback = Substitute.For<Action<int>>();

            await enumerator.ForEachAsync(callback, CancellationToken.None);

            callback.Received(3).Invoke(Arg.Any<int>());
        }

        [AsyncTest]
        public async Task AsyncEnumerable_ForEach_Exception()
        {
            var enumerator = new ExceptionEnumerator();

            var callback = Substitute.For<Action<int>>();

            await AsyncAssert.ThrowsAsync<Exception>(
                async () => await enumerator.ForEachAsync(callback, CancellationToken.None));
        }

        [AsyncTest]
        public async Task AsyncEnumerable_ForEach_Cancellation()
        {
            var enumerator = new CountEnumerator(10, 100);

            var callback = Substitute.For<Action<int>>();

            var tokenSource = new CancellationTokenSource();

            async Task DelayThenCancel()
            {
                await Task.Delay(250);
                tokenSource.Cancel();
            }

            try
            {
                await Task.WhenAll(
                    enumerator.ForEachAsync(callback, tokenSource.Token),
                    DelayThenCancel()
                );
            }
            catch (TaskCanceledException)
            {
            }

            callback.Received(3).Invoke(Arg.Any<int>());
        }
    }
}