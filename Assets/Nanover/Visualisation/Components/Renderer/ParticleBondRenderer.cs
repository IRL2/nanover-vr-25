using Nanover.Visualisation.Node.Renderer;
using UnityEngine;

namespace Nanover.Visualisation.Components.Renderer
{
    /// <inheritdoc cref="ParticleBondRendererNode" />
    public class ParticleBondRenderer : VisualisationComponentRenderer<ParticleBondRendererNode>
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