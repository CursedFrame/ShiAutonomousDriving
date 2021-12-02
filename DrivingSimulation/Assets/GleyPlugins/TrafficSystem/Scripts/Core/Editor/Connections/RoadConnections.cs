using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GleyTrafficSystem
{
    public class RoadConnections : Editor
    {
        static List<ConnectionPool> connectionPools;
        static RoadConnections instance;


        public static RoadConnections Initialize()
        {
            if (instance == null)
            {
                instance = CreateInstance<RoadConnections>();
                LoadAllConnections();
            }

            return instance;
        }


        public List<ConnectionPool> ConnectionPools
        {
            get
            {
                return connectionPools;
            }
        }


        public void MakeConnection(ConnectionPool connectionPool, Road fromRoad, int fromIndex, Road toRoad, int toIndex, float waypointDistance)
        {
            Vector3 offset = Vector3.zero;
            if (!GleyPrefabUtilities.EditingInsidePrefab())
            {
                if (GleyPrefabUtilities.IsInsidePrefab(fromRoad.gameObject) && GleyPrefabUtilities.GetInstancePrefabRoot(fromRoad.gameObject) == GleyPrefabUtilities.GetInstancePrefabRoot(toRoad.gameObject))
                {
                    connectionPool = RoadCreator.GetRoadWaypointsHolder().GetComponent<ConnectionPool>();
                    offset = fromRoad.positionOffset;
                }
                else
                {
                    connectionPool = RoadCreator.GetRoadWaypointsHolder().GetComponent<ConnectionPool>();
                    offset = fromRoad.positionOffset;
                }
            }
            connectionPool.AddConnection(fromRoad.lanes[fromIndex].laneEdges.outConnector, toRoad.lanes[toIndex].laneEdges.inConnector, fromRoad, fromIndex, toRoad, toIndex, offset);

            ConnectionWaypoints.GenerateConnectorWaypoints(connectionPool, connectionPool.connectionCurves.Count - 1, waypointDistance);

            EditorUtility.SetDirty(connectionPool);
            AssetDatabase.SaveAssets();
            LoadAllConnections();
        }


        public void DeleteConnection(ConnectionCurve connectingCurve)
        {
            ConnectionWaypoints.RemoveConnectionHolder(connectingCurve.holder);
            for (int i = 0; i < ConnectionPools.Count; i++)
            {
                if (ConnectionPools[i].connectionCurves != null)
                {
                    if (ConnectionPools[i].connectionCurves.Contains(connectingCurve))
                    {
                        ConnectionPools[i].RemoveConnection(connectingCurve);
                        EditorUtility.SetDirty(ConnectionPools[i]);
                    }
                }
            }
            AssetDatabase.SaveAssets();
        }


        public void GenerateSelectedConnections(float waypointDistance)
        {
            for (int i = 0; i < ConnectionPools.Count; i++)
            {
                int nrOfConnections = ConnectionPools[i].GetNrOfConnections();
                for (int j = 0; j < nrOfConnections; j++)
                {
                    if (ConnectionPools[i].connectionCurves[j].draw)
                    {
                        ConnectionWaypoints.GenerateConnectorWaypoints(ConnectionPools[i], j, waypointDistance);
                    }
                }
            }
        }


        private static void LoadAllConnections()
        {
            connectionPools = new List<ConnectionPool>();
            List<Road> allRoads = RoadsLoader.Initialize().LoadAllRoads();
            for (int i = 0; i < allRoads.Count; i++)
            {
                ConnectionPool connectionsScript = allRoads[i].transform.parent.GetComponent<ConnectionPool>();
                if (!connectionPools.Contains(connectionsScript))
                {
                    connectionPools.Add(connectionsScript);
                }
            }
        }
    }
}
