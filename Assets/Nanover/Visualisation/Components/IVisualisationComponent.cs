namespace Nanover.Visualisation.Components
{
    /// <summary>
    /// An object which wraps a node for use in visualisation.
    /// </summary>
    public interface IVisualisationComponent<out TNode>
    {
        /// <summary>
        /// The node wrapped by this component.
        /// </summary>
        TNode Node { get; }   
    }
}