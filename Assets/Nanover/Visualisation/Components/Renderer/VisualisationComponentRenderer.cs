using UnityEngine;

namespace Nanover.Visualisation.Components.Renderer
{
    public abstract class VisualisationComponentRenderer<T> : VisualisationComponent<T>
        where T : new()
    {
        protected override void OnEnable()
        {
            base.OnEnable();
            Camera.onPreCull += Render;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            Camera.onPreCull -= Render;
        }

        protected abstract void Render(Camera camera);

        private void Update()
        {
            if (!Application.isPlaying) UpdateInEditor();
        }

        protected virtual void UpdateInEditor()
        {
        }
    }
}