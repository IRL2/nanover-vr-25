using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Nanover.Core.Editor
{
    /// <summary>
    /// Editor window to generate a dependency graph using the DOT graph language.
    /// </summary>
    public class AssemblyExplorerWindow : EditorWindow
    {
        [MenuItem("Window/Analysis/Assembly Explorer")]
        public static void ShowWindow()
        {
            GetWindow(typeof(AssemblyExplorerWindow));
        }

        private void Awake()
        {
            titleContent = new GUIContent("Assembly Explorer");
        }

        private bool showTestAssemblies = false;
        private bool showEditorAssemblies = false;

        void OnGUI()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            
            var validAssemblies = assemblies.Where(a => a.FullName.StartsWith("Nanover"));
            var graph = PoC(validAssemblies);
            
            EditorGUILayout.BeginVertical();

            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.ExpandWidth(true));
            if (GUILayout.Button("Copy to Clipboard", EditorStyles.toolbarButton))
            {
                EditorGUIUtility.systemCopyBuffer = graph;
            }
            showTestAssemblies = GUILayout.Toggle(showTestAssemblies, "Test Assemblies", EditorStyles.toolbarButton);
            showEditorAssemblies = GUILayout.Toggle(showEditorAssemblies, "Editor Assemblies", EditorStyles.toolbarButton);
            EditorGUILayout.EndHorizontal();
            
            GUI.enabled = false;
            EditorGUILayout.TextArea(graph);
            GUI.enabled = true;
            EditorGUILayout.EndVertical();
        }

        string GetDisplayName(string assembly)
        {
            if (!showTestAssemblies && (assembly.EndsWith("Tests") || assembly == "Nanover.Testing"))
                return null;
            if (assembly == "netstandard")
                return null;
            if (assembly.StartsWith("nunit"))
                return "NUnit";
            if (assembly == "mscorlib")
                return null;
            if (assembly.StartsWith("System"))
                return null;
            if (assembly.StartsWith("UnityEngine."))
                return null;
            if (!showEditorAssemblies && assembly.EndsWith(".Editor"))
                return null;
            if (assembly.StartsWith("UnityEditor"))
                return "UnityEditor";
            if (assembly.StartsWith("Grpc.Core") || assembly.StartsWith("Google"))
                return "GRPC";
            return assembly;
        }

        private string PoC(IEnumerable<Assembly> assemblies)
        {
            var writer = new StringWriter();
            writer.WriteLine("digraph Dependencies {");

            foreach (var a in assemblies)
            {
                var af = GetDisplayName(a.GetName().Name);
                if (af == null)
                    continue;
                var refs = new List<string>();
                foreach (var r in a.GetReferencedAssemblies())
                {
                    var rf = GetDisplayName(r.Name);
                    if (rf == null)
                        continue;
                    if (!refs.Contains(rf))
                        refs.Add(rf);
                }

                foreach (var rf in refs)
                    writer.WriteLine(@"""{0}"" -> ""{1}"";", af, rf);
            }

            writer.WriteLine("}");

            return writer.ToString();
        }
    }
}