using System.Collections.Generic;
using Nanover.Visualisation.Components;
using UnityEditor;

namespace Nanover.Visualisation.Editor
{
    /// <summary>
    /// Custom editor for <see cref="VisualisationComponent" />, that draws all the
    /// children of the wrapped node as direct children of the component.
    /// </summary>
    [CustomEditor(typeof(VisualisationComponent), true)]
    public sealed class VisualisationComponentEditor : UnityEditor.Editor
    {
        /// <inheritdoc cref="UnityEditor.Editor.OnInspectorGUI" />
        public override void OnInspectorGUI()
        {
            var obj = serializedObject;
            EditorGUI.BeginChangeCheck();
            obj.UpdateIfRequiredOrScript();

            var node = obj.FindProperty("node").Copy();

            foreach (var child in GetChildren(node))
                EditorGUILayout.PropertyField(child, true);

            var iterator = obj.GetIterator();
            for (var enterChildren = true;
                 iterator.NextVisible(enterChildren);
                 enterChildren = false)
            {
                if (iterator.propertyPath == "inputLinkCollection")
                    continue;
                if (iterator.propertyPath == "node")
                    continue;
                if (iterator.propertyPath == "m_Script")
                    continue;
                EditorGUILayout.PropertyField(iterator, true);
            }

            obj.ApplyModifiedProperties();
            EditorGUI.EndChangeCheck();
        }

        /// <summary>
        /// Get all direct children of a <see cref="SerializedProperty" />
        /// </summary>
        private static IEnumerable<SerializedProperty> GetChildren(SerializedProperty property)
        {
            var next = property.GetEndProperty();
            var firstChild = property.Copy();
            if (!firstChild.NextVisible(true))
                yield break;

            if (next.Equals(firstChild))
                yield break;
            while (!next.Equals(firstChild))
            {
                yield return firstChild;
                if (!firstChild.NextVisible(false))
                    yield break;
            }
        }
    }
}