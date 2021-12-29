#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;

namespace GleyTrafficSystem
{
    //Stores all road connections
    public class ConnectionPool : MonoBehaviour
    {
        public List<ConnectionCurve> connectionCurves;

        public void AddConnection(WaypointSettings startPosition, WaypointSettings endPosition, Road fromRoad, int fromIndex, Road toRoad, int toIndex, Vector3 offset)
        {
            if (connectionCurves == null)
            {
                connectionCurves = new List<ConnectionCurve>();
            }
            Path newConnection = new Path(startPosition.transform.position - offset, endPosition.transform.position - offset);
            string name = fromRoad.name + "_" + fromIndex + "->" + toRoad.name + "_" + toIndex;
            Transform connectorsHolder = new GameObject(name).transform;
            connectorsHolder.SetParent(transform);
            connectorsHolder.name = name;
            connectorsHolder.gameObject.tag = Constants.editorTag;
            connectionCurves.Add(new ConnectionCurve(newConnection, fromRoad, fromIndex, toRoad, toIndex, true, connectorsHolder));
        }


        public ConnectionCurve GetLaneConnection(int index)
        {
            return connectionCurves[index];
        }


        public void RemoveConnection(ConnectionCurve curve)
        {
            connectionCurves.Remove(curve);
        }


        public int GetNrOfConnections()
        {
            if (connectionCurves != null)
            {
                return connectionCurves.Count;
            }
            return 0;
        }


        public Path GetCurve(int lane)
        {
            return connectionCurves[lane].curve;
        }


        public int GetLane(int index)
        {
            return connectionCurves[index].fromIndex;
        }


        public Vector3 GetOffset(int index)
        {
            return connectionCurves[index].fromRoad.positionOffset;
        }


        public string GetName(int index)
        {
            return connectionCurves[index].name;
        }


        public Transform GetHolder(int index)
        {
            return connectionCurves[index].holder;
        }


        public WaypointSettings GetInConnector(int lane)
        {
            return connectionCurves[lane].toRoad.lanes[connectionCurves[lane].toIndex].laneEdges.inConnector;

        }


        public Transform GetOriginRoad(int index)
        {
            return connectionCurves[index].fromRoad.transform;
        }


        public WaypointSettings GetOutConnector(int index)
        {
            return connectionCurves[index].fromRoad.lanes[connectionCurves[index].fromIndex].laneEdges.outConnector;
        }


        public List<ConnectionCurve> ContainsRoad(Road road)
        {
            List<ConnectionCurve> curves = new List<ConnectionCurve>();
            for (int i = 0; i < connectionCurves.Count; i++)
            {
                if (connectionCurves[i].toRoad == road || connectionCurves[i].fromRoad == road)
                {
                    curves.Add(connectionCurves[i]);
                }
            }
            return curves;
        }
    }
}
#endif
