// Copyright (c) Intangible Realities Laboratory. All rights reserved.
// Licensed under the GPL. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Nanover.Frame;
using Nanover.Visualisation;
using Nanover.Visualisation.Utility;
using UnityEngine;

namespace NanoverImd.Examples
{
    /// <summary>
    /// Example of how to use the IndirectMeshDrawCommand for rendering particles.
    /// </summary>
    public sealed class ExampleBallAndStickRenderer : MonoBehaviour
    {
        private readonly IndirectMeshDrawCommand atomDrawCommand = new IndirectMeshDrawCommand();
        private readonly IndirectMeshDrawCommand bondDrawCommand = new IndirectMeshDrawCommand();

#pragma warning disable 0649
        [Header("Rendering")]
        [SerializeField]
        private Material atomMaterial;

        [SerializeField]
        private Mesh atomMesh;

        [SerializeField]
        private Material bondMaterial;

        [SerializeField]
        private Mesh bondMesh;

        [Header("Particle Generation")]
        [SerializeField]
        private int particleCount = 10;

        [SerializeField]
        private float bondCutoff = 1f;

        [SerializeField]
        private float minSize = 1f;

        [SerializeField]
        private float maxSize = 1f;

        [SerializeField]
        private float maxParticleDisplacement = 1;

        [SerializeField]
        private int seed = 0;
#pragma warning restore 0649

        private Vector3[] positions = new Vector3[0];
        private Color[] colors = new Color[0];
        private float[] scales = new float[0];
        private BondPair[] bonds = new BondPair[0];
        private int[] bondOrders = new int[0];

        private void Start()
        {
            Random.InitState(seed);
            GenerateParticles();
        }

        private void Update()
        {
            UpdateRendering();
            UpdateParticles();
            Render();
        }

        private void OnDestroy()
        {
            atomDrawCommand.Dispose();
            bondDrawCommand.Dispose();
        }

        [ContextMenu("Regenerate Particles")]
        private void GenerateParticles()
        {
            positions = Enumerable.Range(0, particleCount)
                                  .Select(i => Random.insideUnitSphere * maxParticleDisplacement)
                                  .ToArray();
            colors = Enumerable.Range(0, particleCount)
                               .Select(i => new Color(Random.value, Random.value, Random.value))
                               .ToArray();
            scales = Enumerable.Range(0, particleCount)
                               .Select(i => Random.Range(minSize, maxSize))
                               .ToArray();

            if (bondCutoff > 0)
            {
                var list = new List<BondPair>();
                for (var i = 0; i < positions.Length; i++)
                {
                    var pos1 = positions[i];
                    for (var j = i + 1; j < positions.Length; j++)
                    {
                        var sqD = Vector3.SqrMagnitude(pos1 - positions[j]);
                        if (sqD < bondCutoff * bondCutoff)
                            list.Add(new BondPair(i, j));
                    }
                }

                bonds = list.ToArray();
                bondOrders = Enumerable.Range(0, bonds.Length)
                                       .Select(i => Random.Range(1, 4))
                                       .ToArray();
            }
        }

        private void UpdateRendering()
        {
            atomDrawCommand.SetMesh(atomMesh);
            atomDrawCommand.SetMaterial(atomMaterial);

            bondDrawCommand.SetMesh(bondMesh);
            bondDrawCommand.SetMaterial(bondMaterial);
        }

        private void UpdateParticles()
        {
            atomDrawCommand.SetInstanceCount(positions.Length);

            InstancingUtility.SetPositions(atomDrawCommand, positions);

            InstancingUtility.SetColors(atomDrawCommand, colors);

            InstancingUtility.SetScales(atomDrawCommand, scales);

            InstancingUtility.SetTransform(atomDrawCommand, transform);

            if (bonds.Length > 0)
            {
                bondDrawCommand.SetInstanceCount(bonds.Length);

                InstancingUtility.SetPositions(bondDrawCommand, positions);

                InstancingUtility.SetColors(bondDrawCommand, colors);

                InstancingUtility.SetScales(bondDrawCommand, scales);

                InstancingUtility.SetEdges(bondDrawCommand, bonds);

                InstancingUtility.SetEdgeCounts(bondDrawCommand, bondOrders);

                InstancingUtility.SetTransform(bondDrawCommand, transform);
            }
        }

        private void Render()
        {
            atomDrawCommand.MarkForRenderingThisFrame();
            bondDrawCommand.MarkForRenderingThisFrame();
        }
    }
}