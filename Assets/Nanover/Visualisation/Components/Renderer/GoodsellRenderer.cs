using Nanover.Visualisation.Components.Renderer;
using Nanover.Visualisation.Node.Renderer;
using UnityEngine;

namespace Nanover.Visualisation.Components.Renderer
{
    /// <inheritdoc cref="Nanover.Visualisation.Node.Renderer" />
    public class GoodsellRenderer : VisualisationComponentRenderer<GoodsellRendererNode>
    {
        protected override void OnEnable()
        {
            base.OnEnable();
            node.Transform = transform;
            node.Dispose();
            node.Setup();
        }

        protected override void Render(Camera camera)
        {
            node.Render(camera);
        }

        protected override void UpdateInEditor()
        {
            base.UpdateInEditor();
            node.Dispose();
        }
    }
}