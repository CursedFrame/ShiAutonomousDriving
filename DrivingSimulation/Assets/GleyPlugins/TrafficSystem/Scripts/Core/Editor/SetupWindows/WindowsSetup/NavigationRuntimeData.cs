using System.Collections.Generic;
using UnityEngine;

namespace GleyTrafficSystem
{
    public static class NavigationRuntimeData
    {
        static List<WindowType> path;
        static Road selectedRoad;
        static WaypointSettings selectedWaypoint;
        static GenericIntersectionSettings selectedIntersection;
        static LayerMask roadLayers;


        public static Road GetSelectedRoad()
        {
            return selectedRoad;
        }


        public static void SetSelectedRoad(Road road)
        {
            selectedRoad = road;
        }


        public static WaypointSettings GetSelectedWaypoint()
        {
            return selectedWaypoint;
        }


        public static void SetSelectedWaypoint( WaypointSettings waypoint)
        {
            selectedWaypoint = waypoint;
        }


        public static GenericIntersectionSettings GetSelectedIntersection()
        {
            return selectedIntersection;
        }


        public static void SetSelectedIntersection(GenericIntersectionSettings intersection)
        {
            selectedIntersection = intersection;
        }


        public static void InitializeData()
        {
            path = new List<WindowType>();
            UpdateLayers();
            selectedRoad = null;
        }


        public static void AddWindow(WindowType newWindow)
        {
            if (!path.Contains(newWindow))
            {
                path.Add(newWindow);
            }
            else
            {
                Debug.LogWarning("Trying to add same window twice: " + newWindow);
            }
        }


        public static WindowType RemoveLastWindow()
        {
            WindowType lastWindow = path[path.Count - 1];

            path.RemoveAt(path.Count - 1);
            return lastWindow;
        }


        public static string GetBackPath()
        {
            if (path.Count == 0)
                return "";

            string result = "";
            for (int i = 0; i < path.Count; i++)
            {
                result += AllSettingsWindows.GetWindowName(path[i]) + " > ";
            }
            return result;
        }


        public static void UpdateLayers()
        {
            roadLayers = LayerOperations.LoadRoadLayers();
        }


        public static LayerMask GetRoadLayers()
        {
            return roadLayers;
        }
    }
}
