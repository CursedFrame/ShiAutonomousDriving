using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GleyTrafficSystem
{
    public class ShowWaypointsBase : SetupWindowBase
    {
        protected List<WaypointSettings> waypointsOfInterest;
        protected ViewWaypointsSettings save;
        protected RoadColors roadColors;
        protected float scrollAdjustment = 220;
        private bool waypointsLoaded = false;


        public override ISetupWindow Initialize(WindowProperties windowProperties)
        {
            WaypointDrawer.Initialize();
            WaypointDrawer.onWaypointClicked += WaipointClicked;
            roadColors = SettingsLoader.LoadRoadColors();
            return base.Initialize(windowProperties);
        }


        protected virtual void WaipointClicked(WaypointSettings clickedWaypoint, bool leftClick)
        {
            SettingsWindow.SetActiveWindow(WindowType.EditWaypoint, true);
        }


        protected override void TopPart()
        {
            base.TopPart();
            EditorGUI.BeginChangeCheck();
            roadColors.waypointColor = EditorGUILayout.ColorField("Waypoint Color ", roadColors.waypointColor);

            EditorGUILayout.BeginHorizontal();
            save.showConnections = EditorGUILayout.Toggle("Show Connections", save.showConnections);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            save.showOtherLanes = EditorGUILayout.Toggle("Show Lane Change", save.showOtherLanes);
            roadColors.laneChangeColor = EditorGUILayout.ColorField(roadColors.laneChangeColor);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            save.showSpeed = EditorGUILayout.Toggle("Show Speed", save.showSpeed);
            roadColors.speedColor = EditorGUILayout.ColorField(roadColors.speedColor);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            save.showCars = EditorGUILayout.Toggle("Show Cars", save.showCars);
            roadColors.carsColor = EditorGUILayout.ColorField(roadColors.carsColor);
            EditorGUILayout.EndHorizontal();

            EditorGUI.EndChangeCheck();
            if (GUI.changed)
            {
                SceneView.RepaintAll();
            }
        }


        protected override void ScrollPart(float width, float height)
        {
            if (waypointsOfInterest != null)
            {
                if (waypointsOfInterest.Count == 0)
                {
                    EditorGUILayout.LabelField("No " + GetWindowTitle());
                }
                for (int i = 0; i < waypointsOfInterest.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                    EditorGUILayout.LabelField(waypointsOfInterest[i].name);
                    if (GUILayout.Button("View", GUILayout.Width(BUTTON_DIMENSION)))
                    {
                        GleyUtilities.TeleportSceneCamera(waypointsOfInterest[i].transform.position);
                        SceneView.RepaintAll();
                    }
                    if (GUILayout.Button("Edit", GUILayout.Width(BUTTON_DIMENSION)))
                    {
                        NavigationRuntimeData.SetSelectedWaypoint(waypointsOfInterest[i]);
                        GleyUtilities.TeleportSceneCamera(waypointsOfInterest[i].transform.position);
                        SettingsWindow.SetActiveWindow(WindowType.EditWaypoint, true);
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
            else
            {
                EditorGUILayout.LabelField("No " + GetWindowTitle());
            }
            base.ScrollPart(width, height);
        }


        public override void DrawInScene()
        {
            if (waypointsLoaded == false)
            {
                SettingsWindow.Refresh();
                waypointsLoaded = true;
            }
            base.DrawInScene();
        }


        public override void DestroyWindow()
        {
            WaypointDrawer.onWaypointClicked -= WaipointClicked;
            base.DestroyWindow();
        }
    }
}
