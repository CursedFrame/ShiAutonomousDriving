using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GleyTrafficSystem
{
    public class DrawRoadConnectors : Editor
    {
        static RoadColors roadColors;
        static GUIStyle style;

        private Road selectedRoad;
        private int selectedindex;
        private bool inConnectorsActive;

        static DrawRoadConnectors instance;
        public static DrawRoadConnectors Initialize()
        {
            if (instance == null)
            {
                instance = CreateInstance<DrawRoadConnectors>();
                roadColors = SettingsLoader.LoadRoadColors();
            }
            style = new GUIStyle();
            return instance;
        }


        public void MakeCnnections(List<Road> allRoads, List<ConnectionPool> allConnections, Color connectorLaneColor, Color anchorPointColor, Color roadConnectorColor, Color selectedRoadConnectorColor, Color disconnectedColor, float waypointDistance, Color textColor)
        {
            for (int i = 0; i < allRoads.Count; i++)
            {
                DrawConnectors(allRoads[i], roadConnectorColor, selectedRoadConnectorColor, disconnectedColor, waypointDistance);
            }

            for (int i = 0; i < allConnections.Count; i++)
            {
                DrawlaneConnections(allConnections[i], connectorLaneColor, anchorPointColor, textColor);
            }
        }


        private void DrawlaneConnections(ConnectionPool connections, Color connectorLaneColor, Color anchorPointColor, Color textColor)
        {
            int nrOfLaneConnections = connections.GetNrOfConnections();
            for (int j = 0; j < nrOfLaneConnections; j++)
            {
                if (connections.connectionCurves[j].draw == true)
                {
                    DrawConnectionLane(connections, j, connectorLaneColor, anchorPointColor, textColor);
                }
                if (connections.connectionCurves[j].drawWaypoints == true)
                {
                    Transform holder = connections.transform.Find(connections.GetName(j));
                    LaneDrawer.DrawLane(holder, false, roadColors.waypointColor, roadColors.disconnectedColor, roadColors.laneChangeColor);
                }
            }
        }


        private void DrawConnectionLane(ConnectionPool connections, int connectionNumber, Color connectorLaneColor, Color anchorPointColor, Color textColor)
        {
            Path curve = connections.GetCurve(connectionNumber);
            for (int i = 0; i < curve.NumSegments; i++)
            {
                Vector3[] points = curve.GetPointsInSegment(i, connections.GetOffset(connectionNumber));
                Handles.color = Color.black;
                Handles.DrawLine(points[1], points[0]);
                Handles.DrawLine(points[2], points[3]);
                Handles.DrawBezier(points[0], points[3], points[1], points[2], connectorLaneColor, null, 2);
            }

            style.normal.textColor = textColor;
            Handles.Label(connections.GetOutConnector(connectionNumber).gameObject.transform.position, connections.GetName(connectionNumber), style);

            for (int i = 0; i < curve.NumPoints; i++)
            {
                if (i % 3 != 0)
                {
                    float handleSize = Customizations.GetAnchorPointSize(SceneView.lastActiveSceneView.camera.transform.position, curve.GetPoint(i, connections.GetOffset(connectionNumber)));
                    Handles.color = anchorPointColor;
                    Vector3 newPos = curve.GetPoint(i, connections.GetOffset(connectionNumber));

                    newPos = Handles.FreeMoveHandle(curve.GetPoint(i, connections.GetOffset(connectionNumber)), Quaternion.identity, handleSize, Vector2.zero, Handles.SphereHandleCap);
                    newPos.y = curve.GetPoint(i, connections.GetOffset(connectionNumber)).y;
                    if (curve.GetPoint(i, connections.GetOffset(connectionNumber)) != newPos)
                    {
                        Undo.RecordObject(connections, "Move point");
                        curve.MovePoint(i, newPos - connections.GetOffset(connectionNumber));
                    }
                }
            }
        }


        private void DrawConnectors(Road road, Color roadConnectorColor, Color selectedRoadConnectorColor, Color disconnectedColor, float waypointDistance)
        {
            float size;
            for (int i = 0; i < road.lanes.Count; i++)
            {
                if (road.lanes[i].laneEdges.inConnector == null || road.lanes[i].laneEdges.outConnector == null)
                {
                    Debug.LogWarning("Road " + road.name + " is not correctly generated. Please regenerate it from Traffic System Settings -> Road Setup -> Edit ->" + road.name);
                    return;
                }
                if (!inConnectorsActive)
                {
                    size = Customizations.GetRoadConnectorSize(SceneView.lastActiveSceneView.camera.transform.position, road.lanes[i].laneEdges.outConnector.transform.position);

                    if (road.lanes[i].laneEdges.outConnector.neighbors.Count == 0)
                    {
                        Handles.color = disconnectedColor;
                    }
                    else
                    {
                        Handles.color = roadConnectorColor;
                    }
                    if (Handles.Button(road.lanes[i].laneEdges.outConnector.transform.position, Quaternion.LookRotation(Camera.current.transform.forward, Camera.current.transform.up), size, size, Handles.DotHandleCap))
                    {
                        selectedRoad = road;
                        selectedindex = i;
                        inConnectorsActive = true;
                    }
                }
                else
                {
                    if (i == selectedindex && selectedRoad == road)
                    {
                        size = Customizations.GetRoadConnectorSize(SceneView.lastActiveSceneView.camera.transform.position, road.lanes[i].laneEdges.outConnector.transform.position);
                        Handles.color = selectedRoadConnectorColor;
                        if (Handles.Button(road.lanes[i].laneEdges.outConnector.transform.position, Quaternion.LookRotation(Camera.current.transform.forward, Camera.current.transform.up), size, size, Handles.DotHandleCap))
                        {
                            inConnectorsActive = false;
                        }
                    }

                    size = Customizations.GetRoadConnectorSize(SceneView.lastActiveSceneView.camera.transform.position, road.lanes[i].laneEdges.inConnector.transform.position);

                    if (road.lanes[i].laneEdges.inConnector.prev.Count == 0)
                    {
                        Handles.color = disconnectedColor;
                    }
                    else
                    {
                        Handles.color = roadConnectorColor;
                    }
                    if (Handles.Button(road.lanes[i].laneEdges.inConnector.transform.position, Quaternion.LookRotation(Camera.current.transform.forward, Camera.current.transform.up), size, size, Handles.DotHandleCap))
                    {
                        RoadConnections.Initialize().MakeConnection(road.transform.parent.GetComponent<ConnectionPool>(), selectedRoad, selectedindex, road, i, waypointDistance);
                        inConnectorsActive = false;
                        selectedRoad = null;
                        SettingsWindow.Refresh();
                    }
                }
            }
        }
    }
}
