using UnityEditor;
using UnityEngine;

namespace GleyTrafficSystem
{
    public class EditRoadWindow : SetupWindowBase
    {
        const float minValue = 323;
        const float maxValue = 449;

        private bool[] allowedCarIndex;
        private Road selectedRoad;
        private RoadDrawer roadDrawer;
        private EditRoadSave save;
        private RoadColors roadColors;
        private float scrollAdjustment;
        private int nrOfCars;
        private bool showCustomizations;


        public override ISetupWindow Initialize(WindowProperties windowProperties)
        {
            selectedRoad = NavigationRuntimeData.GetSelectedRoad();
            roadDrawer = RoadDrawer.Initialize();
            save = SettingsLoader.LoadEditRoadSave();
            roadColors = SettingsLoader.LoadRoadColors();
            nrOfCars = System.Enum.GetValues(typeof(VehicleTypes)).Length;
            allowedCarIndex = new bool[nrOfCars];
            for (int i = 0; i < allowedCarIndex.Length; i++)
            {
                if (save.globalCarList.Contains((VehicleTypes)i))
                {
                    allowedCarIndex[i] = true;
                }
            }
            return base.Initialize(windowProperties);
        }


        public override void DrawInScene()
        {
            if (selectedRoad == null)
            {
                Debug.LogWarning("No road selected");
                return;
            }

            roadDrawer.DrawPath(selectedRoad, save.moveTool, roadColors.roadColor, roadColors.anchorPointColor, roadColors.controlPointColor, roadColors.textColor);
            LaneDrawer.DrawAllLanes(selectedRoad, save.viewRoadsSettings.viewWaypoints, save.viewRoadsSettings.viewLaneChanges, roadColors.laneColor,
                roadColors.waypointColor, roadColors.disconnectedColor, roadColors.laneChangeColor, roadColors.textColor);
            base.DrawInScene();
        }


        public override void MouseMove(Vector3 mousePosition)
        {
            if (selectedRoad == null)
            {
                Debug.LogWarning("No road selected");
                return;
            }
            base.MouseMove(mousePosition);
            roadDrawer.SelectSegmentIndex(selectedRoad, mousePosition);
        }


        public override void LeftClick(Vector3 mousePosition)
        {
            if (selectedRoad == null)
            {
                Debug.LogWarning("No road selected");
                return;
            }

            roadDrawer.AddPathPoint(mousePosition, selectedRoad);
            base.LeftClick(mousePosition);
        }


        public override void RightClick(Vector3 mousePosition)
        {
            if (selectedRoad == null)
            {
                Debug.LogWarning("No road selected");
                return;
            }
            roadDrawer.Delete(selectedRoad, mousePosition);
            base.RightClick(mousePosition);
        }


        protected override void TopPart()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Press SHIFT + Left Click to add a road point");
            EditorGUILayout.LabelField("Press SHIFT + Right Click to remove a road point");
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginHorizontal();
            save.viewRoadsSettings.viewWaypoints = EditorGUILayout.Toggle("View Waypoints", save.viewRoadsSettings.viewWaypoints);
            save.viewRoadsSettings.viewLaneChanges = EditorGUILayout.Toggle("View Lane Changes", save.viewRoadsSettings.viewLaneChanges);
            EditorGUILayout.EndHorizontal();

            EditorGUI.BeginChangeCheck();
            selectedRoad.nrOfLanes = EditorGUILayout.IntField("Nr of lanes", selectedRoad.nrOfLanes);
            EditorGUI.EndChangeCheck();
            if (GUI.changed)
            {
                selectedRoad.UpdateLaneNumber(save.maxSpeed);
            }

            selectedRoad.laneWidth = EditorGUILayout.FloatField("Lane width (m)", selectedRoad.laneWidth);
            selectedRoad.waypointDistance = EditorGUILayout.FloatField("Waypoint distance ", selectedRoad.waypointDistance);

            EditorGUI.BeginChangeCheck();
            save.moveTool = (MoveTools)EditorGUILayout.EnumPopup("Select move tool ", save.moveTool);

            showCustomizations = EditorGUILayout.Toggle("Change Colors ", showCustomizations);
            if (showCustomizations == true)
            {
                scrollAdjustment = maxValue;
                roadColors.roadColor = EditorGUILayout.ColorField("Road Color", roadColors.roadColor);
                roadColors.laneColor = EditorGUILayout.ColorField("Lane Color", roadColors.laneColor);
                roadColors.waypointColor = EditorGUILayout.ColorField("Waypoint Color", roadColors.waypointColor);
                roadColors.disconnectedColor = EditorGUILayout.ColorField("Disconnected Color", roadColors.disconnectedColor);
                roadColors.laneChangeColor = EditorGUILayout.ColorField("Lane Change Color", roadColors.laneChangeColor);
                roadColors.controlPointColor = EditorGUILayout.ColorField("Control Point Color", roadColors.controlPointColor);
                roadColors.anchorPointColor = EditorGUILayout.ColorField("Anchor Point Color", roadColors.anchorPointColor);
            }
            else
            {
                scrollAdjustment = minValue;
            }
            EditorGUI.EndChangeCheck();
            if (GUI.changed)
            {
                SceneView.RepaintAll();
            }

            base.TopPart();
        }


