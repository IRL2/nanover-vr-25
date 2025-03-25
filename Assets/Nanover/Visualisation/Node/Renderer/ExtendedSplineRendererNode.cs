using System;
using System.Collections.Generic;
using Nanover.Visualisation.Node.Spline;
using Nanover.Visualisation.Properties;
using Nanover.Visualisation.Properties.Collections;
using Nanover.Visualisation.Property;
using Nanover.Visualisation.Utility;
using UnityEngine;

namespace Nanover.Visualisation.Node.Renderer
{
    [Serializable]
    public class ExtendedSplineRendererNode : IDisposable
    {
        [SerializeField]
        private bool useBox = true;

        [SerializeField]
        private ExtendedSplineArrayProperty splineSegments = new ExtendedSplineArrayProperty();

        [SerializeField]
        private ColorProperty rendererColor = new ColorProperty();

        public IProperty<ExtendedSplineSegment[]> SplineSegments => splineSegments;

        [SerializeField]
        private int segments = 1;

        [SerializeField]
        private int sides = 1;

        private IndirectMeshDrawCommand drawCommand = new IndirectMeshDrawCommand();

        [SerializeField]
        private Material material;

        private Mesh mesh;

        [SerializeField]
        private FloatProperty splineRadius = new FloatProperty();

        public void Render(Camera camera)
        {
            if (!splineSegments.HasNonEmptyValue())
                return;

            if (mesh == null)
                mesh = GenerateMesh();

            var count = splineSegments.Value.Length;
            drawCommand.SetInstanceCount(count);
            if (count > 0)
            {
                drawCommand.SetMaterial(material);
                drawCommand.SetMesh(mesh);
                InstancingUtility.SetTransform(drawCommand, Transform);

                drawCommand.SetDataBuffer("SplineArray", splineSegments.Value);
                drawCommand.SetColor("_Color", rendererColor.HasValue ? rendererColor.Value : UnityEngine.Color.white);
                drawCommand.SetFloat("_Radius", splineRadius.HasValue ? splineRadius.Value : 1f);

                drawCommand.MarkForRenderingThisFrame(camera);
            }
        }

        public Transform Transform { get; set; }

        private Mesh GenerateMesh()
        {
            var vertices = new List<Vector3>();
            var normals = new List<Vector3>();
            var triangles = new List<int>();

            if (useBox)
                GenerateBox(vertices, normals, triangles);
            else
                GenerateCylinder(vertices, normals, triangles);


            var mesh = new Mesh();
            mesh.SetVertices(vertices);
            mesh.SetNormals(normals);
            mesh.SetTriangles(triangles, 0);

            return mesh;
        }

        private void GenerateBox(List<Vector3> vertices, List<Vector3> normals, List<int> triangles)
        {
            var height = 1f;
            
            for (var level = 0; level <= segments; level++)
            {
                var z = height * level / segments;
                normals.Add(-Vector3.forward);
                normals.Add(-Vector3.forward);
                normals.Add(Vector3.right);
                normals.Add(Vector3.right);
                normals.Add(Vector3.forward);
                normals.Add(Vector3.forward);
                normals.Add(-Vector3.right);
                normals.Add(-Vector3.right);

                vertices.Add(new Vector3(-0.5f, z, -0.5f));
                vertices.Add(new Vector3(0.5f, z, -0.5f));
                vertices.Add(new Vector3(0.5f, z, -0.5f));
                vertices.Add(new Vector3(0.5f, z, 0.5f));
                vertices.Add(new Vector3(0.5f, z, 0.5f));
                vertices.Add(new Vector3(-0.5f, z, 0.5f));
                vertices.Add(new Vector3(-0.5f, z, 0.5f));
                vertices.Add(new Vector3(-0.5f, z, -0.5f));
            }

            triangles.AddRange(new[]
            {
                0, 2, 6
            });
            triangles.AddRange(new[]
            {
                2, 4, 6
            });

            // Bottom

            for (var l = 0; l < segments; l++)
            {
                var lvl = l * 8;
                var lvl2 = l * 8 + 8;
                triangles.AddRange(new[]
                {
                    lvl + 0, lvl2 + 0, lvl + 1
                });
                triangles.AddRange(new[]
                {
                    lvl + 1, lvl2 + 0, lvl2 + 1
                });

                triangles.AddRange(new[]
                {
                    lvl + 2, lvl2 + 2, lvl + 3
                });
                triangles.AddRange(new[]
                {
                    lvl + 3, lvl2 + 2, lvl2 + 3
                });

                triangles.AddRange(new[]
                {
                    lvl + 4, lvl2 + 4, lvl + 5
                });
                triangles.AddRange(new[]
                {
                    lvl + 5, lvl2 + 4, lvl2 + 5
                });

                triangles.AddRange(new[]
                {
                    lvl + 6, lvl2 + 6, lvl + 7
                });
                triangles.AddRange(new[]
                {
                    lvl + 7, lvl2 + 6, lvl2 + 7
                });
            }
        }

        private void GenerateCylinder(List<Vector3> vertices,
                                      List<Vector3> normals,
                                      List<int> triangles)
        {
            var height = 1f;
            var radius = 0.5f;

            var i = 0;
            for (var level = 0; level <= segments; level++)
            {
                var z = height * level / segments;
                for (var side = 0; side < sides; side++)
                {
                    var ang = side / (float) sides * Mathf.PI * 2f;
                    var normal = new Vector3(Mathf.Sin(ang), 0, Mathf.Cos(ang));
                    normals.Add(normal);
                    vertices.Add(radius * normal + Vector3.up * z);
                    i++;
                }
            }

            i = 0;

            for (var l = 0; l < segments; l++)
            for (var s = 0; s < sides; s++)
            {
                triangles.AddRange(new[]
                {
                    l * sides + (s) % sides, l * sides + (s + 1) % sides,
                    (l + 1) * sides + (s) % sides
                });
                triangles.AddRange(new[]
                {
                    l * sides + (s + 1) % sides, (l + 1) * sides + (s + 1) % sides,
                    (l + 1) * sides + (s) % sides
                });
            }
        }

        public void Dispose()
        {
            drawCommand?.Dispose();
        }
    }
}