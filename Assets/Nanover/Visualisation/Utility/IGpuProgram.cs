using UnityEngine;

namespace Nanover.Visualisation.Utility
{
    /// <summary>
    /// An abstraction of both traditional rendering shaders and compute shaders,
    /// representing a GPU program which can have data uploaded to it using
    /// <see cref="ComputeBuffer" />'s.
    /// </summary>
    public interface IGpuProgram
    {
        /// <summary>
        /// Set a <see cref="ComputeBuffer" /> with the provided <paramref name="id" /> for
        /// the GPU program. This will then be available from within the shader.
        /// </summary>
        void SetBuffer(string id, ComputeBuffer buffer);
    }
}