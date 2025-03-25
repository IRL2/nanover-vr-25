using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Nanover.Core;
using Nanover.Core.Science;
using Nanover.Visualisation.Components;
using Nanover.Visualisation.Property;
using UnityEditor;
using UnityEngine;

namespace Nanover.Visualisation.Editor
{
    /// <summary>
    /// Draws a <see cref="Property" /> in the Editor.
    /// </summary>
    [CustomPropertyDrawer(typeof(Property.Property), true)]
    public sealed class VisualisationPropertyDrawer : PropertyDrawer
    {
        /// <summary>
        /// Callback for overriding this property drawer. Returns true if this specific
        /// property was overriden.
        /// </summary>
        public delegate bool OnGuiOverride(ref Rect position,
                                           SerializedProperty property,
                                           GUIContent label);

        private static readonly List<OnGuiOverride> overrides = new List<OnGuiOverride>();

        /// <summary>
        /// Add an override for drawing properties, for example to allow linking in the
        /// Editor for Visualisation Components
        /// </summary>
        public static void AddOverride(OnGuiOverride guiOverride)
        {
            overrides.Add(guiOverride);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Prefix the name of the property
            position = EditorGUI.PrefixLabel(position, label);

            // Check if any overrides apply here
            if (overrides.Any(overrideOnGui => overrideOnGui(ref position, property, label)))
            {
                return;
            }

            var valueProperty = GetValueProperty(property);
            var objectProperty = GetUnityObjectProperty(property);

            if (objectProperty == null && (valueProperty == null ||
                                           (valueProperty.isArray &&
                                            valueProperty.type != "string")))
            {
                // Don't draw arrays in the editor.
                EditorGUI.HelpBox(position, "Set array input from within code.", MessageType.None);
            }
            else
            {
                var valueRect = position;

                var isProvidedSerializedProperty = GetIsProvidedProperty(property);

                valueRect.xMin -= 15f;
                var togglePosition = new Rect(valueRect);
                valueRect.xMin += 20f;

                var isValueProvided = isProvidedSerializedProperty.boolValue;

                if (isValueProvided)
                {
                    DrawValueGui(property, valueRect);
                }
                else
                {
                    DrawMissingValueGui(property, valueRect);
                }

                EditorGUI.PropertyField(togglePosition, isProvidedSerializedProperty,
                                        GUIContent.none);
            }
        }

        /// <summary>
        /// Draw a box indicating a missing value, indicating if the requirement is
        /// required
        /// </summary>
        private void DrawMissingValueGui(SerializedProperty property, Rect rect)
        {
            var field = GetField(property);
            if (field?.GetCustomAttribute<RequiredPropertyAttribute>() != null)
            {
                EditorGUI.HelpBox(rect, "Missing input!", MessageType.Error);
            }
            else
            {
                EditorGUI.HelpBox(rect, "No input", MessageType.None);
            }
        }

        /// <summary>
        /// Draw the value of the property in the given rect
        /// </summary>
        private void DrawValueGui(SerializedProperty property, Rect rect)
        {
            var valueSerializedProperty = GetValueProperty(property);
            var objectSerializedProperty = GetUnityObjectProperty(property);

            // Draw a property field, wrapping in a ChangeCheck to 
            // check if its been changed, and dirty the underlying
            // field if so
            EditorGUI.BeginChangeCheck();
            if (valueSerializedProperty != null)
                EditorGUI.PropertyField(rect, valueSerializedProperty, GUIContent.none, true);
            else if (objectSerializedProperty != null)
                EditorGUI.PropertyField(rect, objectSerializedProperty, GUIContent.none, true);

            if (EditorGUI.EndChangeCheck())
            {
                // Get the underlying field as a Property
                var obj = GetVisualisationBaseObject(property.serializedObject.targetObject);
                var field = obj.GetType()
                               .GetFieldInSelfOrParents(property.name, BindingFlags.Instance
                                                                     | BindingFlags.Public
                                                                     | BindingFlags.NonPublic)
                               ?.GetValue(obj) as Property.Property;

                field?.MarkValueAsChanged();
            }
        }

        private static object GetVisualisationBaseObject(Object src)
        {
            if (src is VisualisationComponent component)
                return component.GetWrappedVisualisationNode();
            return src;
        }

        /// <summary>
        /// For a given <see cref="SerializedProperty" />, get the underlying
        /// <see cref="FieldInfo" />.
        /// </summary>
        private static FieldInfo GetField(SerializedProperty property)
        {
            // TODO: Support nested objects
            return property.serializedObject
                           .targetObject
                           .GetType()
                           .GetFieldInSelfOrParents(property.propertyPath,
                                                    BindingFlags.Public
                                                  | BindingFlags.Instance
                                                  | BindingFlags.NonPublic);
        }

        /// <summary>
        /// Get the <see cref="SerializedProperty" /> representing the internal input
        /// value.
        /// </summary>
        private SerializedProperty GetValueProperty(SerializedProperty inputProperty)
        {
            return inputProperty.FindPropertyRelative("value");
        }

        private SerializedProperty GetUnityObjectProperty(SerializedProperty inputProperty)
        {
            return inputProperty.FindPropertyRelative("unityObject");
        }

        /// <summary>
        /// Get the <see cref="SerializedProperty" /> representing the internal input
        /// value.
        /// </summary>
        private SerializedProperty GetIsProvidedProperty(SerializedProperty inputProperty)
        {
            return inputProperty.FindPropertyRelative("isValueProvided");
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var provided = GetIsProvidedProperty(property);
            if (provided.boolValue)
            {
                var valueProperty = GetValueProperty(property);
                if (valueProperty != null)
                    return EditorGUI.GetPropertyHeight(valueProperty);
                var objectProperty = GetUnityObjectProperty(property);
                if (objectProperty != null)
                    return EditorGUI.GetPropertyHeight(objectProperty);
            }

            return base.GetPropertyHeight(property, label);
        }
    }
}