#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Packages.Animus.Unity.Runtime.Core.Actions
{
    [CustomEditor(typeof(ActionQueueManager))]
    public class ActionQueueManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Debug Controls", EditorStyles.boldLabel);

            var manager = (ActionQueueManager)target;

            GUI.backgroundColor = new Color(1f, 0.5f, 0.5f);
            if (GUILayout.Button("Kill Random Request", GUILayout.Height(30)))
            {
                if (Application.isPlaying)
                {
                    manager.DebugCancelRandomAgent();
                }
                else
                {
                    Debug.LogWarning("Enter Play Mode to test cancellation.");
                }
            }

            GUI.backgroundColor = Color.white;
        }
    }
}
#endif