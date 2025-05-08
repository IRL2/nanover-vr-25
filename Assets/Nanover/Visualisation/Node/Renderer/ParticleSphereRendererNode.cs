using System;
using Nanover.Visualisation.Properties;
using Nanover.Visualisation.Properties.Collections;
using Nanover.Visualisation.Property;
using Nanover.Visualisation.Utility;
using UnityEngine;
using UnityEngine.Rendering;

namespace Nanover.Visualisation.Node.Renderer
{
    /// <summary>
    /// Visualisation node for rendering particles using spheres.
    /// </summary>
    /// <remarks>
    /// A call to <see cref="UpdateRenderer" /> should be made every frame. Depending
    /// on use case, either call <see cref="Render" /> to draw for a single frame, or
    /// add to a <see cref="CommandBuffer" /> using
    /// <see cref="AppendToCommandBuffer" />.
    /// </remarks>
    [Serializable]
    public class ParticleSphereRendererNode : IndirectMeshRenderer, IDisposable
    {
        private readonly IndirectMeshDrawCommand drawCommand = new IndirectMeshDrawCommand();

        /// <summary>
        /// Positions of particles.
        /// </summary>
        public IProperty<Vector3[]> ParticlePositions => particlePositions;

        /// <summary>
        /// Color of particles.
        /// </summary>
        public IProperty<UnityEngine.Color[]> ParticleColors => particleColors;

        /// <summary>
        /// Scale of particles.
        /// </summary>
        public IProperty<float[]> ParticleScales => particleScales;

        /// <summary>
        /// Overall color of the renderer. Each particle color will be multiplied by this
        /// value.
        /// </summary>
        public IProperty<UnityEngine.Color> RendererColor => rendererColor;

        /// <summary>
        /// Overall scaling of the renderer. Each particle scale will be multiplied by this
        /// value.
        /// </summary>
        public IProperty<float> RendererScale => rendererScale;

        public IProperty<Mesh> Mesh => mesh;

        public IProperty<Material> Material => material;

#pragma warning disable 0649
        [SerializeField]
        private Vector3ArrayProperty particlePositions = new Vector3ArrayProperty();

        [SerializeField]
        private ColorArrayProperty particleColors = new ColorArrayProperty();

        [SerializeField]
        private FloatArrayProperty particleScales = new FloatArrayProperty();

        [SerializeField]
        private ColorProperty rendererColor = new ColorProperty();

        [SerializeField]
        private FloatProperty rendererScale = new FloatProperty();

        [SerializeField]
        private MeshProperty mesh = new MeshProperty();

        [SerializeField]
        private MaterialProperty material = new MaterialProperty();
#pragma warning restore 0649

        public override bool ShouldRender => mesh.HasNonNullValue()
                                          && material.HasNonNullValue()
                                          && particlePositions.HasNonEmptyValue()
                                          && rendererColor.HasValue
                                          && rendererScale.HasValue
                                          && Transform != null
                                          && InstanceCount > 0;

        private int InstanceCount => particlePositions.Value.Length;

        public override bool IsInputDirty => mesh.IsDirty
                                          || material.IsDirty
                                          || rendererColor.IsDirty
                                          || rendererScale.IsDirty
                                          || particlePositions.IsDirty
                                          || particleColors.IsDirty
                                          || particleScales.IsDirty;


        public override void UpdateInput()
        {
            UpdateMeshAndMaterials();

            drawCommand.SetInstanceCount(InstanceCount);
            drawCommand.SetFloat("_Scale", rendererScale.Value);
            drawCommand.SetColor("_Color", rendererColor.Value);
            rendererScale.IsDirty = false;
            rendererColor.IsDirty = false;

            UpdateBuffers();
        }

        protected virtual void UpdateBuffers()
        {
            UpdatePositionsIfDirty();
            UpdateColorsIfDirty();
            UpdateScalesIfDirty();
        }

        private void UpdatePositionsIfDirty()
        {
            if (particlePositions.IsDirty && particlePositions.HasNonEmptyValue())
            {
                InstancingUtility.SetPositions(drawCommand, particlePositions.Value);

                particlePositions.IsDirty = false;
            }
        }

        private void UpdateColorsIfDirty()
        {
            if (particleColors.IsDirty && particleColors.HasNonEmptyValue())
            {
                InstancingUtility.SetColors(drawCommand, particleColors.Value);

                particleColors.IsDirty = false;
            }
        }

        private void UpdateScalesIfDirty()
        {
            if (particleScales.IsDirty && particleScales.HasNonEmptyValue())
            {
                InstancingUtility.SetScales(drawCommand, particleScales.Value);

                particleScales.IsDirty = false;
            }
        }

        private void UpdateMeshAndMaterials()
        {
            drawCommand.SetMesh(mesh);
            drawCommand.SetMaterial(material);
            mesh.IsDirty = false;
            material.IsDirty = false;
        }

        protected override IndirectMeshDrawCommand DrawCommand => drawCommand;

        public override void ResetBuffers()
        {
            base.ResetBuffers();
            particleColors.IsDirty = true;
            particlePositions.IsDirty = true;
            particleScales.IsDirty = true;
        }
    }
}