        protected override void ScrollPart(float width, float height)
        {
            if (selectedRoad == null)
            {
                Debug.LogWarning("No road selected");
                return;
            }
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false, GUILayout.Width(width - SCROLL_SPACE), GUILayout.Height(height - scrollAdjustment));

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Global Lane Settings", EditorStyles.boldLabel);
            SelectAllowedCars();
            EditorGUILayout.BeginHorizontal();
            save.maxSpeed = EditorGUILayout.IntField("Global Max Speed", save.maxSpeed);
            if (GUILayout.Button("Apply Speed"))
            {
                SetSpeedOnLanes(selectedRoad, save.maxSpeed);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            if (GUILayout.Button("Apply All Settings"))
            {
                SetSpeedOnLanes(selectedRoad, save.maxSpeed);
                ApplyGlobalCarSettings();
            }
            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();


            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Individual Lane Settings", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            LoadLanes();
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
            GUILayout.EndScrollView();

            base.ScrollPart(width, height);
        }


        protected override void BottomPart()
        {
            if (selectedRoad == null)
            {
                Debug.LogWarning("No road selected");
                return;
            }
            
            if (GUILayout.Button("Generate waypoints"))
            {
                save.viewRoadsSettings.viewWaypoints = true;

                if (selectedRoad.nrOfLanes <= 0)
                {
                    Debug.LogError("Nr of lanes has to be >0");
                    return;
                }

                if (selectedRoad.waypointDistance <= 0)
                {
                    Debug.LogError("Waypoint distance needs to be >0");
                    return;
                }

                if (selectedRoad.laneWidth <= 0)
                {
                    Debug.LogError("Lane width has to be >0");
                    return;
                }

                WaypointsGenerator.GenerateWaypoints(selectedRoad);
                EditorUtility.SetDirty(selectedRoad);
                AssetDatabase.SaveAssets();
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Link other lanes"))
            {
                save.viewRoadsSettings.viewWaypoints = true;
                save.viewRoadsSettings.viewLaneChanges = true;
                LinkOtherLanes.Link(selectedRoad);
                EditorUtility.SetDirty(selectedRoad);
                AssetDatabase.SaveAssets();
            }
            if (GUILayout.Button("Unlink other lanes"))
            {
                LinkOtherLanes.Unlinck(selectedRoad);
                EditorUtility.SetDirty(selectedRoad);
                AssetDatabase.SaveAssets();
            }
            EditorGUILayout.EndHorizontal();

            base.BottomPart();
        }


        public override void DestroyWindow()
        {
            SettingsLoader.SaveEditRoadSettings(save, allowedCarIndex, roadColors, new RoadDefaults(selectedRoad.nrOfLanes, selectedRoad.laneWidth, selectedRoad.waypointDistance));
            base.DestroyWindow();
        }


        private void SelectAllowedCars()
        {
            GUILayout.Label("Allowed Car Types:");
            for (int i = 0; i < nrOfCars; i++)
            {
                allowedCarIndex[i] = EditorGUILayout.Toggle(((VehicleTypes)i).ToString(), allowedCarIndex[i]);
            }
            if (GUILayout.Button("Apply Global Car Settings"))
            {
                ApplyGlobalCarSettings();
            }
        }


        private void ApplyGlobalCarSettings()
        {
            for (int i = 0; i < selectedRoad.lanes.Count; i++)
            {
                for (int j = 0; j < allowedCarIndex.Length; j++)
                {
                    selectedRoad.lanes[i].allowedCars[j] = allowedCarIndex[j];
                }
            }
        }


        private void LoadLanes()
        {
            if (selectedRoad)
            {
                if (selectedRoad.lanes != null)
                {
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    for (int i = 0; i < selectedRoad.lanes.Count; i++)
                    {
                        if (selectedRoad.lanes[i].laneDirection == true)
                        {
                            DrawLaneButton(i);
                        }
                    }
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.Space();
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    for (int i = 0; i < selectedRoad.lanes.Count; i++)
                    {
                        if (selectedRoad.lanes[i].laneDirection == false)
                        {
                            DrawLaneButton(i);
                        }
                    }
                    EditorGUILayout.EndVertical();
                }
            }
        }


        private void SetSpeedOnLanes(Road selectedRoad, int maxSpeed)
        {
            for (int i = 0; i < selectedRoad.lanes.Count; i++)
            {
                selectedRoad.lanes[i].laneSpeed = maxSpeed;
            }
        }


        private void DrawLaneButton(int currentLane)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.BeginHorizontal();
            selectedRoad.lanes[currentLane].laneSpeed = EditorGUILayout.IntField("Lane " + currentLane + ", Lane Speed:", selectedRoad.lanes[currentLane].laneSpeed);
            string buttonLebel = "<--";
            if (selectedRoad.lanes[currentLane].laneDirection == false)
            {
                buttonLebel = "-->";
            }
            if (GUILayout.Button(buttonLebel))
            {
                selectedRoad.lanes[currentLane].laneDirection = !selectedRoad.lanes[currentLane].laneDirection;
                WaypointsGenerator.SwitchLaneDirection(selectedRoad, currentLane);
            }
            GUILayout.EndHorizontal();

            EditorGUILayout.LabelField("Allowed Car Types on this lane:");
            for (int i = 0; i < nrOfCars; i++)
            {
                if (i >= selectedRoad.lanes[currentLane].allowedCars.Length)
                {
                    selectedRoad.lanes[currentLane].UpdateAllowedCars(nrOfCars);
                }
                selectedRoad.lanes[currentLane].allowedCars[i] = EditorGUILayout.Toggle(((VehicleTypes)i).ToString(), selectedRoad.lanes[currentLane].allowedCars[i]);
            }
            EditorGUILayout.EndVertical();
        }
    }
}
