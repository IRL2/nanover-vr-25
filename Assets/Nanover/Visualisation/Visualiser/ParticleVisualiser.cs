using System.Collections.Generic;
using Nanover.Core;
using Nanover.Core.Science;
using Nanover.Frame;
using Nanover.Visualisation.Node.Adaptor;
using Nanover.Visualisation.Node.Color;
using Nanover.Visualisation.Node.Renderer;
using Nanover.Visualisation.Node.Scale;
using Nanover.Visualisation.Properties;
using Nanover.Visualisation.Property;
using UnityEngine;

namespace Nanover.Visualisation.Visualiser
{
    /// <summary>
    /// Simple integrated visualiser, with several visualisation nodes embedded into
    /// the same <see cref="MonoBehaviour" /> to render a sphere-and-bond
    /// representation of a frame.
    /// </summary>
    /// <remarks>
    /// The five visualisation nodes that make up this visualisation are serialized by
    /// the Unity editor.
    /// </remarks>
    [ExecuteAlways]
    public class ParticleVisualiser : MonoBehaviour, IFrameConsumer, ISerializationCallbackReceiver
    {
        #region Visualisation Properties

        [Header("Rendering")]
        [SerializeField]
        [Tooltip("The material used by the visualiser for particles.")]
        [RequiredProperty]
        private MaterialProperty particleMaterial = new MaterialProperty();

        /// <summary>
        /// The material used by the visualiser for particles.
        /// </summary>
        public IProperty<Material> ParticleMaterial => particleMaterial;

        [SerializeField]
        [Tooltip("The mesh used by the visualiser for bonds.")]
        private MaterialProperty bondMaterial = new MaterialProperty();

        /// <summary>
        /// The material used by the visualiser for bonds.
        /// </summary>
        public IProperty<Material> BondMaterial => bondMaterial;

        [SerializeField]
        [Tooltip("The mesh used by the visualiser for particles.")]
        [RequiredProperty]
        private MeshProperty particleMesh = new MeshProperty();

        /// <summary>
        /// The mesh used by the visualiser for particles.
        /// </summary>
        public IProperty<Mesh> ParticleMesh => particleMesh;

        [SerializeField]
        [Tooltip("The mesh used by the visualiser for bonds.")]
        private MeshProperty bondMesh = new MeshProperty();

        /// <summary>
        /// The mesh used by the visualiser for bonds.
        /// </summary>
        public IProperty<Mesh> BondMesh => bondMesh;

        [Header("Colors")]
        [SerializeField]
        [Tooltip("Tint applied to the color of particles and bonds.")]
        private ColorProperty tint = new ColorProperty
        {
            Value = Color.white
        };

        /// <summary>
        /// Tint applied to the color of particles and bonds.
        /// </summary>
        public IProperty<Color> Tint => tint;

        [SerializeField]
        [Tooltip("Per-element color scheme for coloring atoms.")]
        private ElementColorMappingProperty colorScheme =
            new ElementColorMappingProperty();

        /// <summary>
        /// Per-element color scheme for coloring atoms.
        /// </summary>
        public IProperty<IMapping<Element, Color>> ColorScheme => colorScheme;

        [Header("Scale")]
        [SerializeField]
        [Tooltip("Multiplier for the scale of the particles.")]
        private FloatProperty particleScale = new FloatProperty
        {
            Value = 1f
        };

        /// <summary>
        /// Multiplier for the scale of the particles.
        /// </summary>
        public IProperty<float> ParticleScale => particleScale;

        [SerializeField]
        [Tooltip("Multiplier for the scale of the bonds.")]
        private FloatProperty bondScale = new FloatProperty
        {
            Value = 0.1f
        };

        /// <summary>
        /// Should this use VdW radii?
        /// </summary>
        public IProperty<bool> UseVdwRadii => useVdwRadii;

        [SerializeField]
        [Tooltip("Should this use VdW radii.")]
        private BoolProperty useVdwRadii = new BoolProperty
        {
            Value = true
        };

