using UnityEditor;
using UnityEngine;

namespace GleyTrafficSystem
{
    public class RoadSetupWindow : SetupWindowBase
    {
        public override ISetupWindow Initialize(WindowProperties windowProperties)
        {
            base.Initialize(windowProperties);
            return this;
        }


        protected override void TopPart()
        {
            base.TopPart();
            EditorGUILayout.LabelField("Select action:");
            EditorGUILayout.Space();

            if (GUILayout.Button("Create Road"))
            {
                SettingsWindow.SetActiveWindow(WindowType.CreateRoad, true);
            }
            EditorGUILayout.Space();

            if (GUILayout.Button("Connect Roads"))
            {
                SettingsWindow.SetActiveWindow(WindowType.ConnectRoads, true);
            }
            EditorGUILayout.Space();

            if (GUILayout.Button("View Roads"))
            {
                SettingsWindow.SetActiveWindow(WindowType.ViewRoads, true);
            }
        }
    }
}