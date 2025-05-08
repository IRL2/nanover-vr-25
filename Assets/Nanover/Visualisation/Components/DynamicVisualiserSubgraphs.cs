using System.Collections.Generic;
using System.Linq;
using Nanover.Visualisation.Node.Adaptor;
using Nanover.Visualisation.Node.Input;
using Nanover.Visualisation.Node.Output;
using Nanover.Visualisation.Properties;
using Nanover.Visualisation.Property;
using UnityEngine;

namespace Nanover.Visualisation.Components
{
    /// <summary>
    /// Instantiates a set of visualisation subgraphs (collection of visualisation
    /// nodes with inputs and outputs), and links them together by name.
    /// </summary>
    [ExecuteAlways]
    public class DynamicVisualiserSubgraphs : MonoBehaviour
    {
        [SerializeField]
        private GameObject[] prefabs = new GameObject[0];

        private readonly List<GameObject> currentSubgraphs = new List<GameObject>();

        [SerializeField]
        private FrameAdaptorProperty frameAdaptor = new FrameAdaptorProperty();

        /// <summary>
        /// The <see cref="FrameAdaptor" /> used to provide keys that are not provided
        /// anywhere else and do not have a default value.
        /// </summary>
        public IDynamicPropertyProvider FrameAdaptor
        {
            get => frameAdaptor.Value;
            set => frameAdaptor.Value = value;
        }

        private void OnEnable()
        {
            SetupSubgraphs();
        }

        private void SetupSubgraphs()
        {
            if (currentSubgraphs.Count == 0)
                foreach (var prefab in prefabs)
                {
                    var subgraph = Instantiate(prefab, transform);
                    //subgraph.hideFlags = HideFlags.DontSave | HideFlags.NotEditable;
                    SetupAllFrameAdaptorsInSubgraph(subgraph);
                    SetupAllInputNodesInSubgraph(subgraph);
                    currentSubgraphs.Add(subgraph);
                }
        }

        private void SetupAllFrameAdaptorsInSubgraph(GameObject subgraph)
        {
            foreach (var node in subgraph.GetVisualisationNodesInChildren<ParentedAdaptorNode>())
                node.ParentAdaptor.LinkedProperty = this.frameAdaptor;
        }

        private void OnDisable()
        {
            if (Application.isEditor)
            {
                foreach (var obj in currentSubgraphs)
                    DestroyImmediate(obj);
                currentSubgraphs.Clear();
            }
        }

        /// <summary>
        /// Setup all the input nodes present in a given subgraph.
        /// </summary>
        private void SetupAllInputNodesInSubgraph(GameObject subgraph)
        {
            foreach (var node in subgraph.GetVisualisationNodesInChildren<IInputNode>())
                SetupInputNode(node);
        }

        /// <summary>
        /// For a given input node, find an appropriate output port to bind to.
        /// </summary>
        /// <remarks>
        /// The order in which an appropriate property to link to is:
        /// 1) Search each of the subgraphs for an <see cref="IOutputNode" /> of the same
        /// name.
        /// 2) Search the root object for an <see cref="IInputNode" /> with the same name.
        /// 3) If there is no default value, bind to the frame adaptor.
        /// </remarks>
        private void SetupInputNode(IInputNode input)
        {
            // Search for an output node / adaptor of a preceding subgraph

            for (var i = currentSubgraphs.Count - 1; i >= 0; i--)
            {
                var subgraph = currentSubgraphs[i];
                
                if (GetOutputNodeWithName(subgraph, input.Name)?.Output is IReadOnlyProperty output)
                {
                    input.Input.TrySetLinkedProperty(output);
                    return;
                }
            }

            // Look for an input node on the main object
            foreach (var mainInput in GetInputNodes(gameObject))
                if (input.Name == mainInput.Name)
                {
                    input.Input.TrySetLinkedProperty(mainInput.Input);
                    return;
                }

            // If there's a default value, that's okay
            if (input.Input.HasValue)
                return;

            // Look for adaptors further up the chain
            for (var i = currentSubgraphs.Count - 1; i >= 0; i--)
            {
                var subgraph = currentSubgraphs[i];
                
                if (GetAdaptor(subgraph) is IDynamicPropertyProvider adaptor)
                {
                    var outputProperty =
                        adaptor.GetOrCreateProperty(input.Name, input.InputType);
                    input.Input.TrySetLinkedProperty(outputProperty);
                    return;
                }
            }

            // Search for the key in the frame
            input.Input.TrySetLinkedProperty(
                FrameAdaptor.GetOrCreateProperty(input.Name,
                                                 input.Input.PropertyType));
        }

        /// <summary>
        /// Iterate over all the <see cref="IInputNode" />s present in a given
        /// <see cref="GameObject" />.
        /// </summary>
        private static IEnumerable<IInputNode> GetInputNodes(GameObject obj)
        {
            return obj.GetVisualisationNodes<IInputNode>();
        }

        /// <summary>
        /// Find an <see cref="IOutputNode" /> present in a given <see cref="GameObject" />
        /// with a given name.
        /// </summary>
        private static IOutputNode GetOutputNodeWithName(GameObject existing, string name)
        {
            return existing.GetVisualisationNodes<IOutputNode>().FirstOrDefault(
                c => c.Name == name);
        }

        /// <summary>
        /// Find an <see cref="IDynamicPropertyProvider" /> present in a given <see cref="GameObject" />.
        /// </summary>
        private static IDynamicPropertyProvider GetAdaptor(GameObject existing)
        {
            return existing.GetVisualisationNodes<IDynamicPropertyProvider>().LastOrDefault();
        }

        /// <summary>
        /// Set the subgraph objects that make up this renderer.
        /// </summary>
        public void SetSubgraphs(params GameObject[] subgraphs)
        {
            prefabs = subgraphs ?? new GameObject[0];
            SetupSubgraphs();
        }
    }
}