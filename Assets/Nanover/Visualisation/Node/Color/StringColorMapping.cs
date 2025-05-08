using System;
using System.Collections.Generic;
using UnityEngine;

namespace Nanover.Visualisation.Node.Color
{
    /// <summary>
    /// Stores key-value pair of atomic number and color, for defining CPK style
    /// coloring.
    /// </summary>
    [CreateAssetMenu(menuName = "Definition/String-Color Mapping")]
    public class StringColorMapping : ScriptableObject
    {
#pragma warning disable 0649
        /// <summary>
        /// Default color used when a color is not found.
        /// </summary>
        [SerializeField]
        private UnityEngine.Color defaultColor;

        /// <summary>
        /// List of assignments, acting as a dictionary so Unity can serialize.
        /// </summary>
        [SerializeField]
        private List<StringColorAssignment> dictionary;
#pragma warning restore 0649

        /// <summary>
        /// Get the color for the given atomic element, returning a default color if the
        /// element is not defined.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public UnityEngine.Color GetColor(string name)
        {
            foreach (var item in dictionary)
                if (item.value.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                    return item.color;
            return defaultColor;
        }

        /// <summary>
        /// Key-value pair for atomic element to color mappings for serialisation in Unity
        /// </summary>
        [Serializable]
        public class StringColorAssignment
        {
            public string value;
            public UnityEngine.Color color;
        }
    }
}