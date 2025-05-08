using System;
using Nanover.Visualisation.Utility;
using UnityEngine;
using UnityEngine.Rendering;

namespace Nanover.Visualisation
{
    /// <summary>
    /// Wraps
    /// <see
    ///     cref="Graphics.DrawMeshInstancedIndirect(Mesh,int,Material,Bounds,ComputeBuffer)" />
    /// to add state management
    /// for mesh, material, and computer buffers.
    /// </summary>
    /// <remarks>
    /// To actually execute the draw command, two approaches are possible. The first is
    /// to call <see cref="MarkForRenderingThisFrame" />, which will draw just for this
    /// frame. The other is to append it to a <see cref="CommandBuffer" /> using
    /// <see cref="AppendToCommandBuffer" />.
    /// </remarks>
    public class IndirectMeshDrawCommand : IDisposable
    {
        /// <summary>
        /// Bounding box in which the mesh is considered to be contained within. Currently,
        /// this is
        /// not computed, so an arbitrary bounds is used.
        /// </summary>
        /// <remarks>
        /// The value of the bounds is taken from the example usage at
        /// https://docs.unity3d.com/ScriptReference/Graphics.DrawMeshInstancedIndirect.html
        /// </remarks>
        private static readonly Bounds ArbitraryBounds =
            new Bounds(Vector3.zero, new Vector3(100, 100, 100));

        private readonly ComputeBufferCollection dataBuffers = new ComputeBufferCollection();

        private ComputeBuffer drawArgumentsBuffer;
        private bool areDrawArgumentsDirty;

        private int instanceCount;

        private Mesh mesh;
        private int submeshIndex;
        private Material renderMaterial;
        private Material sourceMaterial;

        private MaterialShaderProgram shaderProgram;

        /// <summary>
        /// Is there enough valid data to render something?
        /// </summary>
        private bool IsRenderable => instanceCount > 0 && mesh != null && renderMaterial != null;

        /// <summary>
        /// Dispose the compute buffers used for draw arguments and shader data.
        /// </summary>
        public void Dispose()
        {
            drawArgumentsBuffer?.Dispose();
            dataBuffers.Dispose();
        }

        /// <summary>
        /// Set the number of mesh copies to be drawn.
        /// </summary>
        public void SetInstanceCount(int instanceCount)
        {
            if (instanceCount != this.instanceCount)
            {
                this.instanceCount = instanceCount;
                areDrawArgumentsDirty = true;
            }
        }

        /// <summary>
        /// Set the mesh/submesh to be drawn
        /// </summary>
        public void SetMesh(Mesh mesh, int submeshIndex = 0)
        {
            if (mesh != this.mesh || submeshIndex != this.submeshIndex)
            {
                this.mesh = mesh;
                this.submeshIndex = submeshIndex;
                areDrawArgumentsDirty = true;
            }
        }

        /// <summary>
        /// Set the source material to use for drawing. A copy will be made.
        /// </summary>
        public void SetMaterial(Material material)
        {
            if (material != sourceMaterial)
            {
                sourceMaterial = material;
                SetMaterialDirect(new Material(material));
            }
        }

        /// <summary>
        /// Set the actual material to use for drawing. The material will be
        /// directly manipulated.
        /// </summary>
        public void SetMaterialDirect(Material material)
        {
            if (material != renderMaterial)
            {
                renderMaterial = material;

                shaderProgram = new MaterialShaderProgram(material);
                dataBuffers.ApplyAllBuffersToShader(shaderProgram);
            }
        }

        /// <summary>
        /// Set a named buffer of data for use by the material when drawing.
        /// </summary>
        public void SetDataBuffer<T>(string id, T[] content) where T : struct
        {
            dataBuffers.SetBuffer(id, content);
        }

        /// <summary>
        /// Enable/disable a named keyword in the material.
        /// </summary>
        public void SetKeyword(string keyword, bool active = true)
        {
            shaderProgram.SetKeyword(keyword, active);
        }

        /// <inheritdoc cref="MaterialShaderProgram.SetFloat" />
        public void SetFloat(string id, float value) => shaderProgram.SetFloat(id, value);

        /// <inheritdoc cref="MaterialShaderProgram.SetMatrix" />
        public void SetMatrix(string id, Matrix4x4 value) => shaderProgram.SetMatrix(id, value);

        /// <inheritdoc cref="MaterialShaderProgram.SetColor" />
        public void SetColor(string id, Color value) => shaderProgram.SetColor(id, value);

        /// <summary>
        /// Indicate that the draw command should render this frame.
        /// </summary>
        public void MarkForRenderingThisFrame(Camera camera = null)
        {
            if (!IsRenderable)
                return;

            UpdateDrawArgumentsBuffer();
            UpdateDataBuffers();

            Graphics.DrawMeshInstancedIndirect(mesh,
                                               submeshIndex,
                                               renderMaterial,
                                               ArbitraryBounds,
                                               drawArgumentsBuffer,
                                               0,
                                               (MaterialPropertyBlock) null,
                                               ShadowCastingMode.On,
                                               true,
                                               0,
                                               camera);
        }

        /// <summary>
        /// Add a command to the provided buffer that would execute this draw command.
        /// </summary>
        public void AppendToCommandBuffer(CommandBuffer buffer)
        {
            if (!IsRenderable)
                return;

            UpdateDrawArgumentsBuffer();
            UpdateDataBuffers();

            buffer.DrawMeshInstancedIndirect(mesh,
                                             submeshIndex,
                                             renderMaterial,
                                             0,
                                             drawArgumentsBuffer,
                                             0);
        }

        private void UpdateDataBuffers()
        {
            dataBuffers.ApplyDirtyBuffersToShader(shaderProgram);
            dataBuffers.ClearDirtyBuffers();
        }

        /// <summary>
        /// Clear all buffers
        /// </summary>
        public void ResetCommand()
        {
            foreach (var buffer in dataBuffers.Values)
                buffer?.Dispose();
            drawArgumentsBuffer?.Dispose();
            dataBuffers.Clear();
            drawArgumentsBuffer = null;
            sourceMaterial = null;
            renderMaterial = null;
            mesh = null;
        }

        private void UpdateDrawArgumentsBuffer()
        {
            if (!IsRenderable || !areDrawArgumentsDirty)
                if (!IsRenderable || (drawArgumentsBuffer != null && !areDrawArgumentsDirty))
                    return;

            if (drawArgumentsBuffer == null)
                drawArgumentsBuffer =
                    new ComputeBuffer(5, sizeof(uint), ComputeBufferType.IndirectArguments);

            var startInstanceLocation = 0u;
            var instanceCount = (uint) this.instanceCount;

            var vertexCountPerInstance = mesh.GetIndexCount(submeshIndex);
            var startVertexCount = mesh.GetIndexStart(submeshIndex);
            var baseVertexCount = mesh.GetBaseVertex(submeshIndex);

            drawArgumentsBuffer.SetData(new[]
            {
                vertexCountPerInstance, instanceCount, startVertexCount, baseVertexCount,
                startInstanceLocation
            });

            areDrawArgumentsDirty = false;
        }

        public void ClearDataBuffer(string key)
        {
            dataBuffers.RemoveBuffer(key);
        }
    }
}