using System.Collections.Generic;
using UnityEngine;

namespace Nanover.Visualisation.Components
{
    /// <summary>
    /// Extension methods related to <see cref="VisualisationComponent"/>.
    /// </summary>
    public static class VisualisationComponentExtensions
    {
        /// <summary>
        /// Get all visualisation nodes of a given type that are in children of the game object.
        /// </summary>
        public static IEnumerable<TNode> GetVisualisationNodesInChildren<TNode>(this GameObject go)
        {
            foreach(var comp in go.GetComponentsInChildren<VisualisationComponent>())
                if (comp.GetWrappedVisualisationNode() is TNode node)
                    yield return node;
        }
        
        /// <summary>
        /// Get all visualisation nodes of a given type that are in this game object.
        /// </summary>
        public static IEnumerable<TNode> GetVisualisationNodes<TNode>(this GameObject go)
        {
            foreach(var comp in go.GetComponents<VisualisationComponent>())
                if (comp.GetWrappedVisualisationNode() is TNode node)
                    yield return node;
        }
        
        /// <summary>
        /// Get the first visualisation nodes of a given type that are in this game object.
        /// </summary>
        public static TNode GetVisualisationNode<TNode>(this GameObject go) where TNode : class
        {
            foreach(var comp in go.GetComponents<VisualisationComponent>())
                if (comp.GetWrappedVisualisationNode() is TNode node)
                    return node;
            return null;
        }
    }
}