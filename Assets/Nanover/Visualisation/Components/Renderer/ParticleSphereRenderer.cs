using Nanover.Visualisation.Components.Renderer;
using Nanover.Visualisation.Node.Renderer;
using UnityEngine;

namespace Nanover.Visualisation.Components.Visualiser
{
    /// <inheritdoc cref="ParticleSphereRendererNode" />
    public class ParticleSphereRenderer :
        VisualisationComponentRenderer<ParticleSphereRendererNode>
    {
        private void Start()
        {
            node.Transform = transform;
        }

        protected override void OnDestroy()
        {
            node.Dispose();
        }

        protected override void Render(Camera camera)
        {
            node.Render(camera);
        }

        protected override void UpdateInEditor()
        {
            base.UpdateInEditor();
            node.ResetBuffers();
        }
    }
}