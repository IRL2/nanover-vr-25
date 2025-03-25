using System;
using Nanover.Visualisation.Properties;
using Nanover.Visualisation.Property;
using Nanover.Visualisation.Utility;
using UnityEngine;

namespace Nanover.Visualisation.Node.Renderer
{
    /// <summary>
    /// Extends the normal sphere rendering by providing residue IDs in a separate buffer.
    /// </summary>
    [Serializable]
    public class GoodsellSphereRendererNode : ParticleSphereRendererNode
    {
        [SerializeField]
        private IntArrayProperty particleResidues = new IntArrayProperty();

        public IntArrayProperty ParticleResidues => particleResidues;

        protected override void UpdateBuffers()
        {
            base.UpdateBuffers();
            UpdateResidueIdsIfDirty();
        }
        
        private void UpdateResidueIdsIfDirty()
        {
            if (particleResidues.IsDirty && particleResidues.HasNonEmptyValue())
            {
                DrawCommand.SetDataBuffer("ResidueIds", particleResidues.Value);
                particleResidues.IsDirty = false;
            }
        }

        public override void ResetBuffers()
        {
            base.ResetBuffers();
            particleResidues.IsDirty = true;
        }
    }
}