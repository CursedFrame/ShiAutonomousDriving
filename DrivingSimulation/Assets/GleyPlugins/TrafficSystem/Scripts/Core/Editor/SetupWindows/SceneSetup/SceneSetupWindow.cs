using UnityEditor;
using UnityEngine;

namespace GleyTrafficSystem
{
    public class SceneSetupWindow : SetupWindowBase
    {
        protected override void ScrollPart(float width, float height)
        {
            EditorGUILayout.LabelField("Select action:");
            EditorGUILayout.Space();

            if (GUILayout.Button("Layer Setup"))
            {
                SettingsWindow.SetActiveWindow(WindowType.LayerSetupWindow, true);
            }
            EditorGUILayout.Space();


            if (GUILayout.Button("Grid Setup"))
            {
                SettingsWindow.SetActiveWindow(WindowType.GridSetupWindow, true);
            }
            EditorGUILayout.Space();

            if (GUILayout.Button("Car Type Setup"))
            {
                SettingsWindow.SetActiveWindow(WindowType.CarTypes, true);
            }
            EditorGUILayout.Space();

            base.ScrollPart(width, height);
        }
    }
}
