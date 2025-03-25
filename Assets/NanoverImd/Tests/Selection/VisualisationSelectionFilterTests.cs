using System.Collections.Generic;
using NanoverImd.Selection;
using NUnit.Framework;

namespace NanoverImd.Tests.Selection
{
    internal class VisualisationSelectionFilterTests
    {
        public class ThreeSelectionIndices
        {
            public int[] FirstLayerFiltered = new int[0];
            public int[] FirstLayerUnfiltered = new int[0];
            public int[] SecondLayerFiltered = new int[0];
            public int[] SecondLayerUnfiltered = new int[0];
            public int[] ThirdLayerFiltered = new int[0];
            public int[] ThirdLayerUnfiltered = new int[0];
        }

        public struct TestParameters
        {
            public IReadOnlyList<int> First;
            public IReadOnlyList<int> Second;
            public IReadOnlyList<int> Third;
            public int MaxCount;
            public ThreeSelectionIndices Expected;
        }

        public void TestThreeFilters(IReadOnlyList<int> first,
                                     IReadOnlyList<int> second,
                                     IReadOnlyList<int> third,
                                     int maxCount,
                                     ThreeSelectionIndices expected)
        {
            var result = new ThreeSelectionIndices();

            VisualisationSelection.FilterIndices(null,
                                                 third,
                                                 maxCount,
                                                 ref result.ThirdLayerFiltered,
                                                 ref result.ThirdLayerUnfiltered);

            VisualisationSelection.FilterIndices(result.ThirdLayerUnfiltered,
                                                 second,
                                                 maxCount,
                                                 ref result.SecondLayerFiltered,
                                                 ref result.SecondLayerUnfiltered);

            VisualisationSelection.FilterIndices(result.SecondLayerUnfiltered,
                                                 first,
                                                 maxCount,
                                                 ref result.FirstLayerFiltered,
                                                 ref result.FirstLayerUnfiltered);

            CollectionAssert.AreEqual(expected.ThirdLayerFiltered, result.ThirdLayerFiltered);
            CollectionAssert.AreEqual(expected.SecondLayerFiltered, result.SecondLayerFiltered);
            CollectionAssert.AreEqual(expected.FirstLayerFiltered, result.FirstLayerFiltered);
        }

        public static IEnumerable<TestParameters> GetParameters()
        {
            // All < All < All
            yield return new TestParameters
            {
                First = null,
                Second = null,
                Third = null,
                MaxCount = 8,
                Expected = new ThreeSelectionIndices
                {
                    FirstLayerFiltered = new int[0],
                    SecondLayerFiltered = new int[0],
                    ThirdLayerFiltered = null,
                }
            };
            
            // All < All < None
            yield return new TestParameters
            {
                First = null,
                Second = null,
                Third = new int[0],
                MaxCount = 8,
                Expected = new ThreeSelectionIndices
                {
                    FirstLayerFiltered = new int[0],
                    SecondLayerFiltered = null,
                    ThirdLayerFiltered = new int[0],
                }
            };
            
            // All < None < None
            yield return new TestParameters
            {
                First = null,
                Second = new int[0],
                Third = new int[0],
                MaxCount = 8,
                Expected = new ThreeSelectionIndices
                {
                    FirstLayerFiltered = null,
                    SecondLayerFiltered = new int[0],
                    ThirdLayerFiltered = new int[0],
                }
            };
            
            // All < [2, 4, 7] < [1, 3, 6] # Disjoint, non empty base
            yield return new TestParameters
            {
                First = null,
                Second = new [] {2, 4, 7},
                Third = new [] {1, 3, 6},
                MaxCount = 8,
                Expected = new ThreeSelectionIndices
                {
                    FirstLayerFiltered = new [] {0, 5},
                    SecondLayerFiltered = new [] {2, 4, 7},
                    ThirdLayerFiltered = new [] {1, 3, 6},
                }
            };
            
            // All < [0, 2, 4, 7] < [1, 3, 5, 6] # Disjoint, empty base
            yield return new TestParameters
            {
                First = null,
                Second = new [] {0, 2, 4, 7},
                Third = new [] {1, 3, 5, 6},
                MaxCount = 8,
                Expected = new ThreeSelectionIndices
                {
                    FirstLayerFiltered = new int [0],
                    SecondLayerFiltered = new [] {0, 2, 4, 7},
                    ThirdLayerFiltered = new [] {1, 3, 5, 6},
                }
            };
            
            // All < [2, 3, 4, 7] < [1, 2, 3, 5, 7] # Overlap, non empty base
            yield return new TestParameters
            {
                First = null,
                Second = new [] {2, 3, 4, 7},
                Third = new [] {1, 2, 3, 5, 7},
                MaxCount = 8,
                Expected = new ThreeSelectionIndices
                {
                    FirstLayerFiltered = new [] {0, 6},
                    SecondLayerFiltered = new [] {4},
                    ThirdLayerFiltered = new [] {1, 2, 3, 5, 7},
                }
            };
            
            // All < [0, 2, 3, 4, 6, 7] < [1, 2, 3, 5, 7] # Overlap, empty base
            yield return new TestParameters
            {
                First = null,
                Second = new [] {0, 2, 3, 4, 6, 7},
                Third = new [] {1, 2, 3, 5, 7},
                MaxCount = 8,
                Expected = new ThreeSelectionIndices
                {
                    FirstLayerFiltered = new int[0],
                    SecondLayerFiltered = new [] {0, 4, 6},
                    ThirdLayerFiltered = new [] {1, 2, 3, 5, 7},
                }
            };
            
            // All < [0, 2, 4, 6, 7] < None
            yield return new TestParameters
            {
                First = null,
                Second = new [] {0, 2, 4, 6, 7},
                Third = new int[0],
                MaxCount = 8,
                Expected = new ThreeSelectionIndices
                {
                    FirstLayerFiltered = new [] {1, 3, 5},
                    SecondLayerFiltered = new [] {0, 2, 4, 6, 7},
                    ThirdLayerFiltered = new int[0],
                }
            };
            
            // All < [0, 2, 4, 6, 7] < [0, 2, 4, 6, 7] # Duplicate
            yield return new TestParameters
            {
                First = null,
                Second = new [] {0, 2, 4, 6, 7},
                Third = new [] {0, 2, 4, 6, 7},
                MaxCount = 8,
                Expected = new ThreeSelectionIndices
                {
                    FirstLayerFiltered = new [] {1, 3, 5},
                    SecondLayerFiltered = new int[0],
                    ThirdLayerFiltered = new [] {0, 2, 4, 6, 7},
                }
            };
        }

        [Test]
        public void TestThreeFilters([ValueSource(nameof(GetParameters))] TestParameters parameters)
        {
            TestThreeFilters(parameters.First,
                             parameters.Second,
                             parameters.Third,
                             parameters.MaxCount,
                             parameters.Expected);
        }
    }
}