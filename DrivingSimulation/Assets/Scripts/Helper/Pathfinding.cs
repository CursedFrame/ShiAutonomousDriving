using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GleyTrafficSystem;
using Priority_Queue;
public class Pathfinding
{
    public const string TAG = "Pathfinding";
    private int numNodes;
    private Dictionary<Waypoint, Node> nodes;

    private class Node : FastPriorityQueueNode 
    {
        public Waypoint waypoint;
        public Node parent;
        public Node[] neighbors;
        public float g = float.MaxValue, f = float.MaxValue;

        public Node(Waypoint waypoint, Node parent, Node[] neighbors)
        {
            this.waypoint = waypoint;
            this.parent = parent;
            this.neighbors = neighbors;
        }
    }

    public Pathfinding(List<Waypoint> waypoints)
    {
        nodes = new Dictionary<Waypoint, Node>();

        // construct nodes
        foreach (Waypoint waypoint in waypoints)
        {
            Node node = new Node(waypoint, null, new Node[waypoint.neighbors.Count]);
            nodes.Add(waypoint, node);
            ++numNodes;
        }

        // add neighbors to each node
        foreach (Node node in nodes.Values)
        {
            for (int i = 0; i < node.waypoint.neighbors.Count; i++)
            {
                node.neighbors[i] = nodes[waypoints[node.waypoint.neighbors[i]]];
            }
        }
    }

    // f(n) = g(n) + h(n)
    // Heuristic function: euclidian distance
    public List<Waypoint> AStar(Waypoint startWaypoint, Waypoint endWaypoint)
    {
        Node start = nodes[startWaypoint], end = nodes[endWaypoint];
        FastPriorityQueue<Node> openList = new FastPriorityQueue<Node>(numNodes);
        start.g = 0;
        start.f = start.g + GetDistanceBetween(start, end);
        openList.Enqueue(start, start.f);

        while (openList.Count > 0)
        {
            Node current = openList.First;
            if (current == end) return GetPath(end);
            openList.Dequeue();

            foreach (Node neighbor in current.neighbors)
            {
                float gScore = current.g + GetDistanceBetween(current, neighbor);

                if (gScore < neighbor.g)
                {
                    neighbor.parent = current;
                    neighbor.g = gScore;
                    neighbor.f = gScore + GetDistanceBetween(neighbor, end);

                    if (!openList.Contains(neighbor)) 
                    {
                        openList.Enqueue(neighbor, neighbor.f);
                    }
                }
            }
        }

        return null;
    }

    private List<Waypoint> GetPath(Node end)
    {
        List<Waypoint> path = new List<Waypoint>();
        Node current = end;
        path.Add(current.waypoint);

        while (current.parent != null)
        {
            path.Insert(0, current.parent.waypoint);
            current = current.parent;
        }

        Debug.Log("Found optimal path of length " + path.Count);
        return path;
    }

    private float GetDistanceBetween(Node node, Node end)
    {
        return Vector3.Distance(node.waypoint.position, end.waypoint.position);
    }
}
