using UnityEngine;

namespace Nanover.Visualisation.Components.Renderer
{
    public class ExtendedSplineRenderer : VisualisationComponentRenderer<Node.Renderer.ExtendedSplineRendererNode>
    {
        protected override void OnEnable()
        {
            base.OnEnable();
            node.Transform = transform;
        }

        protected override void Render(Camera camera)
        {
            if (camera.name == "Preview Scene Camera")
                return;
            node.Render(camera);
        }
    }
}