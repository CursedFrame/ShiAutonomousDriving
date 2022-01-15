using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Priority_Queue;

namespace GleyTrafficSystem {
    // // very inefficient version of djikstras
    public class Dijkstra {
        public class Edge
        {
            public Waypoint origin, destination;
            public float distance;
        }
        private List<Edge> edges = new List<Edge>();
        private List<Waypoint> basis;
        private Dictionary<string, float> dist;
        private Dictionary<string, Waypoint> previous;
        public bool pathCalculated = false, calculatingPath = false;

        public Dijkstra(List<Waypoint> nodes) {
            basis = new List<Waypoint>();
            dist = new Dictionary<string, float>();
            previous = new Dictionary<string, Waypoint>();

            // record node 
            foreach (Waypoint n in nodes)
            {
                if (!dist.ContainsKey(n.name)){
                    previous.Add(n.name, null);
                    basis.Add(n);
                    dist.Add(n.name, float.MaxValue);
                }
            }

            foreach (Waypoint n in basis){
                foreach (int r in n.neighbors){
                    Edge e = new Edge();
                    e.origin = n;
                    e.destination = nodes[r];
                    e.distance = Vector3.Distance(e.origin.position, e.destination.position);
                    edges.Add(e);
                }
            }
            Debug.Log("Finished constructing Dijkstra");
        }

        /// Calculates the shortest path from the start
        ///  to all other nodes
        public void CalculateDistance(Waypoint start) {
            calculatingPath = true;
            dist[start.name] = 0;
            while (basis.Count > 0)
            {
                Waypoint u = GetWaypointWithSmallestDistance();
                if (u == null)
                {
                    basis.Clear();
                }
                else
                {
                    foreach (Waypoint v in GetNeighbors(u))
                    {
                        float alt = dist[u.name] + GetDistanceBetween(u, v);
                        if (alt < dist[v.name])
                        {
                            dist[v.name] = alt;
                            previous[v.name] = u;
                        }
                    }
                    basis.Remove(u);
                }
            }
            calculatingPath = false;
            pathCalculated = true;
            Debug.Log("pathCalculated");
        }

        public List<Waypoint> GetPathTo(Waypoint d) {
            List<Waypoint> path = new List<Waypoint>();

            path.Insert(0, d);

            while (previous[d.name] != null)
            {
                d = previous[d.name];
                path.Insert(0, d);
            }

            return path;
        }

        public Waypoint GetWaypointWithSmallestDistance() {
            float distance = float.MaxValue;
            Waypoint smallest = null;

            foreach (Waypoint n in basis)
            {
                if (dist[n.name] < distance)
                {
                    distance = dist[n.name];
                    smallest = n;
                }
            }

            return smallest;
        }


        public List<Waypoint> GetNeighbors(Waypoint n) {
            List<Waypoint> neighbors = new List<Waypoint>();

            foreach (Edge e in edges)
            {
                if (e.origin.Equals(n) && basis.Contains(n))
                {
                    neighbors.Add(e.destination);
                }
            }

            return neighbors;
        }

        public float GetDistanceBetween(Waypoint o, Waypoint d) {
            foreach (Edge e in edges)
            {
                if (e.origin.Equals(o) && e.destination.Equals(d))
                {
                    return e.distance;
                }
            }

            return 0;
        }
    }
}
