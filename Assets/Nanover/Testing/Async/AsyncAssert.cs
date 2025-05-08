using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using UnityEditor;
using UnityEngine;

namespace Nanover.Testing.Async
{
    /// <summary>
    /// Delegate used by tests that execute code asynchronously
    /// </summary>
    public delegate Task AsyncTestCallback();

    public static class AsyncAssert
    {
        /// <summary>
        /// Verifies that a delegate throws a particular exception when called.
        /// </summary>
        /// <param name="expression">A constraint to be satisfied by the exception</param>
        /// <param name="callback">A AsyncTestCallback delegate</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        /// <remarks>
        /// Copied from Assert.Throws in NUnit, with changes to await async execution
        /// </remarks>
        public static async Task<Exception> ThrowsAsync(
            IResolveConstraint expression,
            AsyncTestCallback callback,
            string message,
            params object[] args)
        {
            Exception actual = null;
            try
            {
                await callback();
            }
            catch (Exception ex)
            {
                actual = ex;
            }

            Assert.That(actual, expression, message, args);
            return actual;
        }

        /// <summary>
        /// Verifies that a delegate throws a particular exception when called.
        /// </summary>
        /// <param name="expression">A constraint to be satisfied by the exception</param>
        /// <param name="callback">A AsyncTestCallback delegate</param>
        /// <remarks>
        /// Copied from Assert.Throws in NUnit, with changes to await async execution
        /// </remarks>
        public static async Task<Exception> ThrowsAsync(IResolveConstraint expression,
                                                        AsyncTestCallback callback)
        {
            return await ThrowsAsync(expression, callback, string.Empty, null);
        }

        /// <summary>
        /// Verifies that a delegate throws a particular exception when called.
        /// </summary>
        /// <param name="expectedExceptionType">The exception Type expected</param>
        /// <param name="callback">A AsyncTestCallback delegate</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        /// <remarks>
        /// Copied from Assert.Throws in NUnit, with changes to await async execution
        /// </remarks>
        public static async Task<Exception> ThrowsAsync(
            Type expectedExceptionType,
            AsyncTestCallback callback,
            string message,
            params object[] args)
        {
            return await ThrowsAsync(
                       new ExceptionTypeConstraint(expectedExceptionType),
                       callback, message, args);
        }

        /// <summary>
        /// Verifies that a delegate throws a particular exception when called.
        /// </summary>
        /// <param name="expectedExceptionType">The exception Type expected</param>
        /// <param name="callback">A TestDelegate</param>
        /// <remarks>
        /// Copied from Assert.Throws in NUnit, with changes to await async execution
        /// </remarks>
        public static async Task<Exception> ThrowsAsync(Type expectedExceptionType,
                                                        AsyncTestCallback callback)
        {
            return await ThrowsAsync(
                       new ExceptionTypeConstraint(expectedExceptionType),
                       callback, string.Empty,
                       null);
        }

        /// <summary>
        /// Verifies that a delegate throws a particular exception when called.
        /// </summary>
        /// <typeparam name="TActual">Type of the expected exception</typeparam>
        /// <param name="callback">A AsyncTestCallback delegate</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        /// <remarks>
        /// Copied from Assert.Throws in NUnit, with changes to await async execution
        /// </remarks>
        public static async Task<TActual> ThrowsAsync<TActual>(AsyncTestCallback callback,
                                                               string message,
                                                               params object[] args)
            where TActual : Exception
        {
            return (TActual) await ThrowsAsync(typeof(TActual), callback, message, args);
        }

        /// <summary>
        /// Verifies that a delegate throws a particular exception when called.
        /// </summary>
        /// <typeparam name="TActual">Type of the expected exception</typeparam>
        /// <param name="callback">A AsyncTestDelegate delegate</param>
        /// <remarks>
        /// Copied from Assert.Throws in NUnit, with changes to await async execution
        /// </remarks>
        public static async Task<TActual> ThrowsAsync<TActual>(AsyncTestCallback callback)
            where TActual : Exception
        {
            return await ThrowsAsync<TActual>(callback, string.Empty, null);
        }

        /// <summary>
        /// Run the provided test at 100 millisecond intervals, ignoring assertion
        /// exceptions. This allows the test to return early if it passes, and only fail
        /// after a certain timespan has passed.
        /// </summary>
        public static async Task PassesWithinTimeout(Action test,
                                                     int timeout = 1000,
                                                     int interval = 100,
                                                     Task backgroundTask = null)
        {
            var time = 0;
            while (time < timeout)
            {
                await (backgroundTask != null
                    ? RunTasksForDuration(interval, backgroundTask)
                    : Task.Delay(interval));
                
                try
                {
                    test();
                    return;
                }
                catch (AssertionException)
                {
                    // Ignore assertions
                }

                time += interval;
            }

            test();
        }

        /// <summary>
        /// Await a task, throwing an exception if the task does not complete within a timeout period.
        /// </summary>
        public static async Task CompletesWithinTimeout(Task task, int timeout = 1000, Task backgroundTask = null)
        {
            var delay = backgroundTask != null
                            ? RunTasksForDuration(timeout, backgroundTask)
                            : Task.Delay(timeout);
            if (await Task.WhenAny(task, delay) != task)
                throw new AssertionException($"Task did not complete within {timeout} milliseconds.");
        }

        /// <summary>
        /// Run a task that will last for a given duration, executing the provided tasks as well.
        /// </summary>
        /// <remarks>
        /// Allows the simulation of waiting for a given time while one or more background tasks are running.
        /// </remarks>
        public static async Task RunTasksForDuration(int duration = 500, params Task[] tasks)
        {
            await Task.WhenAny(Task.WhenAll(Task.WhenAll(tasks), Task.Delay(duration)), Task.Delay(duration));
        }

        public class EventListener
        {
            private readonly Action callback;
            
            internal EventListener(Action callback)
            {
                this.callback = callback;
            }

            public void AssertCalledAtLeastOnce()
            {
                callback.Received().Invoke();
            }
            
            public void AssertNotCalled()
            {
                callback.DidNotReceive().Invoke();
            }
        }

        public static EventListener ListenToEvent(Action<Action> subscribe)
        {
            var callback = Substitute.For<Action>();
            subscribe(callback);
            return new EventListener(callback);
        }

    }
}