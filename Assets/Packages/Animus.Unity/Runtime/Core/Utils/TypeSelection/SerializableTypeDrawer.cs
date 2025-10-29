﻿using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Packages.Animus.Unity.Runtime.Core.Utils.TypeSelection
{
    [CustomPropertyDrawer(typeof(SerializableType))]
    public class SerializableTypeDrawer : PropertyDrawer
    {
        private TypeFilterAttribute _typeFilter;
        private string[] _typeNames, _typeFullNames;

        private void Initialize()
        {
            if (_typeFullNames != null) return;

            _typeFilter = (TypeFilterAttribute)Attribute.GetCustomAttribute(fieldInfo, typeof(TypeFilterAttribute));

            var filteredTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(t => _typeFilter == null ? DefaultFilter(t) : _typeFilter.Filter(t))
                .ToArray();

            _typeNames = filteredTypes.Select(t => t.ReflectedType == null ? t.Name : $"t.ReflectedType.Name + t.Name")
                .ToArray();
            _typeFullNames = filteredTypes.Select(t => t.AssemblyQualifiedName).ToArray();
        }

        private static bool DefaultFilter(Type type)
        {
            return !type.IsAbstract && !type.IsInterface && !type.IsGenericType;
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
            var selectedIndex = EditorGUI.Popup(position, label.text, currentIndex, _typeNames);

            if (selectedIndex < 0 || selectedIndex == currentIndex) return;

            typeIdProperty.stringValue = _typeFullNames[selectedIndex];
            property.serializedObject.ApplyModifiedProperties();
        }
    }
}