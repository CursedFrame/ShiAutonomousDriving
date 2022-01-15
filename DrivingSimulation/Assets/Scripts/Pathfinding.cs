using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GleyTrafficSystem;
using Priority_Queue;
public class Pathfinding
{
    private int numNodes;
    private Dictionary<Waypoint, Node> nodes;
    private List<Waypoint> waypoints;
    public class Node : FastPriorityQueueNode {
        public Waypoint waypoint;
        public Node parent;
        public List<Node> neighbors;
        public float g = float.MaxValue, f = float.MaxValue;
    }

    public Pathfinding(List<Waypoint> waypoints){
        this.waypoints = waypoints;
        nodes = new Dictionary<Waypoint, Node>();

        // construct nodes
        foreach (Waypoint waypoint in waypoints){
            Node node = new Node();
            node.waypoint = waypoint;
            node.parent = null;
            node.neighbors = new List<Node>();
            nodes.Add(waypoint, node);
            ++numNodes;
        }

        // add neighbors to each node
        foreach(Node node in nodes.Values){
            foreach(int neighborIndex in node.waypoint.neighbors){
                node.neighbors.Add(nodes[waypoints[neighborIndex]]);
            }
        }
    }

    // f(n) = g(n) + h(n)
    // Heuristic function: euclidian distance
    public List<Waypoint> AStar(Waypoint startWaypoint, Waypoint endWaypoint){
        Node start = nodes[startWaypoint], end = nodes[endWaypoint];
        FastPriorityQueue<Node> openList = new FastPriorityQueue<Node>(numNodes);
        start.g = 0;
        start.f = start.g + GetDistanceBetween(start, end);
        openList.Enqueue(start, start.f);
        while(openList.Count > 0){
            Node current = openList.First;
            if (current == end) return GetPath(end);
            openList.Dequeue();
            foreach (Node neighbor in current.neighbors){
                float gScore = current.g + GetDistanceBetween(current, neighbor);
                if (gScore < neighbor.g){
                    neighbor.parent = current;
                    neighbor.g = gScore;
                    neighbor.f = gScore + GetDistanceBetween(neighbor, end);
                    if (!openList.Contains(neighbor)) {
                        openList.Enqueue(neighbor, neighbor.f);
                    }
                }
            }
        }
        return null;
    }

    public List<Waypoint> GetPath(Node end){
        List<Waypoint> path = new List<Waypoint>();
        Node current = end;
        path.Add(current.waypoint);
        while (current.parent != null){
            path.Insert(0, current.parent.waypoint);
            current = current.parent;
        }
        Debug.Log("Found optimal path of length " + path.Count);
        return path;
    }

    public float GetDistanceBetween(Node node, Node end){
        return Vector3.Distance(node.waypoint.position, end.waypoint.position);
    }
}
