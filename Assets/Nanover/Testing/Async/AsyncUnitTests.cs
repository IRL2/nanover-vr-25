using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Nanover.Testing.Async
{
    /// <summary>
    /// Marks a method as an asynchronous unit test. To be invoked by NUnit,
    /// the <see cref="MethodInfo" /> describing this method should be passed to
    /// <see cref="AsyncUnitTests.RunAsyncTest" />
    /// </summary>
    /// <remarks>
    /// The method should have the signature `public async Task`
    /// </remarks>
    public class AsyncTest : Attribute
    {
    }

    /// <summary>
    /// Marks a method as an asynchronous set up. To be invoked by NUnit,
    /// a standard NUnit [SetUp] method should call
    /// <see cref="AsyncUnitTests.RunAsyncSetUp" />
    /// </summary>
    /// <remarks>
    /// The method should have the signature `public async Task`
    /// </remarks>
    public class AsyncSetUp : Attribute
    {
    }

    /// <summary>
    /// Marks a method as an asynchronous tear down. To be invoked by NUnit,
    /// a standard NUnit [TearDown] method should call
    /// <see cref="AsyncUnitTests.RunAsyncTearDown" />
    /// </summary>
    /// <remarks>
    /// The method should have the signature `public async Task`
    /// </remarks>
    public class AsyncTearDown : Attribute
    {
    }

    /// <summary>
    /// Utility methods for finding and running asynchronous unit tests using NUnit
    /// </summary>
    public static class AsyncUnitTests
    {
        /// <summary>
        /// Wrapper for a test definition, containing the method that represents the test
        /// </summary>
        public struct AsyncTestInfo
        {
            public MethodInfo TestMethod;

            public override string ToString()
            {
                return TestMethod.Name;
            }
        }

        /// <summary>
        /// Use reflection to find all [AsyncTest]'s in a class
        /// </summary>
        public static IEnumerable<AsyncTestInfo> FindAsyncTestsInClass<T>()
        {
            var methods = typeof(T).GetMethods(BindingFlags.Instance | BindingFlags.Public);
            foreach (var method in methods)
            {
                if (method.ReturnType != typeof(Task))
                    continue;
                var attr = method.GetCustomAttribute<AsyncTest>();
                if (attr == null)
                    continue;
                yield return new AsyncTestInfo
                {
                    TestMethod = method
                };
            }
        }

        /// <summary>
        /// Execute an asynchronous test by calling the method described in
        /// <paramref name="method" /> on the object <paramref name="instance" />
        /// </summary>
        public static void RunAsyncTest(object instance, AsyncTestInfo method)
        {
            // Invoke the test method on testObject, with no parameters
            Task UnitTest() => method.TestMethod.Invoke(instance, new object[0]) as Task;

            var task = Task.Run(UnitTest);

            task.GetAwaiter().GetResult();
        }

        /// <summary>
        /// Execute an asynchronous setup for unit tests
        /// </summary>
        public static void RunAsyncSetUp(object instance)
        {
            var setUp = FindAsyncMethodWithAttribute<AsyncSetUp>(instance);

            if (setUp == null)
                return;

            var task = Task.Run(setUp);

            task.GetAwaiter().GetResult();
        }

        /// <summary>
        /// Execute an asynchronous teardown for unit tests
        /// </summary>
        public static void RunAsyncTearDown(object instance)
        {
            var setUp = FindAsyncMethodWithAttribute<AsyncTearDown>(instance);

            if (setUp == null)
                return;

            var task = Task.Run(setUp);

            task.GetAwaiter().GetResult();
        }

        private static Func<Task> FindAsyncMethodWithAttribute<TAttribute>(object instance)
            where TAttribute : Attribute
        {
            var methods =
                instance.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public);
            foreach (var method in methods)
            {
                if (method.ReturnType != typeof(Task))
                    continue;
                var attr = method.GetCustomAttribute<TAttribute>();
                if (attr == null)
                    continue;
                return () => method.Invoke(instance, new object[0]) as Task;
            }

            return null;
        }
    }
}