using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Nanover.Visualisation.Components;
using Nanover.Visualisation.Components.Adaptor;
using Nanover.Visualisation.Node.Input;
using Nanover.Visualisation.Node.Output;
using Nanover.Visualisation.Property;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Nanover.Visualisation.Editor
{
    /// <summary>
    /// Generates a graph of the currently selected visualiser as graph in the DOT
    /// language, suitable for visualisation using GraphViz.
    /// </summary>
    internal static class GraphVizVisualisationGraph
    {
        [MenuItem("Nanover/Generate GraphVIZ")]
        internal static void GenerateGraphViz()
        {
            var root = Selection.activeGameObject;
            if (root == null)
                root = PrefabStageUtility.GetCurrentPrefabStage()?.prefabContentsRoot;
            if (root == null)
                return;

            var file = new StringWriter();

            file.WriteLine("digraph G {");

            file.WriteLine("rankdir=RL");

            var connections = new List<(long, long)>();

            var gotoChildren = true;

            while (root != null)
            {
                DrawObject(root, file, connections, gotoChildren);
                gotoChildren = false;
                root = root.transform.parent?.gameObject;
            }

            foreach (var connection in connections)
                file.WriteLine($"{connection.Item1} -> {connection.Item2};");


            file.WriteLine("}");

            var str = file.ToString();

            EditorGUIUtility.systemCopyBuffer = str;
        }

        internal static void DrawObject(GameObject root,
                                        TextWriter file,
                                        List<(long, long)> connections,
                                        bool gotoChildren)
        {
            file.WriteLine($"subgraph cluster_{GetId(root)} {{");
            file.WriteLine($"label = \"{root.name}\"");

            foreach (var child in root.GetComponents<VisualisationComponent>())
            {
                file.WriteLine($"subgraph cluster_{GetId(child)} {{");
                file.WriteLine($"label = \"{child.GetType().Name}\"");
                foreach (var (name, prop) in GetProperties(child))
                {
                    var label = name;
                    if (child.GetWrappedVisualisationNode() is IInputNode input)
                        label = input.Name;
                    if (child.GetWrappedVisualisationNode() is IOutputNode output)
                        label = output.Name;
                    if (prop.HasValue && prop.PropertyType.IsArray)
                        label +=
                            $" {prop.PropertyType.GetElementType().Name}[{(prop.Value as Array).Length}]";
                    var color = prop.HasValue ? "green" : "red";
                    file.WriteLine($"{GetId(prop)} [label=\"{label}\" color={color} shape=box];");
                    FindConnections(prop, connections);
                }

                if (child is SecondaryStructureAdaptor ss)
                {
                    var node = ss.Node;

                    file.WriteLine($"subgraph cluster_{GetId(node.polypeptideSequence)} {{");
                    file.WriteLine($"label = \"polypeptideSequence\"");
                    foreach (var (name, prop) in VisualisationUtility.GetAllPropertyFields(
                        node.polypeptideSequence))
                    {
                        var color = prop.HasValue ? "green" : "red";
                        file.WriteLine(
                            $"{GetId(prop)} [label=\"{name}\" color={color} shape=box];");
                        FindConnections(prop, connections);
                    }

                    file.WriteLine("}");

                    file.WriteLine($"subgraph cluster_{GetId(node.secondaryStructure)} {{");
                    file.WriteLine($"label = \"secondaryStructure\"");
                    foreach (var (name, prop) in VisualisationUtility.GetAllPropertyFields(
                        node.secondaryStructure))
                    {
                        var color = prop.HasValue ? "green" : "red";
                        file.WriteLine(
                            $"{GetId(prop)} [label=\"{name}\" color={color} shape=box];");
                        FindConnections(prop, connections);
                    }

                    file.WriteLine("}");
                }

                file.WriteLine("}");
            }

            if (gotoChildren)
            {
                foreach (var child in Enumerable
                                      .Range(0, root.transform.childCount)
                                      .Select(i => root.transform.GetChild(i)))
                {
                    DrawObject(child.gameObject, file, connections, gotoChildren);
                }
            }

            file.WriteLine("}");
        }

        internal static IEnumerable<(string, IReadOnlyProperty)> GetProperties(
            VisualisationComponent child)
        {
            foreach (var prop in child.GetProperties())
            {
                yield return prop;
                if (prop.property is IFilteredProperty filter)
                    yield return (prop.name, filter.SourceProperty);
            }
        }

        internal static void FindConnections(IReadOnlyProperty prop, List<(long, long)> connections)
        {
            if (prop is IProperty property && property.HasLinkedProperty)
            {
                var linked = property.LinkedProperty;
                var conn = (GetId(prop), GetId(linked));
                if (!connections.Contains(conn))
                    connections.Add(conn);
                FindConnections(linked, connections);
            }

            if (prop is IFilteredProperty filtered)
            {
                var conn1 = (GetId(prop), GetId(filtered.SourceProperty));
                if (!connections.Contains(conn1))
                    connections.Add(conn1);
                var conn2 = (GetId(prop), GetId(filtered.FilterProperty));
                if (!connections.Contains(conn2))
                    connections.Add(conn2);
            }
        }

        internal static long GetId(object obj)
        {
            return (long) int.MaxValue + (long) obj.GetHashCode();
        }
    }
}