        /// <summary>
        /// Multiplier for the scale of the bonds.
        /// </summary>
        public IProperty<float> BondScale => particleScale;

        #endregion

        #region Visualisation Nodes

        private readonly FrameAdaptorNode frameAdaptor = new FrameAdaptorNode();

        private readonly ElementColorMappingNode particleColors = new ElementColorMappingNode();

        private readonly VdwScaleNode particleScales = new VdwScaleNode();

        private readonly ParticleBondRendererNode bondRenderer = new ParticleBondRendererNode();

        private readonly ParticleSphereRendererNode sphereRenderer = new ParticleSphereRendererNode();

        #endregion

        /// <inheritdoc cref="IFrameConsumer.FrameSource" />
        public ITrajectorySnapshot FrameSource
        {
            set => frameAdaptor.FrameSource = value;
        }

        private void Setup()
        {
            // Setup links between the various nodes of the visualisation

            var usePerElementColor = colorScheme.HasNonNullValue();

            sphereRenderer.RendererColor.LinkedProperty = tint;
            sphereRenderer.Material.LinkedProperty = particleMaterial;
            sphereRenderer.Mesh.LinkedProperty = particleMesh;
            sphereRenderer.RendererScale.LinkedProperty = particleScale;

            bondRenderer.RendererColor.LinkedProperty = tint;
            bondRenderer.Material.LinkedProperty = bondMaterial;
            bondRenderer.Mesh.LinkedProperty = bondMesh;
            bondRenderer.ParticleScale.LinkedProperty = particleScale;
            bondRenderer.BondScale.LinkedProperty = bondScale;
            bondRenderer.BondOrders.LinkedProperty = frameAdaptor.BondOrders;

            if (usePerElementColor)
            {
                particleColors.Mapping.LinkedProperty = colorScheme;
                particleColors.Elements.LinkedProperty = frameAdaptor.ParticleElements;
            }

            if (useVdwRadii.HasValue && useVdwRadii)
            {
                particleScales.Elements.LinkedProperty = frameAdaptor.ParticleElements;
            }

            sphereRenderer.ParticlePositions.LinkedProperty = frameAdaptor.ParticlePositions;
            if (useVdwRadii.HasValue && useVdwRadii)
                sphereRenderer.ParticleScales.LinkedProperty = particleScales.Scales;

            if (usePerElementColor)
                sphereRenderer.ParticleColors.LinkedProperty = particleColors.Colors;

            bondRenderer.ParticlePositions.LinkedProperty = frameAdaptor.ParticlePositions;
            if (useVdwRadii.HasValue && useVdwRadii)
                bondRenderer.ParticleScales.LinkedProperty = particleScales.Scales;
            if (usePerElementColor)
                bondRenderer.ParticleColors.LinkedProperty = particleColors.Colors;

            bondRenderer.BondPairs.LinkedProperty = frameAdaptor.BondPairs;

            // Set transforms of renderers
            var transform = this.transform;
            sphereRenderer.Transform = transform;
            bondRenderer.Transform = transform;
        }

        private void Update()
        {
            particleColors.Refresh();
            if (useVdwRadii)
                particleScales.Refresh();
            if (needRefresh)
            {
                Setup();
                sphereRenderer.ResetBuffers();
                bondRenderer.ResetBuffers();
                needRefresh = false;
            }
        }

        private bool needRefresh = false;

        private void OnDestroy()
        {
            sphereRenderer.Dispose();
            bondRenderer.Dispose();
        }


        private void OnEnable()
        {
            Camera.onPreCull += Render;
            Setup();
            needRefresh = false;
        }

        private void OnDisable()
        {
            Camera.onPreCull -= Render;
        }

        private void Render(Camera camera)
        {
            sphereRenderer.Render(camera);
            bondRenderer.Render(camera);
        }

        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            needRefresh = true;
        }
    }
}