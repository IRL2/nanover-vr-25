using System;
using Nanover.Visualisation.Property;
using UnityEngine;

namespace Nanover.Visualisation.Node.Input
{
    /// <summary>
    /// Generic input for the visualisation system that provides some value with a
    /// given key.
    /// </summary>
    [Serializable]
    public abstract class InputNode<TProperty> : IInputNode where TProperty : Property.Property, new()
    {
        IProperty IInputNode.Input => input;

        public Type InputType => input.PropertyType;

        [SerializeField]
        private string name;

        [SerializeField]
        private TProperty input = new TProperty();

        public TProperty Input => input;

        public string Name
        {
            get => name;
            set => name = value;
        }
    }

    public interface IInputNode
    { 
        string Name { get; set; }

        IProperty Input { get; }
        
        Type InputType { get; }
    }
}