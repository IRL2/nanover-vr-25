using Nanover.Core.Science;
using Nanover.Visualisation.Node.Color;
using Nanover.Visualisation.Node.Scale;
using Nanover.Visualisation.Properties.Collections;
using Nanover.Visualisation.Property;
using NUnit.Framework;
using UnityEngine;

namespace Nanover.Visualisation.Tests.Node.Color
{
    public class ElementPaletteColorTests
    {
        private Element[] ValidElements = {Element.Carbon, Element.Hydrogen};
        
        [Test]
        public void NullPalette()
        {
            var node = new ElementColorMappingNode();

            node.Elements.Value = ValidElements;
            node.Mapping.Value = null;

            node.Refresh();
            
            Assert.IsFalse(node.Colors.HasNonNullValue());
        }
        
        [Test]
        public void NullElements()
        {
            var node = new ElementColorMappingNode();

            node.Mapping.Value = ScriptableObject.CreateInstance<ElementColorMapping>();
            node.Elements.UndefineValue();

            node.Refresh();
            
            Assert.IsFalse(node.Colors.HasNonNullValue());
        }
        
        [Test]
        public void EmptyElements()
        {
            var node = new ElementColorMappingNode();

            node.Elements.Value = new Element[0];
            node.Mapping.Value = ScriptableObject.CreateInstance<ElementColorMapping>();

            node.Refresh();
            
            Assert.IsFalse(node.Colors.HasNonNullValue());
        }
        
        [Test]
        public void ValidInput()
        {
            var node = new ElementColorMappingNode();

            node.Elements.Value = ValidElements;
            node.Mapping.Value = ScriptableObject.CreateInstance<ElementColorMapping>();

            node.Refresh();
            
            Assert.IsTrue(node.Colors.HasNonNullValue());
        }
        
        [Test]
        public void OnlyRefreshOnce()
        {
            var node = new ElementColorMappingNode();
            var linked = new ColorArrayProperty()
            {
                LinkedProperty = node.Colors, 
                IsDirty = false
            };
            Assert.IsFalse(linked.IsDirty);

            node.Elements.Value = ValidElements;
            node.Mapping.Value = ScriptableObject.CreateInstance<ElementColorMapping>();

            node.Refresh();

            Assert.IsTrue(linked.IsDirty);
            linked.IsDirty = false;
            
            node.Refresh();
            
            Assert.IsFalse(linked.IsDirty);
        }
    }
}