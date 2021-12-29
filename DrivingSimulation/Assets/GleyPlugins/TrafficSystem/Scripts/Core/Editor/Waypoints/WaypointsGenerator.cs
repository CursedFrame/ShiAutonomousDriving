using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GleyTrafficSystem
{
    public class WaypointsGenerator : Editor
    {
        public static Transform CreateWaypoint(Transform parent, Vector3 waypointPosition, string name, List<VehicleTypes> allowedCars, int maxSpeed, ConnectionCurve connection)
        {
            GameObject go = new GameObject();
            go.transform.SetParent(parent);
            go.transform.position = waypointPosition;
            go.name = name;
            go.tag = Constants.editorTag;
            WaypointSettings waypointScript = go.AddComponent<WaypointSettings>();
            waypointScript.allowedCars = allowedCars;
            waypointScript.maxSpeed = maxSpeed;
            waypointScript.EditorSetup();
            waypointScript.connection = connection;
            return go.transform;
        }


        public static void GenerateWaypoints(Road road)
        {
            ClearOldWaypointConnections(road.transform);

            GleyPrefabUtilities.ClearAllChildObjects(road.transform);

            List<Transform> helpingPoints = SplitBezierIntoPoints.CreatePoints(road);

            AddFinalWaypoints(road, helpingPoints);

            LinkNeighbors(road);

            for (int i = 0; i < road.lanes.Count; i++)
            {
                if (road.lanes[i].laneDirection == true)
                {
                    SwitchLaneDirection(road, i);
                }
            }

            DeleteHelpingPoints(helpingPoints);
            GleyPrefabUtilities.ApplyPrefabInstance(road.gameObject);
        }


        public static void SwitchLaneDirection(Road road, int laneNumber)
        {
            RemoveConnection(road.lanes[laneNumber].laneEdges.inConnector);
            RemoveConnection(road.lanes[laneNumber].laneEdges.outConnector);
            SwitchWaypointDirection(road.lanes[laneNumber].laneEdges.inConnector, road.lanes[laneNumber].laneEdges.outConnector);
            road.SwitchDirection(laneNumber);
        }


        private static void ClearOldWaypointConnections(Transform holder)
        {
            WaypointSettings[] allWaypoints = holder.GetComponentsInChildren<WaypointSettings>();
            for (int i = 0; i < allWaypoints.Length; i++)
            {
                WaypointSettings waypoint = allWaypoints[i];
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
            }
        }


        private static void AddFinalWaypoints(Road road, List<Transform> helpingPoints)
        {
            float startPosition;
            if (road.nrOfLanes % 2 == 0)
            {
                startPosition = -road.laneWidth / 2;
            }
            else
            {
                startPosition = 0;
            }

            int laneModifier = 0;

            Transform lanesHolder = new GameObject(Constants.lanesHolderName).transform;
            lanesHolder.SetParent(road.transform, false);

            for (int i = 0; i < road.nrOfLanes; i++)
            {
                Transform laneHolder = AddLaneHolder(lanesHolder, Constants.laneNamePrefix + i);
                if (i % 2 == 0)
                {
                    laneModifier = -laneModifier;
                }
                else
                {
                    laneModifier = Mathf.Abs(laneModifier) + 1;
                }

                List<Transform> finalPoints = new List<Transform>();
                string waypointName;
                Vector3 waypointPosition;

                for (int j = 0; j < helpingPoints.Count - 1; j++)
                {
                    waypointPosition = helpingPoints[j].position + (startPosition + laneModifier * road.laneWidth) * helpingPoints[j].right;
                    if (PositionIsValid(helpingPoints, waypointPosition, Mathf.Abs(startPosition + laneModifier * road.laneWidth) - 0.1f))
                    {
                        waypointPosition = PutWaypointOnRoad(waypointPosition, helpingPoints[j].up);
                        if (PositionIsValid(finalPoints, waypointPosition, road.waypointDistance))
                        {
                            waypointName = road.name + "-" + Constants.laneNamePrefix + i + "-" + Constants.waypointNamePrefix + j;
                            finalPoints.Add(CreateWaypoint(laneHolder, waypointPosition, waypointName, road.GetAllowedCars(i), road.lanes[i].laneSpeed, null));
                        }
                    }
                }

                //add last point from the list
                waypointPosition = helpingPoints[helpingPoints.Count - 1].position + (startPosition + laneModifier * road.laneWidth) * helpingPoints[helpingPoints.Count - 1].right;
                waypointPosition = PutWaypointOnRoad(waypointPosition, helpingPoints[helpingPoints.Count - 1].up);
                waypointName = road.name + "-" + Constants.laneNamePrefix + i + "-" + Constants.waypointNamePrefix + helpingPoints.Count;
                finalPoints.Add(CreateWaypoint(laneHolder, waypointPosition, waypointName, road.GetAllowedCars(i), road.lanes[i].laneSpeed, null));

                finalPoints[0].name = road.name + "-" + Constants.laneNamePrefix + i + "-WConnect-" + 0;
                finalPoints[finalPoints.Count - 1].name = road.name + "-" + Constants.laneNamePrefix + i + "-WConnect-" + (helpingPoints.Count - 1);
                road.AddLaneConnector(finalPoints[0].GetComponent<WaypointSettings>(), finalPoints[finalPoints.Count - 1].GetComponent<WaypointSettings>(), i);
            }
        }


        private static void LinkNeighbors(Road road)
        {
            for (int i = 0; i < road.nrOfLanes; i++)
            {
                Transform laneHolder = road.transform.Find(Constants.lanesHolderName).Find(Constants.laneNamePrefix + i);
                WaypointSettings previousWaypoint = laneHolder.GetChild(0).GetComponent<WaypointSettings>();
                for (int j = 1; j < laneHolder.childCount; j++)
                {
                    string waypointName = laneHolder.GetChild(j).name;
                    WaypointSettings waypointScript = laneHolder.GetChild(j).GetComponent<WaypointSettings>();
                    if (previousWaypoint != null)
                    {
                        previousWaypoint.neighbors.Add(waypointScript);
                        waypointScript.prev.Add(previousWaypoint);
                    }

                    if (!waypointName.Contains("Output"))
                    {
                        previousWaypoint = waypointScript;
                    }
                    else
                    {
                        previousWaypoint = null;
                    }
                }
            }
        }


        private static void DeleteHelpingPoints(List<Transform> helpingPoints)
        {
            DestroyImmediate(helpingPoints[0].transform.parent.gameObject);
        }


        private static void RemoveConnection(WaypointSettings waypoint)
        {
            for (int i = 0; i < waypoint.neighbors.Count; i++)
            {
                if (waypoint.neighbors[i].connection != null)
                {
                    RoadConnections.Initialize().DeleteConnection(waypoint.neighbors[i].connection);
                }
            }


            for (int i = 0; i < waypoint.prev.Count; i++)
            {
                if (waypoint.prev[i].connection != null)
                {
                    RoadConnections.Initialize().DeleteConnection(waypoint.prev[i].connection);
                }
            }
        }


        private static void SwitchWaypointDirection(WaypointSettings startWaypoint, WaypointSettings endWaypoint)
        {
            WaypointSettings currentWaypoint = startWaypoint;
            bool continueSwitching = true;
            while (continueSwitching)
            {
                if (currentWaypoint == null)
                {
                    break;
                }
                if (currentWaypoint.neighbors == null)
                {
                    break;
                }

                if (currentWaypoint == endWaypoint)
                {
                    continueSwitching = false;
                }

                for (int i = currentWaypoint.neighbors.Count - 1; i >= 1; i--)
                {
                    currentWaypoint.neighbors[i].prev.Remove(currentWaypoint);
                    currentWaypoint.neighbors.RemoveAt(i);
                }

                for (int i = currentWaypoint.prev.Count - 1; i >= 1; i--)
                {
                    currentWaypoint.prev[i].neighbors.Remove(currentWaypoint);
                    currentWaypoint.prev.RemoveAt(i);
                }

                List<WaypointSettings> aux = currentWaypoint.neighbors;
                currentWaypoint.neighbors = currentWaypoint.prev;
                currentWaypoint.prev = aux;              
                if (currentWaypoint.prev.Count > 0)
                {
                    currentWaypoint = currentWaypoint.prev[0];
                }
            }
        }


        private static Vector3 PutWaypointOnRoad(Vector3 waypointPosition, Vector3 perpendicular)
        {
            if (GleyPrefabUtilities.EditingInsidePrefab())
            {
                if (GleyPrefabUtilities.GetScenePrefabRoot().scene.GetPhysicsScene().Raycast(waypointPosition + 5 * perpendicular, -perpendicular, out RaycastHit hitInfo, Mathf.Infinity, NavigationRuntimeData.GetRoadLayers()))
                {
                    return hitInfo.point;
                }
            }
            else
            {
                if (Physics.Raycast(waypointPosition + 5 * perpendicular, -perpendicular, out RaycastHit hitInfo, Mathf.Infinity, NavigationRuntimeData.GetRoadLayers()))
                {
                    return hitInfo.point;
                }
            }
            return waypointPosition;
        }


        private static bool PositionIsValid(List<Transform> helpingPoints, Vector3 waypointPosition, float limit)
        {
            for (int i = 0; i < helpingPoints.Count; i++)
            {
                if (Vector3.Distance(helpingPoints[i].position, waypointPosition) < limit)
                {
                    return false;
                }
            }
            return true;
        }


        private static Transform AddLaneHolder(Transform parent, string laneName)
        {
            GameObject lane = new GameObject(laneName);
            lane.transform.SetParent(parent, false);
            return lane.transform;
        }
    }
}
