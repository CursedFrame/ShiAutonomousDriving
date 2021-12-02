using System.IO;
using UnityEditor;
using UnityEngine;

namespace GleyTrafficSystem
{
    public class MainMenuWindow : SetupWindowBase
    {
        private const int scrollAdjustment = 103;


        public override ISetupWindow Initialize(WindowProperties windowProperties)
        {
            return base.Initialize(windowProperties);
        }


        protected override void ScrollPart(float width, float height)
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false, GUILayout.Width(width - SCROLL_SPACE), GUILayout.Height(height - scrollAdjustment));

            EditorGUILayout.Space();

            if (GUILayout.Button("Import Required Packages"))
            {
                SettingsWindow.SetActiveWindow(WindowType.ImportPackages, true);
            }
            EditorGUILayout.Space();

            if (GUILayout.Button("Scene Setup"))
            {
                SettingsWindow.SetActiveWindow(WindowType.SceneSetup, true);
            }
            EditorGUILayout.Space();

            if (GUILayout.Button("Road Setup"))
            {
                SettingsWindow.SetActiveWindow(WindowType.RoadSetup, true);
            }
            EditorGUILayout.Space();

            if (GUILayout.Button("Intersection Setup"))
            {
                SettingsWindow.SetActiveWindow(WindowType.IntersectionSetup, true);
            }
            EditorGUILayout.Space();

            if (GUILayout.Button("Waypoint Setup"))
            {
                SettingsWindow.SetActiveWindow(WindowType.WaypointSetup, true);
            }
            EditorGUILayout.Space();

            if (GUILayout.Button("Speed Routes Setup"))
            {
                SettingsWindow.SetActiveWindow(WindowType.SpeedRoutesSetupWindow, true);
            }
            EditorGUILayout.Space();

            if (GUILayout.Button("Vehicle Routes Setup"))
            {
                SettingsWindow.SetActiveWindow(WindowType.CarRoutesSetupWindow, true);
            }
            EditorGUILayout.Space();

            if (GUILayout.Button("External Tools"))
            {
                SettingsWindow.SetActiveWindow(WindowType.ExternalTools, true);
            }
            EditorGUILayout.Space();

            if (GUILayout.Button("Debug"))
            {
                SettingsWindow.SetActiveWindow(WindowType.Debug, true);
            }
            EditorGUILayout.Space();

            EditorGUILayout.EndScrollView();
            base.ScrollPart(width, height);
        }


        protected override void BottomPart()
        {
            if (GUILayout.Button("Apply Settings"))
            {
                if (LayerOperations.LoadOrCreateLayers().edited == false)
                {
                    Debug.LogWarning("Layers are not configured. Go to Window->Gley->Traffic System->Scene Setup->Layer Setup");
                }

                if (GridEditor.AssignWaypoints(CurrentSceneData.GetSceneInstance())==false)
                {
                    return;
                }

                if (!File.Exists(Application.dataPath + "/GleyPlugins/TrafficSystem/Resources/VehicleTypes.cs"))
                {
                    FileCreator.CreateVehicleTypesFile(null);
                }

                Gley.Common.PreprocessorDirective.AddToCurrent(Gley.Common.Constants.USE_GLEY_TRAFFIC, false);
            }
            EditorGUILayout.Space();

            base.BottomPart();
        }
    }
}
