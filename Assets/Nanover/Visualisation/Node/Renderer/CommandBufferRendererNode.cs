using System;
using System.Collections.Generic;
using Nanover.Core;
using UnityEngine;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

namespace Nanover.Visualisation.Node.Renderer
{
    /// <summary>
    /// Base node for a renderer based upon using <see cref="CommandBuffer"/>,
    /// which is a list of commands (rendering to textures, blitting, etc.)
    /// that can be executed at some point in the rendering pipeline.
    /// </summary>
    public abstract class CommandBufferRendererNode : IDisposable
    {
        /// <summary>
        /// Cached store of per camera command buffers.
        /// </summary>
        private Dictionary<Camera, List<(CameraEvent, CommandBuffer)>> buffers =
            new Dictionary<Camera, List<(CameraEvent, CommandBuffer)>>();

        /// <summary>
        /// Cleanup all buffers, removing them from the cameras.
        /// </summary>
        public virtual void Dispose()
        {
            foreach (var (camera, buffers) in buffers)
            {
                if (camera)
                {
                    foreach (var (evnt, buffer) in buffers)
                    {
                        camera.RemoveCommandBuffer(evnt, buffer);
                        buffer.Dispose();
                    }
                }
            }

            buffers.Clear();
            foreach (var material in materials)
                Object.DestroyImmediate(material);
        }

        /// <summary>
        /// Materials created for this renderer. Stored so they
        /// can be destroyed by <see cref="Cleanup()"/>
        /// </summary>
        private List<Material> materials = new List<Material>();

        /// <summary>
        /// Create a new material for use with this renderer.
        /// </summary>
        protected Material CreateMaterial(Shader shader)
        {
            var material = new Material(shader);
            materials.Add(material);
            return material;
        }

        /// <summary>
        /// Render to the given camera using a command buffer, using a cached buffer
        /// if it has already been created.
        /// </summary>
        /// <param name="cam"></param>
        public virtual void Render(Camera cam)
        {
            if (buffers.ContainsKey(cam))
                return;
            buffers[cam] = new List<(CameraEvent, CommandBuffer)>();
            foreach (var (cameraEvent, buffer) in GetBuffers(cam))
            {
                cam.AddCommandBuffer(cameraEvent, buffer);
                buffers[cam].Add((cameraEvent, buffer));
            }
        }

        /// <summary>
        /// Get the command buffers and their triggering events for a given camera.
        /// </summary>
        protected abstract IEnumerable<(CameraEvent Event, CommandBuffer Buffer)> GetBuffers(Camera camera);
    }
    
    
}