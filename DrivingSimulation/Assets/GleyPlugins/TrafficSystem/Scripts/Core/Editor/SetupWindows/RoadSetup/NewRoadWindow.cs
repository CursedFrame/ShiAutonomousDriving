using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GleyTrafficSystem
{
    public class NewRoadWindow : SetupWindowBase
    {
        private Vector3 firstClick;
        private Vector3 secondClick;
        private CreateRoadSave save;
        private RoadColors roadColors;
        private RoadDrawer roadDrawer;
        private List<Road> allRoads;


        public override ISetupWindow Initialize(WindowProperties windowProperties)
        {
            firstClick = secondClick = Vector3.zero;
            save = SettingsLoader.LoadCreateRoadSave();
            roadColors = SettingsLoader.LoadRoadColors();
            roadDrawer = RoadDrawer.Initialize();
            allRoads = RoadsLoader.Initialize().LoadAllRoads();

            return base.Initialize(windowProperties);
        }


        protected override void TopPart()
        {
            base.TopPart();
            EditorGUILayout.LabelField("Press SHIFT + Left Click to add a road point");
            EditorGUILayout.LabelField("Press SHIFT + Right Click to remove a road point");
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("If you are not able to draw, make sure your ground/road is on the layer marked as Road inside Layer Setup");
        }


        protected override void ScrollPart(float width, float height)
        {
            EditorGUI.BeginChangeCheck();
            roadColors.textColor = EditorGUILayout.ColorField("Text Color", roadColors.textColor);

            EditorGUILayout.BeginHorizontal();
            save.viewOtherRoads = EditorGUILayout.Toggle("View Other Roads", save.viewOtherRoads, GUILayout.Width(TOGGLE_WIDTH));
            roadColors.roadColor = EditorGUILayout.ColorField(roadColors.roadColor);
            EditorGUILayout.EndHorizontal();

            if (save.viewOtherRoads)
            {
                EditorGUILayout.BeginHorizontal();
                save.viewRoadsSettings.viewLanes = EditorGUILayout.Toggle("View Lanes", save.viewRoadsSettings.viewLanes, GUILayout.Width(TOGGLE_WIDTH));
                roadColors.laneColor = EditorGUILayout.ColorField(roadColors.laneColor);
                EditorGUILayout.EndHorizontal();

                if (save.viewRoadsSettings.viewLanes)
                {
                    EditorGUILayout.BeginHorizontal();
                    save.viewRoadsSettings.viewWaypoints = EditorGUILayout.Toggle("View Waypoints", save.viewRoadsSettings.viewWaypoints, GUILayout.Width(TOGGLE_WIDTH));
                    roadColors.waypointColor = EditorGUILayout.ColorField(roadColors.waypointColor);
                    roadColors.disconnectedColor = EditorGUILayout.ColorField(roadColors.disconnectedColor);
                    if (save.viewRoadsSettings.viewWaypoints == false)
                    {
                        save.viewRoadsSettings.viewLaneChanges = false;
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    save.viewRoadsSettings.viewLaneChanges = EditorGUILayout.Toggle("View Lane Changes", save.viewRoadsSettings.viewLaneChanges, GUILayout.Width(TOGGLE_WIDTH));
                    if (save.viewRoadsSettings.viewLaneChanges == true)
                    {
                        save.viewRoadsSettings.viewWaypoints = true;
                    }
                    roadColors.laneChangeColor = EditorGUILayout.ColorField(roadColors.laneChangeColor);
                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUI.EndChangeCheck();

            if (GUI.changed)
            {

                SceneView.RepaintAll();
            }
            base.ScrollPart(width, height);
        }


        public override void DrawInScene()
        {
            if (firstClick != Vector3.zero)
            {
                Handles.SphereHandleCap(0, firstClick, Quaternion.identity, 1, EventType.Repaint);
            }

            if (save.viewOtherRoads)
            {
                for (int i = 0; i < allRoads.Count; i++)
                {
                    roadDrawer.DrawPath(allRoads[i], MoveTools.None, roadColors.roadColor, roadColors.controlPointColor, roadColors.controlPointColor, roadColors.textColor);
                    if (save.viewRoadsSettings.viewLanes)
                    {
                        LaneDrawer.DrawAllLanes(allRoads[i], save.viewRoadsSettings.viewWaypoints, save.viewRoadsSettings.viewLaneChanges, roadColors.laneColor,
                            roadColors.waypointColor, roadColors.disconnectedColor, roadColors.laneChangeColor, roadColors.textColor);
                    }
                }
            }
            base.DrawInScene();
        }


        public override void UndoAction()
        {
            base.UndoAction();
            if (secondClick == Vector3.zero)
            {
                if (firstClick != Vector3.zero)
                {
                    firstClick = Vector3.zero;
                }
            }
        }


        public override void LeftClick(Vector3 mousePosition)
        {
            if (firstClick == Vector3.zero)
            {
                firstClick = mousePosition;
            }
            else
            {
                secondClick = mousePosition;
                CreateRoad();
            }
            base.LeftClick(mousePosition);
        }


        public override void DestroyWindow()
        {
            SettingsLoader.SaveCreateRoadSettings(save, roadColors);
            base.DestroyWindow();
        }


        private void CreateRoad()
        {
            Road selectedRoad = new RoadCreator().Create(firstClick);
            selectedRoad.CreatePath(firstClick, secondClick);
            selectedRoad.SetRoadProperties(SettingsLoader.LoadEditRoadSave().maxSpeed);
            NavigationRuntimeData.SetSelectedRoad(selectedRoad);
            SettingsWindow.SetActiveWindow(WindowType.EditRoad, false);
            firstClick = Vector3.zero;
            secondClick = Vector3.zero;
        }
    }
}
