using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

namespace GleyTrafficSystem
{
    public class AutonomousVehicle : MonoBehaviour
    {
        private bool autonomousEnabled = true, dijkstraTest = false, waypointSet = false, calculatedPath = false;
        public Transform forwardPoint;
        // private List<Waypoint> path;
        public Vector3 eventPosition;
        // private Dijkstra pathfinding;
        private int nextPathIndex = 0;

        IEnumerator PathfindingJob(){
            List<Waypoint> path = TrafficComponent.Instance.pathfinding.AStar(TrafficManager.Instance.GetClosestForwardWaypoint(this.gameObject, forwardPoint.position), GleyTrafficSystem.Manager.GetClosestWaypoint(eventPosition));
            calculatedPath = true;
            foreach (Waypoint p in path){
                Debug.Log("name: " + p.name + ", position: " + p.position.ToString());
            }
            yield return null;
        }
        void Start()
        {
            MoveTrafficSystem.Instance.player = this.transform;
            // pathfinding ;
        }
        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha9)){
                if (!autonomousEnabled){
                    GleyTrafficSystem.Manager.SetTrafficVehicleToClosestForwardWaypoint(this.gameObject, forwardPoint.position);
                    GleyTrafficSystem.Manager.StartVehicleDriving(this.gameObject);
                } else {
                    GleyTrafficSystem.Manager.StopVehicleDriving(this.gameObject);
                }
                autonomousEnabled = !autonomousEnabled;
            }

            if (Input.GetKeyDown(KeyCode.Alpha8)){
                dijkstraTest = !dijkstraTest;
            }

            if (dijkstraTest){
                if (!calculatedPath){
                    StartCoroutine(PathfindingJob());
                }
            }

            // if (dijkstraTest){
            //     if (path == null){
            //         if (!TrafficComponent.Instance.pathfinding.calculatingPath && !TrafficComponent.Instance.pathfinding.pathCalculated){
            //             TrafficComponent.Instance.pathfinding.CalculateDistance(TrafficManager.Instance.GetClosestForwardWaypoint(this.gameObject, forwardPoint.position));
            //         } 
            //         if (TrafficComponent.Instance.pathfinding.pathCalculated){
            //             path = TrafficComponent.Instance.pathfinding.GetPathTo(GleyTrafficSystem.Manager.GetClosestWaypoint(eventPosition));
            //             foreach (Waypoint p in path){
            //                 Debug.Log("name: " + p.name + ", position: " + p.position.ToString());
            //             }
            //         } 
            //     } else {
            //         if (!waypointSet){
            //             GleyTrafficSystem.Manager.SetNextWaypoint(this.gameObject, path[nextPathIndex]);
            //             waypointSet = true;
            //         } 
            //         if (nextPathIndex < path.Count){
            //             if (Vector3.Distance(transform.position, path[nextPathIndex].position) < 0.01f){
            //                 nextPathIndex++;
            //                 waypointSet = false;
            //             }
            //         }
            //     }
            // }
        }
    }
}

