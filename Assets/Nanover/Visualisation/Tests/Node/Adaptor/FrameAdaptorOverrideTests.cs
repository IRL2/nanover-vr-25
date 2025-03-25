using System;
using System.Collections.Generic;
using System.Linq;
using Nanover.Core.Collections;
using Nanover.Frame;
using Nanover.Frame.Event;
using Nanover.Visualisation.Node.Adaptor;
using Nanover.Visualisation.Property;
using NUnit.Framework;
using UnityEngine;

namespace Nanover.Visualisation.Tests.Node.Adaptor
{
    public class FrameAdaptorOverrideTests
    {
        private FrameSnapshot source;
        private FrameAdaptorNode adaptor;
        private Frame.Frame frame;
        private IReadOnlyProperty<Vector3[]> property;

        private static readonly Vector3[] FramePositions =
        {
            Vector3.up, Vector3.left, Vector3.down
        };
        
        private static readonly Vector3[] OverridePositions = 
        {
            Vector3.right, Vector3.zero, Vector3.back
        };

        [SetUp]
        public void Setup()
        {
            source = new FrameSnapshot();
            adaptor = new FrameAdaptorNode
            {
                FrameSource = source
            };
            frame = new Frame.Frame
            {
                ParticlePositions = FramePositions
            };
        }

        [Test]
        public void NoOverrideGetPropertyFirst()
        {
            var prop = adaptor.ParticlePositions;
            source.Update(frame, FrameChanges.All);
            CollectionAssert.AreEqual(FramePositions, prop.Value);
        }
        
        [Test]
        public void NoOverrideGetPropertyAfter()
        {
            source.Update(frame, FrameChanges.All);
            var prop = adaptor.ParticlePositions;
            CollectionAssert.AreEqual(FramePositions, prop.Value);
        }

        private static IEnumerable<IEnumerable<Action<FrameAdaptorOverrideTests>>> GetOverrideActions()
        {
            var set = new Action<FrameAdaptorOverrideTests>[]
            {
                GetProperty, UpdateFrame, AddOverride
            };
            return set.GetPermutations().Select(s => s.AsPretty(t => t.Method.Name));
        }
        
        private static IEnumerable<IEnumerable<Action<FrameAdaptorOverrideTests>>> GetOverrideThenRemoveActions()
        {
            var set = new Action<FrameAdaptorOverrideTests>[]
            {
                GetProperty, UpdateFrame, AddOverride, RemoveOverride
            };
            return set.GetPermutations()
                      .Where(s => s.IndexOf(AddOverride) < s.IndexOf(RemoveOverride))
                      .Select(s => s.AsPretty(t => t.Method.Name));
        }


        private static void GetProperty(FrameAdaptorOverrideTests test)
        {
            test.property = test.adaptor.ParticlePositions;
        }

        private static void UpdateFrame(FrameAdaptorOverrideTests test)
        {
            test.source.Update(test.frame, FrameChanges.All);
        }
        
        private static void AddOverride(FrameAdaptorOverrideTests test)
        {
            var @override = test.adaptor.AddOverrideProperty<Vector3[]>(StandardFrameProperties.ParticlePositions.Key);
            @override.Value = OverridePositions;
        }
        
        private static void RemoveOverride(FrameAdaptorOverrideTests test)
        {
            test.adaptor.RemoveOverrideProperty<Vector3[]>(StandardFrameProperties.ParticlePositions.Key);
        }

        [Test]
        public void Override([ValueSource(nameof(GetOverrideActions))] IEnumerable<Action<FrameAdaptorOverrideTests>> actions)
        {
            foreach (var action in actions)
                action(this);
            
            CollectionAssert.AreEqual(OverridePositions, property.Value);
        }
        
        [Test]
        public void OverrideThenRemove([ValueSource(nameof(GetOverrideThenRemoveActions))] IEnumerable<Action<FrameAdaptorOverrideTests>> actions)
        {
            foreach (var action in actions)
                action(this);
            
            CollectionAssert.AreEqual(FramePositions, property.Value);
        }
    }
}