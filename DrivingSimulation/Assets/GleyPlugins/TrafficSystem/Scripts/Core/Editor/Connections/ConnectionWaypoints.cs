using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GleyTrafficSystem
{
    public class ConnectionWaypoints : Editor
    {
        public static void GenerateConnectorWaypoints(ConnectionPool connections, int index, float waypointDistance)
        {
            RemoveConnectionWaipoints(connections.GetHolder(index));
            AddLaneConnectionWaypoints(connections, index, waypointDistance);
            EditorUtility.SetDirty(connections);
            AssetDatabase.SaveAssets();
        }


        public static void RemoveConnectionHolder(Transform holder)
        {
            RemoveConnectionWaipoints(holder);
            GleyPrefabUtilities.DestroyTransform(holder);
        }


        private static void RemoveConnectionWaipoints(Transform holder)
        {
            if (holder)
            {
                for (int i = holder.childCount - 1; i >= 0; i--)
                {
                    WaypointSettings waypoint = holder.GetChild(i).GetComponent<WaypointSettings>();
                    for (int j = 0; j < waypoint.neighbors.Count; j++)
                    {
                        if (waypoint.neighbors[j] != null)
                        {
                            waypoint.neighbors[j].prev.Remove(waypoint);
                        }
                        else
                        {
                            Debug.LogError(waypoint.name + " has null neighbors", waypoint);
                        }
                    }
                    for (int j = 0; j < waypoint.prev.Count; j++)
                    {
                        if (waypoint.prev[j] != null)
                        {
                            waypoint.prev[j].neighbors.Remove(waypoint);
                        }
                        else
                        {
                            Debug.LogError(waypoint.name + " has null prevs", waypoint);
                        }
                    }

                    DestroyImmediate(waypoint.gameObject);
                }
            }
        }


        private static void AddLaneConnectionWaypoints(ConnectionPool connections, int index, float waypointDistance)
        {
            string roadName = connections.GetOriginRoad(index).name;
            List<VehicleTypes> allowedCars = connections.GetOutConnector(index).allowedCars;
            int maxSpeed = connections.GetOutConnector(index).maxSpeed;

            Path curve = connections.GetCurve(index);

            Vector3[] p = curve.GetPointsInSegment(0, connections.GetOffset(index));
            float estimatedCurveLength = Vector3.Distance(p[0], p[3]);
            float nrOfWaypoints = estimatedCurveLength / waypointDistance;
            if (nrOfWaypoints < 1.5f)
            {
                nrOfWaypoints = 1.5f;
            }
            float step = 1 / nrOfWaypoints;
            float t = 0;
            int nr = 0;
            List<Transform> connectorWaypoints = new List<Transform>();
            while (t < 1)
            {
                t += step;
                if (t < 1)
                {
                    string waypointName = roadName + "-" + Constants.laneNamePrefix + connections.GetLane(index) + "-" + Constants.connectionWaypointName + (++nr);
                    connectorWaypoints.Add(WaypointsGenerator.CreateWaypoint(connections.GetHolder(index), BezierCurve.CalculateCubicBezierPoint(t, p[0], p[1], p[2], p[3]), waypointName, allowedCars, maxSpeed, connections.GetLaneConnection(index)));
                }
            }


            WaypointSettings currentWaypoint;
            WaypointSettings connectedWaypoint;

            //set names
            connectorWaypoints[0].name = roadName + "-" + Constants.laneNamePrefix + connections.GetLane(index) + "-" + Constants.connectionEdgeName + nr;
            connectorWaypoints[connectorWaypoints.Count - 1].name = roadName + "-" + Constants.laneNamePrefix + connections.GetLane(index) + "-" + Constants.connectionEdgeName + (connectorWaypoints.Count - 1);

            //link middle waypoints
            for (int j = 0; j < connectorWaypoints.Count - 1; j++)
            {
                currentWaypoint = connectorWaypoints[j].GetComponent<WaypointSettings>();
                connectedWaypoint = connectorWaypoints[j + 1].GetComponent<WaypointSettings>();
                currentWaypoint.neighbors.Add(connectedWaypoint);
                connectedWaypoint.prev.Add(currentWaypoint);
            }

            //link first waypoint
            connectedWaypoint = connectorWaypoints[0].GetComponent<WaypointSettings>();
            currentWaypoint = connections.GetOutConnector(index);
            currentWaypoint.neighbors.Add(connectedWaypoint);
            connectedWaypoint.prev.Add(currentWaypoint);
            EditorUtility.SetDirty(currentWaypoint);
            EditorUtility.SetDirty(connectedWaypoint);

            //link last waypoint
            connectedWaypoint = connections.GetInConnector(index);
            currentWaypoint = connectorWaypoints[connectorWaypoints.Count - 1].GetComponent<WaypointSettings>();
            currentWaypoint.neighbors.Add(connectedWaypoint);
            connectedWaypoint.prev.Add(currentWaypoint);
            EditorUtility.SetDirty(currentWaypoint);
            EditorUtility.SetDirty(connectedWaypoint);
        }
    }
}