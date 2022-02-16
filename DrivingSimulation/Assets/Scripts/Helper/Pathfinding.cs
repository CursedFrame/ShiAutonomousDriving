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

    private class Node
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

        Debug.Log(TAG + ": Pathfinding ready.");
    }

    // f(n) = g(n) + h(n)
    // Heuristic function: euclidian distance
    public List<Waypoint> AStar(Waypoint startWaypoint, Waypoint endWaypoint)
    {
        Node start = nodes[startWaypoint], end = nodes[endWaypoint];
        SimplePriorityQueue<Node> openList = new SimplePriorityQueue<Node>();
        start.g = 0;
        start.f = start.g + GetDistanceBetween(start, end);
        openList.Enqueue(start, start.f);

        while (openList.Count > 0)
        {
            Node current = openList.First;
            openList.Dequeue();
            if (current == end) break;

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

        // Extra disposal if needed
        openList.Clear();

        List<Waypoint> path = GetPath(end);

        // Reset nodes for later pathfinding usage
        foreach (Node node in nodes.Values)
        {
            node.parent = null;
            node.g = float.MaxValue;
            node.f = float.MaxValue;
        }
        
        return path;
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

        Debug.Log(TAG + ": Found optimal path of length " + path.Count);

        return path;
    }

    private float GetDistanceBetween(Node node, Node end)
    {
        return Vector3.Distance(node.waypoint.position, end.waypoint.position);
    }
}
