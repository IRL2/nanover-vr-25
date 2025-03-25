using System.Collections.Generic;
using System.Linq;
using Nanover.Core.Collections;
using Nanover.Frame.Event;
using Nanover.Visualisation.Node.Adaptor;
using Nanover.Visualisation.Property;
using NUnit.Framework;
using UnityEngine;

namespace Nanover.Visualisation.Tests.Node.Adaptor
{
    public class FilteredAdaptorDynamicPropertyTests
    {
        [Test]
        public void InitialNoProperties()
        {
            SetAdaptorsFrame(this);
            GetProperty(this);
            CollectionAssert.AreEqual(positions, property.Value);
        }

        private Frame.Frame emptyFrame;
        private Frame.Frame frameWithPositions;
        private FrameAdaptorNode frameAdaptor;
        private ParticleFilteredAdaptorNode filterAdaptor;
        private int[] filter;
        private IReadOnlyProperty<Vector3[]> property;
        private Vector3[] filteredPositions;
        private Vector3[] positions;

        [SetUp]
        public void Setup()
        {
            positions = new[]
            {
                Vector3.zero, Vector3.right, Vector3.up, Vector3.forward
            };

            filteredPositions = new[]
            {
                Vector3.right, Vector3.forward
            };

            emptyFrame = new Frame.Frame();

            frameWithPositions = new Frame.Frame
            {
                ParticlePositions = positions
            };

            frameAdaptor = new FrameAdaptorNode();
            filterAdaptor = new ParticleFilteredAdaptorNode();
            filterAdaptor.ParentAdaptor.Value = frameAdaptor;

            filter = new[]
            {
                1, 3
            };
        }

        private static void SetAdaptorsFilter(FilteredAdaptorDynamicPropertyTests test)
        {
            test.filterAdaptor.ParticleFilter.Value = test.filter;
        }

        private static void SetAdaptorsFrame(FilteredAdaptorDynamicPropertyTests test)
        {
            var frameSource = new FrameSnapshot();
            frameSource.Update(test.frameWithPositions, FrameChanges.All);
            test.frameAdaptor.FrameSource = frameSource;
        }

        private static void GetProperty(FilteredAdaptorDynamicPropertyTests test)
        {
            test.property = test.filterAdaptor.ParticlePositions;
        }

        public delegate void SetupFunction(FilteredAdaptorDynamicPropertyTests test);

        private static IEnumerable<IEnumerable<SetupFunction>> GetActionsForFilteredProperty()
        {
            var actions = new SetupFunction[]
            {
                SetAdaptorsFilter, SetAdaptorsFrame, GetProperty
            };
            return actions.GetPermutations()
                          .Select(e => e.AsPretty(t => t.Method.Name));
        }

        [Test]
        public void Filter([ValueSource(nameof(GetActionsForFilteredProperty))]
                           IEnumerable<SetupFunction> actions)
        {
            foreach (var setup in actions)
                setup(this);

            CollectionAssert.AreEqual(filteredPositions, property.Value);
        }
    }
}