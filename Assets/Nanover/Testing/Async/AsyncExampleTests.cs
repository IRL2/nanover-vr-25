using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Nanover.Testing.Async
{
    /// <summary>
    /// Example of asynchronous tests
    /// </summary>
    /// <remarks>
    /// Each class that would like to run asynchronous unit tests will require two
    /// methods. The
    /// <see cref="AsyncExampleTests.GetTests" /> static method is needed to feed the
    /// tests into
    /// a single unit test (called <see cref="TestAsync" />) that iterates over them
    /// and executes
    /// each one. Both methods must be in each class, as inheritance would not work
    /// with static
    /// methods.
    /// </remarks>
    public class AsyncExampleTests
    {
        private static IEnumerable<AsyncUnitTests.AsyncTestInfo> GetTests()
        {
            return AsyncUnitTests.FindAsyncTestsInClass<AsyncExampleTests>();
        }

        [Test]
        public void TestAsync([ValueSource(nameof(GetTests))] AsyncUnitTests.AsyncTestInfo test)
        {
            AsyncUnitTests.RunAsyncTest(this, test);
        }

        [AsyncTest]
        public async Task DoesDelayOccur()
        {
            var startTime = DateTime.Now;

            await Task.Delay(2000);

            var endTime = DateTime.Now;

            var duration = (endTime - startTime).TotalMilliseconds;
            Assert.AreEqual(2000, duration, 200);
        }

        private static async Task DelayThenException()
        {
            await Task.Delay(1000);
            throw new Exception();
        }

        [AsyncTest]
        public async Task IsExceptionThrown()
        {
            await AsyncAssert.ThrowsAsync<Exception>(async () => await DelayThenException());
        }
    }
}