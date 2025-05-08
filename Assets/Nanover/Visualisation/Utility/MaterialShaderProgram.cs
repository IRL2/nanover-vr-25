using UnityEngine;

namespace Nanover.Visualisation.Utility
{
    /// <summary>
    /// A Unity <see cref="UnityEngine.Material" /> represented as an abstract GPU
    /// program.
    /// </summary>
    public class MaterialShaderProgram : IGpuRenderProgram
    {
        public Material Material { get; }

        public MaterialShaderProgram(Material material)
        {
            Material = material;
        }

        /// <inheritdoc cref="IGpuProgram.SetBuffer" />
        public void SetBuffer(string id, ComputeBuffer buffer)
        {
            Material.SetBuffer(id, buffer);
        }

        /// <inheritdoc cref="IGpuRenderProgram.SetBuffer" />
        public void SetKeyword(string keyword, bool active = true)
        {
            if (active)
                Material.EnableKeyword(keyword);
            else
                Material.DisableKeyword(keyword);
        }

        /// <summary>
        /// Set the float value for a shader parameter in the Unity material.
        /// </summary>
        public void SetFloat(string id, float value)
        {
            Material.SetFloat(id, value);
        }

        /// <summary>
        /// Set the matrix value for a shader parameter in the Unity material.
        /// </summary>
        public void SetMatrix(string id, Matrix4x4 value)
        {
            Material.SetMatrix(id, value);
        }

        /// <summary>
        /// Set the color value for a shader parameter in the Unity material.
        /// </summary>
        public void SetColor(string id, Color value)
        {
            Material.SetColor(id, value);
        }
    }
}