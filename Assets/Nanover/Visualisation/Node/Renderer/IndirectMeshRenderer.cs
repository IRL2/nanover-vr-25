using System;
using Nanover.Visualisation;
using Nanover.Visualisation.Utility;
using UnityEngine;
using UnityEngine.Rendering;

namespace Nanover.Visualisation.Node.Renderer
{
    public abstract class IndirectMeshRenderer : IDisposable
    {
        public void AppendToCommandBuffer(CommandBuffer buffer)
        {
            UpdateRenderer();
            DrawCommand.AppendToCommandBuffer(buffer);
        }
        
        /// <summary>
        /// Render the provided bonds
        /// </summary>
        public void Render(Camera camera = null)
        {
            if (UpdateRenderer())
            {
                DrawCommand.MarkForRenderingThisFrame(camera);
            }
        }
        
        protected abstract IndirectMeshDrawCommand DrawCommand { get; }

        /// <inheritdoc cref="IDisposable.Dispose" />
        public void Dispose()
        {
            DrawCommand.Dispose();
        }
        
        /// <summary>
        /// Indicate that a deserialisation or other event which resets buffers has
        /// occured.
        /// </summary>
        public virtual void ResetBuffers()
        {
            DrawCommand.ResetCommand();
        }
        
        public abstract bool ShouldRender { get; }
        
        public abstract bool IsInputDirty { get; }

        public abstract void UpdateInput();
        
        
        public Transform Transform { get; set; }
        
        /// <summary>
        /// Update the draw command based upon the input values, by updating the mesh,
        /// material and buffers.
        /// </summary>
        /// <remarks>
        /// This does not actually render the spheres. Either call <see cref="Render" />
        /// each frame or call <see cref="AppendToCommandBuffer" />.
        /// </remarks>
        public bool UpdateRenderer()
        {
            if (ShouldRender)
            {
                if (IsInputDirty)
                {
                    UpdateInput();
                }

                InstancingUtility.SetTransform(DrawCommand, Transform);

                return true;
            }

            DrawCommand.SetInstanceCount(0);
            return false;
        }
    }
}