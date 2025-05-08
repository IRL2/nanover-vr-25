using System;
using Nanover.Visualisation.Property;
using UnityEngine;

namespace Nanover.Visualisation.Node.Output
{
    /// <summary>
    /// Generic output for the visualisation system that provides some value with a
    /// given key.
    /// </summary>
    [Serializable]
    public abstract class OutputNode<TProperty> : IOutputNode where TProperty : Property.Property, new()
    {
        IReadOnlyProperty IOutputNode.Output => output;
        
        [SerializeField]
        private string name;

        [SerializeField]
        private TProperty output = new TProperty();

        public string Name => name;
    }
    
    public interface IOutputNode
    { 
        string Name { get; }
        
        IReadOnlyProperty Output { get; }
    }
}