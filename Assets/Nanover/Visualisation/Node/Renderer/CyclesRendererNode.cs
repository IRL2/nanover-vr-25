using System;
using System.Linq;
using Nanover.Visualisation.Properties;
using Nanover.Visualisation.Properties.Collections;
using Nanover.Visualisation.Property;
using UnityEngine;

namespace Nanover.Visualisation.Node.Renderer
{
    [Serializable]
    public class CyclesRendererNode
    {
        private Mesh mesh;

#pragma warning disable 0649
        [SerializeField]
        private Material material;

        [SerializeField]
        private SelectionArrayProperty cycles = new SelectionArrayProperty();

        [SerializeField]
        private Vector3ArrayProperty particlePositions = new Vector3ArrayProperty();

        [SerializeField]
        private ColorArrayProperty particleColors = new ColorArrayProperty();

        [SerializeField]
        private ColorArrayProperty cyclesColor = new ColorArrayProperty();

        [SerializeField]
        private FloatProperty offset = new FloatProperty();
        
        [SerializeField]
        private ColorProperty color = new ColorProperty();

#pragma warning restore 0649

        public Transform Transform { get; set; }

        public void Render(Camera camera)
        {
            if (mesh == null)
                mesh = new Mesh();

            if ((cycles.IsDirty || particlePositions.IsDirty))
            {
                if (cycles.HasNonEmptyValue() && particlePositions.HasNonEmptyValue())
                {
                    GenerateCycleMeshes();
                }

                cycles.IsDirty = false;
                particlePositions.IsDirty = false;
            }

            var block = new MaterialPropertyBlock();
            if (offset.HasValue)
                block.SetFloat("_Offset", offset.Value / 2f);
            if(color.HasValue)
                block.SetColor("_Color", color.Value);

            Graphics.DrawMesh(mesh, Transform.localToWorldMatrix, material, 0, camera, 0, block);
        }

        private Vector3[] vertices = new Vector3[0];
        private Vector3[] normals = new Vector3[0];
        private UnityEngine.Color[] colors = new UnityEngine.Color[0];
        private int[] triangles = new int[0];
        private int triCount = 0;

        private void CalculateTriangles()
        {
            triCount = 0;
            foreach (var cycle in cycles.Value)
            {
                for (var i = 0; i < cycle.Count - 2; i++)
                {
                    for (var j = i + 1; j < cycle.Count - 1; j++)
                    {
                        for (var k = j + 1; k < cycle.Count; k++)
                        {
                            triCount += 1;
                        }
                    }
                }
            }

            Array.Resize(ref vertices, triCount * 3);
            Array.Resize(ref normals, triCount * 3);
            Array.Resize(ref colors, triCount * 3);

            triangles = Enumerable.Range(0, triCount * 3).ToArray();
        }

        private void GenerateCycleMeshes()
        {
            if (!cycles.HasValue)
                return;

            if (cycles.IsDirty)
            {
                CalculateTriangles();
            }

            var positionArray = particlePositions.Value;
            var colorArray = cyclesColor.HasNonEmptyValue() ? cyclesColor.Value : null;

            var particleColorArray =
                particleColors.HasNonEmptyValue() ? particleColors.Value : null;

            var vertexIndex = 0;
            var normalIndex = 0;
            var colorIndex = 0;

            var ci = 0;
            foreach (var cycle in cycles.Value)
            {
                var cycleLength = cycle.Count;
                for (var i = 0; i < cycleLength - 2; i++)
                {
                    var color1 = particleColorArray?[cycle[i]] ??
                                 colorArray?[ci] ?? UnityEngine.Color.white;

                    var pos1 = positionArray[cycle[i]];

                    for (var j = i + 1; j < cycleLength - 1; j++)
                    {
                        var color2 = particleColorArray?[cycle[j]] ??
                                     colorArray?[ci] ?? UnityEngine.Color.white;

                        var pos2 = positionArray[cycle[j]];
                        var d1 = pos2 - pos1;
                        for (var k = j + 1; k < cycleLength; k++)
                        {
                            var color3 = particleColorArray?[cycle[k]] ??
                                         colorArray?[ci] ?? UnityEngine.Color.white;

                            var pos3 = positionArray[cycle[k]];
                            var normal = Vector3.Cross(d1, pos3 - pos1).normalized;
                            vertices[vertexIndex++] = pos1;
                            vertices[vertexIndex++] = pos2;
                            vertices[vertexIndex++] = pos3;
                            normals[normalIndex++] = normal;
                            normals[normalIndex++] = normal;
                            normals[normalIndex++] = normal;
                            colors[colorIndex++] = color1;
                            colors[colorIndex++] = color2;
                            colors[colorIndex++] = color3;
                        }
                    }
                }

                ci++;
            }
            
            mesh.Clear();

            mesh.vertices = vertices;
            mesh.normals = normals;
            mesh.colors = colors;
            mesh.triangles = triangles;
        }
    }
}