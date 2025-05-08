using System;
using System.Collections.Generic;
using System.Linq;
using Nanover.Visualisation.Components;
using Nanover.Visualisation.Property;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Nanover.Visualisation.Editor
{
    /// <summary>
    /// Property drawer override for drawing <see cref="Property" /> fields
    /// which belong to a <see cref="VisualisationComponent" />.
    /// </summary>
    [InitializeOnLoad]
    public static class VisualiserComponentsPropertyDrawer
    {
        static VisualiserComponentsPropertyDrawer()
        {
            VisualisationPropertyDrawer.AddOverride(OnGUI);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>True if this should override the default property GUI, false if it shouldn't.</returns>
        private static bool OnGUI(ref Rect rect, SerializedProperty property, GUIContent label)
        {
            var collection = property.serializedObject.FindProperty("inputLinkCollection");
            if (collection == null) return false;

            // Get link which is stored in the VisualisationComponent, which
            // describes if this property is linked to anything
            var link = FindVisualisationLink(collection, property.name);

            var linkToggleRect = new Rect(rect)
            {
                width = 40
            };
            linkToggleRect.x = rect.xMax - linkToggleRect.width;
            rect.width -= 44;

            // Is this property linked
            var isLinked = link.HasValue;

            if (GUI.Button(linkToggleRect, isLinked ? "Unlink" : "Link", EditorStyles.miniButton))
            {
                if (isLinked)
                {
                    // Unlink by destroying the link in the VisualisationComponent
                    collection.DeleteArrayElementAtIndex(link.Value.Index);
                    isLinked = false;
                }
                else
                {
                    // Create a link in the VisualisationComponent
                    var index = collection.arraySize;
                    collection.InsertArrayElementAtIndex(index);
                    var linkProperty = collection.GetArrayElementAtIndex(index);
                    var destProperty = linkProperty.FindPropertyRelative(
                        nameof(VisualisationComponent.InputPropertyLink.destinationFieldName));
                    destProperty.stringValue = property.name;
                }
            }

            if (isLinked)
            {
                var linkRect = new Rect(rect);
                linkRect.yMin = linkRect.yMax - 16f;
                DrawSourceForLinkedProperty(linkRect, link.Value);
                rect.height -= 16;
            }
            else
            {
                return false;
            }

            return true;
        }

        private static SerializedInputLink? FindVisualisationLink(
            SerializedProperty linkCollectionSerProp,
            string name)
        {
            for (var i = 0; i < linkCollectionSerProp.arraySize; i++)
            {
                var currentLinkSerProp = linkCollectionSerProp.GetArrayElementAtIndex(i);
                var link = new SerializedInputLink
                {
                    Index = i,
                    DestinationProperty = currentLinkSerProp.FindPropertyRelative(
                        nameof(VisualisationComponent.InputPropertyLink.destinationFieldName))
                };

                if (name != link.DestinationProperty.stringValue)
                    continue;

                link.SourceProperty =
                    currentLinkSerProp.FindPropertyRelative(
                        nameof(VisualisationComponent.InputPropertyLink.sourceFieldName));
                link.SourceComponent =
                    currentLinkSerProp.FindPropertyRelative(
                        nameof(VisualisationComponent.InputPropertyLink.sourceComponent));

                return link;
            }

            return null;
        }

        private struct SerializedInputLink
        {
            public int Index;

            public SerializedProperty SourceComponent;

            public SerializedProperty SourceProperty;

            public SerializedProperty DestinationProperty;
        }

        class ValidShortcut
        {
            public GUIContent Label;
            public Object Source;
            public string Name;
        }

        static IEnumerable<ValidShortcut> GetShortcuts(MonoBehaviour behaviour,
                                                       Type destinationType)
        {
            if (behaviour == null)
                yield break;
            var go = behaviour.gameObject;
            foreach (var shortcut in GetShortcuts(go, destinationType))
                yield return shortcut;
        }

        private static IEnumerable<ValidShortcut> GetShortcuts(GameObject go, Type destinationType)
        {
            foreach (var obj in go.GetComponents<VisualisationComponent>())
            {
                var fields = GetPropertiesOfType(obj, destinationType);
                foreach (var field in fields)
                {
                    yield return new ValidShortcut
                    {
                        Label = new GUIContent($"{go.name}[{obj.GetType().Name}]: {field}"),
                        Source = obj,
                        Name = field
                    };
                }
            }

            if (go.transform.parent != null)
                foreach (var shortcut in GetShortcuts(go.transform.parent.gameObject,
                                                      destinationType))
                    yield return shortcut;
        }

        private static IEnumerable<string> GetPropertiesOfType(
            IPropertyProvider provider,
            Type type)
        {
            var names = new List<string>();
            names.AddRange(provider.GetPotentialProperties().Where(a => a.type == type)
                                   .Select(a => a.name));
            names.AddRange(provider.GetProperties().Where(a => a.property.PropertyType == type)
                                   .Select(a => a.name));
            return names.Distinct();
        }

        private static void DrawSourceForLinkedProperty(Rect rect, SerializedInputLink link)
        {
            var sourceFieldWidth = 144;

            var sourceDropdownRect = new Rect(rect.x - 16, rect.y, 16, rect.height);

            var destinationObject =
                link.DestinationProperty.serializedObject.targetObject as IPropertyProvider;
            var destinationType = destinationObject
                                  .GetProperty(link.DestinationProperty.stringValue).PropertyType;

            var shortcuts =
                GetShortcuts(
                    link.DestinationProperty.serializedObject.targetObject as MonoBehaviour,
                    destinationType).ToArray();

            var shortcutIndex = EditorGUI.Popup(sourceDropdownRect, -1, shortcuts
                                                                        .Select(shortcut =>
                                                                                    shortcut.Label)
                                                                        .ToArray());

            if (shortcutIndex >= 0)
            {
                var shortcut = shortcuts[shortcutIndex];
                link.SourceComponent.objectReferenceValue = shortcut.Source;
                link.SourceProperty.stringValue = shortcut.Name;
            }

            var sourceComponentRect = new Rect(rect.x,
                                               rect.y,
                                               rect.width - sourceFieldWidth - 4,
                                               rect.height);
            var sourceFieldRect = new Rect(rect.xMax - sourceFieldWidth - 4,
                                           rect.y,
                                           sourceFieldWidth,
                                           rect.height);


            EditorGUI.PropertyField(sourceComponentRect, link.SourceComponent, GUIContent.none);

            if (link.SourceComponent.objectReferenceValue is IPropertyProvider sourceProvider)
            {
                var fields = GetPropertiesOfType(sourceProvider, destinationType).ToArray();

                if (fields.Contains(link.SourceProperty.stringValue))
                    GUI.color = Color.green;
                else if (sourceProvider.CanDynamicallyProvideProperty(link.SourceProperty.stringValue,
                                                           destinationType))
                    GUI.color = Color.yellow;
                else
                    GUI.color = Color.red;

                EditorGUI.PropertyField(sourceFieldRect, link.SourceProperty, GUIContent.none);

                GUI.color = Color.white;
            }
        }
    }
}