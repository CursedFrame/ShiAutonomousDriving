using UnityEditor;
using UnityEngine;

namespace GleyTrafficSystem
{
    public class WaypointSetupWindow : SetupWindowBase
    {
        protected override void ScrollPart(float width, float height)
        {
            EditorGUILayout.LabelField("Select action:");
            EditorGUILayout.Space();

            if (GUILayout.Button("Show All Waypoints"))
            {
                SettingsWindow.SetActiveWindow(WindowType.ShowAllWaypoints, true);
               
            }
            EditorGUILayout.Space();

            if (GUILayout.Button("Show Disconnected Waypoints"))
            {
                SettingsWindow.SetActiveWindow(WindowType.ShowDisconnectedWaypoints, true);
            }
            EditorGUILayout.Space();

            if (GUILayout.Button("Show Vehicle Edited Waypoints"))
            {
                SettingsWindow.SetActiveWindow(WindowType.ShowCarTypeEditedWaypoints, true);
            }
            EditorGUILayout.Space();

            if (GUILayout.Button("Show Speed Edited Waypoints"))
            {
                SettingsWindow.SetActiveWindow(WindowType.ShowSpeedEditedWaypoints, true);
            }
            EditorGUILayout.Space();

            if (GUILayout.Button("Show Give Way Waypoints"))
            {
                SettingsWindow.SetActiveWindow(WindowType.ShowGiveWayWaypoints, true);
            }
            EditorGUILayout.Space();

            if (GUILayout.Button("Show Stop Waypoints"))
            {
                SettingsWindow.SetActiveWindow(WindowType.ShowStopWaypoints, true);
            }
            EditorGUILayout.Space();

            if (GUILayout.Button("Show Vehicle Path Problems"))
            {
                SettingsWindow.SetActiveWindow(WindowType.ShowVehiclePathProblems, true);
            }
            EditorGUILayout.Space();

            base.ScrollPart(width, height);
        }
    }
}
