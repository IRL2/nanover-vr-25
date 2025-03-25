using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace Nanover.Visualisation.Utility
{
    /// <summary>
    /// A specific kernel (executable program) of a <see cref="ComputeShader" />, which
    /// is an example of an abstract
    /// GPU program which can have data passed to it from the CPU using
    /// <see cref="ComputeBuffer" />'s.
    /// </summary>
    public class ComputeShaderProgram : IGpuProgram
    {
        private readonly int kernelIndex;

        private readonly ComputeShader shader;

        /// <summary>
        /// Create a <see cref="ComputeShaderProgram" /> that represents the kernel
        /// (executable program) of the
        /// <see cref="ComputeShader" /> <paramref name="shader" /> with the provided
        /// <paramref name="kernelIndex" />
        /// </summary>
        public ComputeShaderProgram(ComputeShader shader, int kernelIndex)
        {
            this.shader = shader;
            this.kernelIndex = kernelIndex;
        }

#if UNITY_EDITOR
        /// <summary>
        /// Path to the asset of the <see cref="ComputeShader" /> containing this program
        /// </summary>
        public string ShaderPath => AssetDatabase.GetAssetPath(shader);
#endif

        /// <inheritdoc cref="IGpuProgram.SetBuffer" />
        public void SetBuffer(string id, ComputeBuffer buffer)
        {
            shader.SetBuffer(kernelIndex, id, buffer);
        }

        /// <summary>
        /// Execute this GPU program by dispatching the corresponding kernel of the
        /// <see cref="ComputeShader" />
        /// </summary>
        public void Dispatch(int threadGroupsX, int threadGroupsY, int threadGroupsZ)
        {
            shader.Dispatch(kernelIndex, threadGroupsX, threadGroupsY, threadGroupsZ);
        }
    }
}