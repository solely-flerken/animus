using UnityEditor;
using UnityEngine;

namespace EditorTools.WallGenerator.Editor
{
    [CustomEditor(typeof(WallGenerator))]
    public class WallGeneratorEditorEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var generator = (WallGenerator)target;

            GUILayout.Space(10);

            EditorGUILayout.LabelField("Actions", EditorStyles.boldLabel);
            
            if (GUILayout.Button("Generate Wall"))
            {
                generator.GenerateWall();
            }

            if (GUILayout.Button("Clear Wall"))
            {
                generator.ClearWall();
            }
        }
    }
}