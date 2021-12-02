using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace GleyTrafficSystem
{
    public class WaypointDrawer : Editor
    {
        public delegate void WaypointClicked(WaypointSettings clickedWaypoint, bool leftClick);
        public static event WaypointClicked onWaypointClicked;
        static void TriggetWaypointClickedEvent(WaypointSettings clickedWaypoint, bool leftClick)
        {
            NavigationRuntimeData.SetSelectedWaypoint(clickedWaypoint);
            if (onWaypointClicked != null)
            {
                onWaypointClicked(clickedWaypoint, leftClick);
            }
        }

        static List<WaypointSettings> allWaypoints;
        static List<WaypointSettings> disconnectedWaypoints;
        static List<WaypointSettings> carTypeEditedWaypoints;
        static List<WaypointSettings> speedEditedWaypoints;
        static List<WaypointSettings> giveWayWaypoints;
        static List<WaypointSettings> stopWaypoints;
        static List<WaypointSettings> pathProblems;
        static GUIStyle style;


        public static void Initialize()
        {
            style = new GUIStyle();
            LoadWaypoints();
        }


        public static void DrawAllWaypoints(Color waypointColor, bool showConnections, Color connectionColor, bool showSpeed, Color speedColor, bool showCars, Color carsColor, bool drawOtherLanes, Color otherLanesColor)
        {
            for (int i = 0; i < allWaypoints.Count; i++)
            {
                DrawCompleteWaypoint(allWaypoints[i], waypointColor, showConnections, connectionColor, showSpeed, speedColor, showCars, carsColor, drawOtherLanes, otherLanesColor);
            }
        }


        public static List<WaypointSettings> ShowDisconnectedWaypoints(Color waypointColor, bool showConnections, Color connectionColor, bool showSpeed, Color speedColor, bool showCars, Color carsColor)
        {
            int nr = 0;
            for (int i = 0; i < allWaypoints.Count; i++)
            {
                if (allWaypoints[i].neighbors.Count == 0)
                {
                    nr++;
                    DrawCompleteWaypoint(allWaypoints[i], waypointColor, showConnections, connectionColor, showSpeed, speedColor, showCars, carsColor, false, Color.white);
                }
            }

            if (nr != disconnectedWaypoints.Count)
            {
                UpdateDisconnectedWaypoints();
            }
            return disconnectedWaypoints;
        }


        public static List<WaypointSettings> ShowSpeedEditedWaypoints(Color waypointColor, bool showConnections, Color connectionColor, bool showSpeed, Color speedColor, bool showCars, Color carsColor, bool drawOtherLanes, Color otherLanesColor)
        {
            int nr = 0;
            for (int i = 0; i < allWaypoints.Count; i++)
            {
                if (allWaypoints[i].speedLocked)
                {
                    nr++;
                    DrawCompleteWaypoint(allWaypoints[i], waypointColor, showConnections, connectionColor, showSpeed, speedColor, showCars, carsColor, drawOtherLanes, otherLanesColor);
                }
            }
            if (nr != speedEditedWaypoints.Count)
            {
                UpdateSpeedEditedWaypoints();
            }

            return speedEditedWaypoints;
        }


        public static List<WaypointSettings> ShowGiveWayWaypoints(Color waypointColor, bool showConnections, Color connectionColor, bool showSpeed, Color speedColor, bool showCars, Color carsColor, bool drawOtherLanes, Color otherLanesColor)
        {
            int nr = 0;
            for (int i = 0; i < allWaypoints.Count; i++)
            {
                if (allWaypoints[i].giveWay)
                {
                    nr++;
                    DrawCompleteWaypoint(allWaypoints[i], waypointColor, showConnections, connectionColor, showSpeed, speedColor, showCars, carsColor, drawOtherLanes, otherLanesColor);
                }
            }

            if (nr != giveWayWaypoints.Count)
            {
                UpdateGiveWayWaypoints();
            }

            return giveWayWaypoints;
        }


        public static List<WaypointSettings> ShowStopWaypoints(Color waypointColor, bool showConnections, Color connectionColor, bool showSpeed, Color speedColor, bool showCars, Color carsColor, bool drawOtherLanes, Color otherLanesColor)
        {
            int nr = 0;
            for (int i = 0; i < allWaypoints.Count; i++)
            {
                if (GleyUtilities.IsPointInsideView(allWaypoints[i].transform.position))
                {
                    if (allWaypoints[i].stop)
                    {
                        nr++;
                        DrawCompleteWaypoint(allWaypoints[i], waypointColor, showConnections, connectionColor, showSpeed, speedColor, showCars, carsColor, drawOtherLanes, otherLanesColor);
                    }
                }
            }

            if (nr != stopWaypoints.Count)
            {
                UpdateStopWaypoints();
            }

            return stopWaypoints;
        }


        public static List<WaypointSettings> ShowVehicleProblems(Color waypointColor, bool showConnections, Color connectionColor, bool showSpeed, Color speedColor, bool showCars, Color carsColor, bool drawOtherLanes, Color otherLanesColor)
        {
            pathProblems = new List<WaypointSettings>();
            for (int i = 0; i < allWaypoints.Count; i++)
            {
                if (GleyUtilities.IsPointInsideView(allWaypoints[i].transform.position))
                {
                    int nr = allWaypoints[i].allowedCars.Count;
                    for (int j = 0; j < allWaypoints[i].allowedCars.Count; j++)
                    {
                        for (int k = 0; k < allWaypoints[i].neighbors.Count; k++)
                        {
                            if (allWaypoints[i].neighbors[k].allowedCars.Contains(allWaypoints[i].allowedCars[j]))
                            {
                                nr--;
                                break;
                            }
                        }
                    }
                    if (nr != 0)
                    {
                        pathProblems.Add(allWaypoints[i]);
                        DrawCompleteWaypoint(allWaypoints[i], waypointColor, showConnections, connectionColor, showSpeed, speedColor, true, carsColor, drawOtherLanes, otherLanesColor);

                        for (int k = 0; k < allWaypoints[i].neighbors.Count; k++)
                        {
                            for (int j = 0; j < allWaypoints[i].neighbors[k].allowedCars.Count; j++)
                            {
                                DrawCompleteWaypoint(allWaypoints[i].neighbors[k], connectionColor, showConnections, connectionColor, showSpeed, speedColor, true, carsColor, drawOtherLanes, otherLanesColor);
                            }
                        }
                    }
                }

            }
            return pathProblems;
        }


        public static void DrawWaypointsForLink(WaypointSettings currentWaypoint, List<WaypointSettings> neighborsList, List<WaypointSettings> otherLinesList, Color waypointColor, Color speedColor)
        {
            for (int i = 0; i < allWaypoints.Count; i++)
            {
                if (allWaypoints[i] != currentWaypoint && !neighborsList.Contains(allWaypoints[i]) && !otherLinesList.Contains(allWaypoints[i]))
                {
                    DrawCompleteWaypoint(allWaypoints[i], waypointColor, true, waypointColor, true, speedColor, false, Color.white, false, Color.white);
                }
            }
        }


        public static List<int> GetDifferentSpeeds()
        {
            List<int> result = new List<int>();
            LoadWaypoints();

            for (int i = 0; i < allWaypoints.Count; i++)
            {
                if (!result.Contains(allWaypoints[i].maxSpeed))
                {
                    result.Add(allWaypoints[i].maxSpeed);
                }
            }
            return result;
        }


        public static void DrawCurrentWaypoint(WaypointSettings waypoint, Color selectedColor, Color waypointColor, Color otherLaneColor, Color prevColor)
        {
            DrawCompleteWaypoint(waypoint, selectedColor, true, waypointColor, false, Color.white, false, Color.white, true, otherLaneColor, true, prevColor);
            for (int i = 0; i < waypoint.neighbors.Count; i++)
            {
                if (waypoint.neighbors[i] != null)
                {
                    DrawCompleteWaypoint(waypoint.neighbors[i], waypointColor, false, waypointColor, false, Color.white, false, Color.white, true, otherLaneColor, false, prevColor);
                }
            }
            for (int i = 0; i < waypoint.prev.Count; i++)
            {
                if (waypoint.prev[i] != null)
                {
                    DrawCompleteWaypoint(waypoint.prev[i], prevColor, false, waypointColor, false, Color.white, false, Color.white, true, otherLaneColor, false, prevColor);
                }
            }
            for (int i = 0; i < waypoint.otherLanes.Count; i++)
            {
                if (waypoint.otherLanes != null)
                {
                    DrawCompleteWaypoint(waypoint.otherLanes[i], otherLaneColor, false, waypointColor, false, Color.white, false, Color.white, true, otherLaneColor, false, prevColor);
                }
            }
        }


        public static void DrawSelectedWaypoint(WaypointSettings selectedWaypoint, Color color)
        {
            Handles.color = color;
            Handles.CubeHandleCap(0, selectedWaypoint.transform.position, Quaternion.LookRotation(Camera.current.transform.forward, Camera.current.transform.up), 1, EventType.Repaint);
        }


        public static void ShowSpeedLimits(int speed, Color color)
        {
            if (color.a == 0)
            {
                color = Color.white;
            }
            Handles.color = color;
            if (allWaypoints == null)
            {
                LoadWaypoints();
            }

            for (int i = 0; i < allWaypoints.Count; i++)
            {
                if (GleyUtilities.IsPointInsideView(allWaypoints[i].transform.position))
                {
                    if (allWaypoints[i].maxSpeed == speed)
                    {
                        if (Handles.Button(allWaypoints[i].transform.position, Quaternion.LookRotation(Camera.current.transform.forward, Camera.current.transform.up), 0.5f, 0.5f, Handles.DotHandleCap))
                        {
                            TriggetWaypointClickedEvent(allWaypoints[i], Event.current.button == 0);
                        }
                        for (int j = 0; j < allWaypoints[i].neighbors.Count; j++)
                        {
                            Handles.DrawLine(allWaypoints[i].transform.position, allWaypoints[i].neighbors[j].transform.position);
                        }
                    }
                }
            }
        }


        public static void DrawWaypointsForCar(VehicleTypes car, Color color)
        {
            Handles.color = color;
            for (int i = 0; i < allWaypoints.Count; i++)
            {
                if (GleyUtilities.IsPointInsideView(allWaypoints[i].transform.position))
                {
                    if (allWaypoints[i].allowedCars.Contains(car))
                    {
                        if (Handles.Button(allWaypoints[i].transform.position, Quaternion.LookRotation(Camera.current.transform.forward, Camera.current.transform.up), 0.5f, 0.5f, Handles.DotHandleCap))
                        {
                            TriggetWaypointClickedEvent(allWaypoints[i], Event.current.button == 0);
                        }
                        for (int j = 0; j < allWaypoints[i].neighbors.Count; j++)
                        {
                            Handles.DrawLine(allWaypoints[i].transform.position, allWaypoints[i].neighbors[j].transform.position);
                        }
                    }
                }
            }
        }


        public static List<WaypointSettings> ShowCarTypeEditedWaypoints(Color waypointColor, bool showConnections, Color connectionColor, bool showSpeed, Color speedColor, bool showCars, Color carsColor, bool drawOtherLanes, Color otherLanesColor)
        {
            int nr = 0;
            for (int i = 0; i < allWaypoints.Count; i++)
            {
                if (allWaypoints[i].carsLocked)
                {
                    nr++;
                    DrawCompleteWaypoint(allWaypoints[i], waypointColor, showConnections, connectionColor, showSpeed, speedColor, showCars, carsColor, drawOtherLanes, otherLanesColor);
                }
            }

            if (nr != carTypeEditedWaypoints.Count)
            {
                UpdateCarTypeEditedWaypoints();
            }

            return carTypeEditedWaypoints;
        }


        private static void UpdateDisconnectedWaypoints()
        {
            disconnectedWaypoints = new List<WaypointSettings>();
            for (int i = 0; i < allWaypoints.Count; i++)
            {
                if (allWaypoints[i].neighbors != null)
                {
                    if (allWaypoints[i].neighbors.Count == 0)
                    {
                        disconnectedWaypoints.Add(allWaypoints[i]);
                    }
                    else
                    {
                        for (int j = 0; j < allWaypoints[i].neighbors.Count; j++)
                        {
                            if (allWaypoints[i].neighbors[j] == null)
                            {
                                allWaypoints[i].neighbors.RemoveAt(j);
                                disconnectedWaypoints.Add(allWaypoints[i]);
                            }
                        }
                    }
                }
                else
                {
                    allWaypoints[i].neighbors = new List<WaypointSettings>();
                    disconnectedWaypoints.Add(allWaypoints[i]);
                }
            }
        }


        private static void UpdateCarTypeEditedWaypoints()
        {
            carTypeEditedWaypoints = new List<WaypointSettings>();
            for (int i = 0; i < allWaypoints.Count; i++)
            {
                if (allWaypoints[i].carsLocked == true)
                {
                    carTypeEditedWaypoints.Add(allWaypoints[i]);
                }
            }
        }


        private static void UpdateSpeedEditedWaypoints()
        {
            speedEditedWaypoints = new List<WaypointSettings>();
            for (int i = 0; i < allWaypoints.Count; i++)
            {
                if (allWaypoints[i].speedLocked == true)
                {
                    speedEditedWaypoints.Add(allWaypoints[i]);
                }
            }
        }


        private static void UpdateGiveWayWaypoints()
        {
            giveWayWaypoints = new List<WaypointSettings>();
            for (int i = 0; i < allWaypoints.Count; i++)
            {
                if (allWaypoints[i].giveWay == true)
                {
                    giveWayWaypoints.Add(allWaypoints[i]);
                }
            }
        }


        private static void UpdateStopWaypoints()
        {
            stopWaypoints = new List<WaypointSettings>();
            for (int i = 0; i < allWaypoints.Count; i++)
            {
                if (allWaypoints[i].stop == true)
                {
                    stopWaypoints.Add(allWaypoints[i]);
                }
            }
        }


        private static void LoadWaypoints()
        {
            if (!GleyPrefabUtilities.EditingInsidePrefab())
            {
                allWaypoints = FindObjectsOfType<WaypointSettings>().ToList();
            }
            else
            {
                allWaypoints = GleyPrefabUtilities.GetScenePrefabRoot().GetComponentsInChildren<WaypointSettings>().ToList();
            }
            UpdateDisconnectedWaypoints();
            UpdateCarTypeEditedWaypoints();
            UpdateSpeedEditedWaypoints();
            UpdateGiveWayWaypoints();
            UpdateStopWaypoints();
        }


        private static void DrawCompleteWaypoint(WaypointSettings waypoint, Color color, bool showConnections, Color connectionColor, bool showSpeed, Color speedColor, bool showCars, Color carsColor, bool drawOtherLanes, Color otherLanesColor, bool drawPrev = false, Color prevColor = new Color())
        {
            if (!waypoint)
                return;
            if (GleyUtilities.IsPointInsideView(waypoint.transform.position))
            {
                DrawClickableButton(waypoint, color);
                if (showConnections)
                {
                    DrawWaypointConnections(waypoint, connectionColor, drawOtherLanes, otherLanesColor, drawPrev, prevColor);
                }
                if (showSpeed)
                {
                    ShowSpeed(waypoint, speedColor);
                }
                if (showCars)
                {
                    ShowCars(waypoint, carsColor);
                }
            }
        }


        private static void DrawClickableButton(WaypointSettings waypoint, Color color)
        {
            if (!waypoint)
                return;
            Handles.color = color;
            if (Handles.Button(waypoint.transform.position, Quaternion.LookRotation(Camera.current.transform.forward, Camera.current.transform.up), 0.5f, 0.5f, Handles.DotHandleCap))
            {
                TriggetWaypointClickedEvent(waypoint, Event.current.button == 0);
            }

        }


        private static void DrawWaypointConnections(WaypointSettings waypoint, Color color, bool drawOtherLanes, Color otherLanesColor, bool drawPrev, Color prevColor)
        {
            if (!waypoint)
                return;
            Handles.color = color;
            if (waypoint.neighbors.Count > 0)
            {
                for (int i = 0; i < waypoint.neighbors.Count; i++)
                {
                    if (waypoint.neighbors[i] != null)
                    {
                        Handles.DrawLine(waypoint.transform.position, waypoint.neighbors[i].transform.position);

                        Vector3 direction = (waypoint.transform.position - waypoint.neighbors[i].transform.position).normalized;
                        Vector3 point1 = (waypoint.transform.position + waypoint.neighbors[i].transform.position) / 2;

                        Vector3 point2 = point1 + Quaternion.Euler(0, -25, 0) * direction;

                        Vector3 point3 = point1 + Quaternion.Euler(0, 25, 0) * direction;

                        Handles.DrawPolyLine(point1, point2, point3, point1);
                    }
                    else
                    {
                        Debug.LogWarning("waypoint " + waypoint.name + " has missing neighbors", waypoint);
                    }
                }
            }

            if (drawOtherLanes)
            {
                if (waypoint.otherLanes != null)
                {
                    for (int i = 0; i < waypoint.otherLanes.Count; i++)
                    {
                        if (waypoint.otherLanes[i] != null)
                        {
                            DrawTriangle(waypoint.transform.position, waypoint.otherLanes[i].transform.position, otherLanesColor, true);
                        }
                    }
                }
            }

            if (drawPrev)
            {
                Handles.color = prevColor;
                for (int i = 0; i < waypoint.prev.Count; i++)
                {
                    if (waypoint.prev[i] != null)
                    {
                        Handles.DrawLine(waypoint.transform.position, waypoint.prev[i].transform.position);
                    }
                }
            }
        }


        private static void DrawTriangle(Vector3 start, Vector3 end, Color waypointColor, bool drawLane)
        {
            Handles.color = waypointColor;
            Vector3 direction = (start - end).normalized;

            Vector3 point2 = end + Quaternion.Euler(0, -25, 0) * direction;

            Vector3 point3 = end + Quaternion.Euler(0, 25, 0) * direction;

            Handles.DrawPolyLine(end, point2, point3, end);

            if (drawLane)
            {
                Handles.DrawLine(start, end);
            }
        }


        private static void ShowSpeed(WaypointSettings waypoint, Color color)
        {
            if (!waypoint)
                return;
            style.normal.textColor = color;
            Handles.Label(waypoint.transform.position, waypoint.maxSpeed.ToString(), style);
        }


        private static void ShowCars(WaypointSettings waypoint, Color color)
        {
            if (!waypoint)
                return;
            style.normal.textColor = color;
            string text = "";
            for (int j = 0; j < waypoint.allowedCars.Count; j++)
            {
                text += waypoint.allowedCars[j] + "\n";
            }
            Handles.Label(waypoint.transform.position, text, style);
        }
    }
}
