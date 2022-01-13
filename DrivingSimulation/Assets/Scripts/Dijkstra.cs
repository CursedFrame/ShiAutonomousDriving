using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GleyTrafficSystem {
    public class Dijkstra : MonoBehaviour {
        public class Edge
        {
            public int originIndex, destinationIndex;
            public float distance;
        }
        private List<Waypoint> nodes;
        private List<Edge> edges = new List<Edge>();
        private List<Waypoint> basis, allWaypoints;
        private Dictionary<string, float> dist;
        private Dictionary<string, Waypoint> previous;
        public bool pathCalculated = false, calculatingPath = false;

        public Dijkstra(List<Waypoint> nodes) {
            this.nodes = nodes;
            basis = new List<Waypoint>();
            allWaypoints = new List<Waypoint>();
            dist = new Dictionary<string, float>();
            previous = new Dictionary<string, Waypoint>();

            foreach (Waypoint n in nodes){
                foreach (int r in n.neighbors){
                    Edge e = new Edge();
                    e.originIndex = n.listIndex;
                    e.destinationIndex = r;
                }
            }

            // record node 
            foreach (Waypoint n in nodes)
            {
                previous.Add(n.name + n.position.ToString(), null);
                basis.Add(n);
                allWaypoints.Add(n);
                dist.Add(n.name + n.position.ToString(), float.MaxValue);
            }
            Debug.Log("Finished constructing Dijkstra");
        }

        /// Calculates the shortest path from the start
        ///  to all other nodes
        public void CalculateDistance(Waypoint start) {
            calculatingPath = true;
            dist[start.name + start.position.ToString()] = 0;
            while (basis.Count > 0)
            {
                Waypoint u = GetNodeWithSmallestDistance();
                if (u == null)
                {
                    basis.Clear();
                }
                else
                {
                    foreach (Waypoint v in GetNeighbors(u))
                    {
                        float alt = dist[u.name + u.position.ToString()] + GetDistanceBetween(u, v);
                        if (alt < dist[v.name + v.position.ToString()])
                        {
                            dist[v.name + v.position.ToString()] = alt;
                            previous[v.name + v.position.ToString()] = u;
                        }
                    }
                    basis.Remove(u);
                }
            }
            calculatingPath = false;
            pathCalculated = true;
        }

        public List<Waypoint> GetPathTo(Waypoint d) {
            List<Waypoint> path = new List<Waypoint>();

            path.Insert(0, d);

            while (previous[d.name + d.position.ToString()] != null)
            {
                d = previous[d.name + d.position.ToString()];
                path.Insert(0, d);
            }

            return path;
        }

        public Waypoint GetNodeWithSmallestDistance() {
            float distance = float.MaxValue;
            Waypoint smallest = null;

            foreach (Waypoint n in basis)
            {
                if (dist[n.name + n.position.ToString()] < distance)
                {
                    distance = dist[n.name + n.position.ToString()];
                    smallest = n;
                }
            }

            return smallest;
        }


        public List<Waypoint> GetNeighbors(Waypoint n) {
            List<Waypoint> neighbors = new List<Waypoint>();

            foreach (Edge e in edges)
            {
                if (e.originIndex.Equals(n.listIndex) && basis.Contains(n))
                {
                    neighbors.Add(allWaypoints[e.destinationIndex]);
                }
            }

            return neighbors;
        }

        public float GetDistanceBetween(Waypoint o, Waypoint d) {
            foreach (Edge e in edges)
            {
                if (allWaypoints[e.originIndex].Equals(o) && allWaypoints[e.destinationIndex].Equals(d))
                {
                    return e.distance;
                }
            }

            return 0;
        }
    }
}
