using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GleyTrafficSystem
{
    public class EditWaypointWindow : SetupWindowBase
    {
        struct CarDisplay
        {
            public Color color;
            public VehicleTypes car;
            public bool active;
            public bool view;

            public CarDisplay(bool active, VehicleTypes car, Color color)
            {
                this.active = active;
                this.car = car;
                this.color = color;
                view = false;
            }
        }

        enum ListToAdd
        {
            None,
            Neighbors,
            OtherLanes
        }


        private CarDisplay[] carDisplay;
        private RoadColors roadColors;
        private WaypointSettings selectedWaypoint;
        private WaypointSettings clickedWaypoint;
        private ListToAdd selectedList;
        private float scrollAdjustment = 223;
        private int maxSpeed;
        private int nrOfCars;


        public override ISetupWindow Initialize(WindowProperties windowProperties)
        {
            WaypointDrawer.onWaypointClicked += WaypointClicked;
            selectedWaypoint = NavigationRuntimeData.GetSelectedWaypoint();
            maxSpeed = selectedWaypoint.maxSpeed;
            WaypointDrawer.Initialize();
            nrOfCars = System.Enum.GetValues(typeof(VehicleTypes)).Length;
            roadColors = SettingsLoader.LoadRoadColors();

            carDisplay = new CarDisplay[nrOfCars];
            for (int i = 0; i < nrOfCars; i++)
            {
                carDisplay[i] = new CarDisplay(selectedWaypoint.allowedCars.Contains((VehicleTypes)i), (VehicleTypes)i, Color.white);
            }

            return base.Initialize(windowProperties);
        }


        public override void DrawInScene()
        {
            base.DrawInScene();

            if (selectedList != ListToAdd.None)
            {
                WaypointDrawer.DrawWaypointsForLink(selectedWaypoint, selectedWaypoint.neighbors, selectedWaypoint.otherLanes, roadColors.waypointColor, roadColors.speedColor);
            }

            WaypointDrawer.DrawCurrentWaypoint(selectedWaypoint, roadColors.selectedWaypointColor, roadColors.waypointColor, roadColors.laneChangeColor, roadColors.prevWaypointColor);

            for (int i = 0; i < carDisplay.Length; i++)
            {
                if (carDisplay[i].view)
                {
                    WaypointDrawer.DrawWaypointsForCar(carDisplay[i].car, carDisplay[i].color);
                }
            }

            if (clickedWaypoint)
            {
                WaypointDrawer.DrawSelectedWaypoint(clickedWaypoint, roadColors.selectedRoadConnectorColor);
            }
        }


        protected override void TopPart()
        {
            base.TopPart();
            EditorGUI.BeginChangeCheck();
            roadColors.selectedWaypointColor = EditorGUILayout.ColorField("Selected Color ", roadColors.selectedWaypointColor);
            roadColors.waypointColor = EditorGUILayout.ColorField("Neighbor Color ", roadColors.waypointColor);
            roadColors.laneChangeColor = EditorGUILayout.ColorField("Lane Change Color ", roadColors.laneChangeColor);
            roadColors.prevWaypointColor = EditorGUILayout.ColorField("Previous Color ", roadColors.prevWaypointColor);

            EditorGUI.EndChangeCheck();
            if (GUI.changed)
            {
                SceneView.RepaintAll();
            }

            if (GUILayout.Button("Select Waypoint"))
            {
                Selection.activeGameObject = selectedWaypoint.gameObject;
            }

            base.TopPart();
        }


        protected override void ScrollPart(float width, float height)
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false, GUILayout.Width(width - SCROLL_SPACE), GUILayout.Height(height - scrollAdjustment));
            EditorGUI.BeginChangeCheck();
            if (selectedList == ListToAdd.None)
            {
                selectedWaypoint.giveWay = EditorGUILayout.Toggle(new GUIContent("Give Way", "Vehicle will stop when reaching this waypoint and check if next waypoint is free before continuing"), selectedWaypoint.giveWay);
                maxSpeed = EditorGUILayout.IntField(new GUIContent("Max speed", "The maximum speed allowed in this waypoint"), maxSpeed);
                if (GUILayout.Button("Set Speed"))
                {
                    if (maxSpeed != 0)
                    {
                        selectedWaypoint.speedLocked = true;
                    }
                    else
                    {
                        selectedWaypoint.speedLocked = false;
                    }
                    SetSpeed();
                }

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField(new GUIContent("Allowed vehicles: ", "Only the following vehicles can pass through this waypoint"), EditorStyles.boldLabel);
                EditorGUILayout.Space();

                for (int i = 0; i < nrOfCars; i++)
                {
                    EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                    carDisplay[i].active = EditorGUILayout.Toggle(carDisplay[i].active, GUILayout.MaxWidth(20));
                    EditorGUILayout.LabelField(((VehicleTypes)i).ToString());
                    carDisplay[i].color = EditorGUILayout.ColorField(carDisplay[i].color, GUILayout.MaxWidth(80));
                    Color oldColor = GUI.backgroundColor;
                    if (carDisplay[i].view)
                    {
                        GUI.backgroundColor = Color.green;
                    }
                    if (GUILayout.Button("View", GUILayout.MaxWidth(64)))
                    {
                        carDisplay[i].view = !carDisplay[i].view;
                    }
                    GUI.backgroundColor = oldColor;

                    EditorGUILayout.EndHorizontal();
                }
                if (GUILayout.Button("Set cars"))
                {
                    SetCars();
                }

                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();
            }

            EditorGUILayout.Space();
            MakeListOperations("Neighbors", "From this waypoint a vehicle can continue to the following ones", selectedWaypoint.neighbors, ListToAdd.Neighbors);
            EditorGUILayout.Space();
            MakeListOperations("Other Lanes", "Connections to other lanes, used for overtaking", selectedWaypoint.otherLanes, ListToAdd.OtherLanes);

            EditorGUI.EndChangeCheck();
            if (GUI.changed)
            {
                SceneView.RepaintAll();
            }

            base.ScrollPart(width, height);
            GUILayout.EndScrollView();
        }


        public override void DestroyWindow()
        {
            if (selectedWaypoint)
            {
                EditorUtility.SetDirty(selectedWaypoint);
            }
            SettingsLoader.SaveRoadColors(roadColors);
            WaypointDrawer.onWaypointClicked -= WaypointClicked;
            base.DestroyWindow();
        }


        private void SetSpeed()
        {
            List<WaypointSettings> waypointList = new List<WaypointSettings>();
            selectedWaypoint.maxSpeed = maxSpeed;
            SetSpeed(waypointList, selectedWaypoint.maxSpeed, selectedWaypoint.neighbors);
        }


        private void SetCars()
        {
            List<VehicleTypes> result = new List<VehicleTypes>();
            for (int i = 0; i < carDisplay.Length; i++)
            {
                if (carDisplay[i].active)
                {
                    result.Add(carDisplay[i].car);
                }
            }
            selectedWaypoint.allowedCars = result;
            if (result.Count > 0)
            {
                selectedWaypoint.carsLocked = true;
            }
            else
            {
                selectedWaypoint.carsLocked = false;
            }
            List<WaypointSettings> waypointList = new List<WaypointSettings>();
            SetCarType(waypointList, selectedWaypoint.allowedCars, selectedWaypoint.neighbors);
        }


        private void SetSpeed(List<WaypointSettings> waypointList, int speed, List<WaypointSettings> neighbors)
        {
            if (speed == 0)
            {
                return;
            }

            for (int i = 0; i < neighbors.Count; i++)
            {
                if (!waypointList.Contains(neighbors[i]))
                {
                    if (!neighbors[i].speedLocked)
                    {
                        waypointList.Add(neighbors[i]);
                        neighbors[i].maxSpeed = speed;
                        EditorUtility.SetDirty(neighbors[i]);
                        SetSpeed(waypointList, speed, neighbors[i].neighbors);
                    }
                }
            }
        }


        private void SetCarType(List<WaypointSettings> waypointList, List<VehicleTypes> carTypes, List<WaypointSettings> neighbors)
        {
            if (carTypes == null || carTypes.Count == 0)
            {
                return;
            }

            for (int i = 0; i < neighbors.Count; i++)
            {
                if (!waypointList.Contains(neighbors[i]))
                {
                    if (!neighbors[i].carsLocked)
                    {
                        waypointList.Add(neighbors[i]);
                        neighbors[i].allowedCars = carTypes;
                        EditorUtility.SetDirty(neighbors[i]);
                        SetCarType(waypointList, carTypes, neighbors[i].neighbors);
                    }
                }
            }
        }


        private void MakeListOperations(string title, string description, List<WaypointSettings> listToEdit, ListToAdd listType)
        {
            if (selectedList == listType || selectedList == ListToAdd.None)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField(new GUIContent(title, description), EditorStyles.boldLabel);
                EditorGUILayout.Space();
                for (int i = 0; i < listToEdit.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label(listToEdit[i].name);
                    Color oldColor = GUI.backgroundColor;
                    if (listToEdit[i] == clickedWaypoint)
                    {
                        GUI.backgroundColor = Color.green;
                    }
                    if (GUILayout.Button("View", GUILayout.MaxWidth(64)))
                    {
                        if (listToEdit[i] == clickedWaypoint)
                        {
                            clickedWaypoint = null;
                        }
                        else
                        {
                            ViewWaypoint(listToEdit[i]);
                        }
                    }
                    GUI.backgroundColor = oldColor;
                    if (GUILayout.Button("Delete", GUILayout.MaxWidth(64)))
                    {
                        DeleteWaypoint(listToEdit[i], listType == ListToAdd.OtherLanes);
                    }

                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.Space();
                if (selectedList == ListToAdd.None)
                {
                    if (GUILayout.Button("Add/Remove " + title))
                    {
                        WaypointDrawer.Initialize();
                        selectedList = listType;
                    }
                }
                else
                {
                    if (GUILayout.Button("Done"))
                    {
                        selectedList = ListToAdd.None;
                        SceneView.RepaintAll();
                    }
                }

                EditorGUILayout.Space();
                EditorGUILayout.EndVertical();
            }
        }


        private void WaypointClicked(WaypointSettings clickedWaypoint, bool leftClick)
        {
            if (leftClick)
            {
                if (selectedList == ListToAdd.Neighbors)
                {
                    AddNeighbor(clickedWaypoint);
                }

                if (selectedList == ListToAdd.OtherLanes)
                {
                    AddOtherLanes(clickedWaypoint);
                }

                if (selectedList == ListToAdd.None)
                {
                    SettingsWindow.SetActiveWindow(WindowType.EditWaypoint, false);
                }
            }
            SettingsWindow.Refresh();
        }


        private void ViewWaypoint(WaypointSettings waypoint)
        {
            clickedWaypoint = waypoint;
            GleyUtilities.TeleportSceneCamera(waypoint.transform.position);
        }


        private void DeleteWaypoint(WaypointSettings waypoint, bool other)
        {
            if (!other)
            {
                waypoint.prev.Remove(selectedWaypoint);
                selectedWaypoint.neighbors.Remove(waypoint);
            }
            else
            {
                selectedWaypoint.otherLanes.Remove(waypoint);
            }
            clickedWaypoint = null;
            SceneView.RepaintAll();
        }


        private void AddNeighbor(WaypointSettings neighbor)
        {
            if (!selectedWaypoint.neighbors.Contains(neighbor))
            {
                selectedWaypoint.neighbors.Add(neighbor);
                neighbor.prev.Add(selectedWaypoint);
            }
        }


        private void AddOtherLanes(WaypointSettings waypoint)
        {
            if (!selectedWaypoint.otherLanes.Contains(waypoint))
            {
                selectedWaypoint.otherLanes.Add(waypoint);
            }
        }
    }
}
