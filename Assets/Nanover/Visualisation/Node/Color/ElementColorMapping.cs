using System;
using System.Collections.Generic;
using Nanover.Core;
using Nanover.Core.Science;
using UnityEngine;

namespace Nanover.Visualisation.Node.Color
{
    /// <summary>
    /// Stores key-value pair of atomic number and color, for defining CPK style
    /// coloring.
    /// </summary>
    [CreateAssetMenu(menuName = "Definition/Element Color Mapping")]
    public class ElementColorMapping : ScriptableObject, IMapping<Element, UnityEngine.Color>
    {
#pragma warning disable 0649
        /// <summary>
        /// Default color used when a color is not found.
        /// </summary>
        [SerializeField]
        private UnityEngine.Color defaultColor = UnityEngine.Color.white;

        /// <summary>
        /// List of assignments, acting as a dictionary so Unity can serialize.
        /// </summary>
        [SerializeField]
        private List<ElementColorAssignment> dictionary = new List<ElementColorAssignment>();
#pragma warning disable 0649

        /// <summary>
        /// Get the color for the given atomic element, returning a default color if the
        /// element is not defined.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public UnityEngine.Color Map(Element element)
        {
            var atomicNumber = (int) element;
            foreach (var item in dictionary)
                if (item.atomicNumber == atomicNumber)
                    return item.color;
            return defaultColor;
        }

        /// <summary>
        /// Key-value pair for atomic element to color mappings for serialisation in Unity
        /// </summary>
        [Serializable]
        public class ElementColorAssignment
        {
            public int atomicNumber;
            public UnityEngine.Color color;
        }
    }
}