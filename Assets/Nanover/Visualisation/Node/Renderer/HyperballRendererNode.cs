using System;
using Nanover.Visualisation.Properties;
using UnityEngine;

namespace Nanover.Visualisation.Node.Renderer
{
    /// <summary>
    /// Visualisation node for rendering hyperballs between particles.
    /// </summary>
    [Serializable]
    public class HyperballRendererNode : ParticleBondRendererNode
    {
        public override bool ShouldRender => base.ShouldRender
                                          && tension.HasValue;

        public override bool IsInputDirty => base.IsInputDirty
                                          || tension.IsDirty;

        protected override void SetMaterialParameters()
        {
            base.SetMaterialParameters();
            DrawCommand.SetFloat("_Tension", tension.Value);
        }
        
        [SerializeField]
        private FloatProperty tension = new FloatProperty();

    }
}