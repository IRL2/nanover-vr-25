namespace Nanover.Visualisation.Utility
{
    /// <summary>
    /// A GPU program that also supports keywords that modify how the shader works
    /// </summary>
    public interface IGpuRenderProgram : IGpuProgram
    {
        /// <summary>
        /// Set a keyword on the material
        /// </summary>
        void SetKeyword(string keyword, bool enabled = true);
    }
}