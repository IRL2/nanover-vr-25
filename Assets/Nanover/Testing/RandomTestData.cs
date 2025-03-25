using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace Nanover.Testing
{
    public static class RandomTestData
    {
        public const int RandomSeed = 40423;

        /// <summary>
        /// Given a data generator function, use the fixed random seed to repeatably
        /// generate random test data.
        /// </summary>
        public static IEnumerable<T> SeededRandom<T>(Func<T> DataGenerator, int? seed = null)
        {
            Random.InitState(seed ?? RandomSeed);

            while (true)
            {
                var data = DataGenerator();
                var state = Random.state;

                yield return data;

                // it's possible that the calling code used Random outside of this
                // function, so make sure we restore the state for repeatability.
                Random.state = state;
            }
        }
    }
}