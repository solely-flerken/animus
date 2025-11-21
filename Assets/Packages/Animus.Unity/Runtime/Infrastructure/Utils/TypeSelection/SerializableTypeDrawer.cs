using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Packages.Animus.Unity.Runtime.Infrastructure.Utils.TypeSelection
{
    [CustomPropertyDrawer(typeof(SerializableType))]
    public class SerializableTypeDrawer : PropertyDrawer
    {
        private TypeFilterAttribute _typeFilter;
        private string[] _typeNames, _typeFullNames;

        // A predefined list of common types to ensure they are always included.
        private static readonly List<Type> PredefinedTypes = new()
        {
            typeof(string),
            typeof(int),
            typeof(bool),
            typeof(float)
        };
        
        private void Initialize()
        {
            if (_typeFullNames != null) return;

            _typeFilter = (TypeFilterAttribute)Attribute.GetCustomAttribute(fieldInfo, typeof(TypeFilterAttribute));
            
            // Get types from assemblies based on the filter.
            var filteredAssemblyTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(t => _typeFilter == null ? DefaultFilter(t) : _typeFilter.Filter(t));

            // Combine the predefined types with the assembly types and remove duplicates.
            var finalTypeList = PredefinedTypes
                .Union(filteredAssemblyTypes)
                .OrderBy(t => t.Name)
                .ToList();
            
            
            _typeNames = finalTypeList.Select(PrettifyTypeName).ToArray();
            _typeFullNames = finalTypeList.Select(t => t.AssemblyQualifiedName).ToArray();
        }

        private static bool DefaultFilter(Type type)
        {
            // A default filter to exclude abstract types, interfaces, generic types, and Unity Objects.
            return !type.IsAbstract && !type.IsInterface && !type.IsGenericType;
        }
        
        /// <summary>
        /// Creates a more readable name for certain types in the dropdown.
        /// </summary>
        private static string PrettifyTypeName(Type type)
        {
            if (type == typeof(string)) return "Primitives/string";
            if (type == typeof(int)) return "Primitives/int";
            if (type == typeof(bool)) return "Primitives/bool";
            if (type == typeof(float)) return "Primitives/float";
            
            // Add prefixes for clarity in the dropdown menu
            if (PredefinedTypes.Contains(type))
            {
                return $"Common/{type.Name}";
            }
            return $"Classes/{type.Name}";
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Initialize();
            var typeIdProperty = property.FindPropertyRelative("assemblyQualifiedName");

            if (string.IsNullOrEmpty(typeIdProperty.stringValue))
            {
                typeIdProperty.stringValue = _typeFullNames.First();
                property.serializedObject.ApplyModifiedProperties();
            }

            var currentIndex = Array.IndexOf(_typeFullNames, typeIdProperty.stringValue);
            if (currentIndex < 0)
            {
                // If the currently saved type is not in our list, show -1 (nothing selected)
                currentIndex = -1; 
            }
            
            var selectedIndex = EditorGUI.Popup(position, label.text, currentIndex, _typeNames);

            if (selectedIndex < 0 || selectedIndex == currentIndex) return;

            typeIdProperty.stringValue = _typeFullNames[selectedIndex];
            property.serializedObject.ApplyModifiedProperties();
        }
    }
}