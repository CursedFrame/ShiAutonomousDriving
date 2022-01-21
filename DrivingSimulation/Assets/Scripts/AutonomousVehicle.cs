using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GleyTrafficSystem;
public class AutonomousVehicle : MonoBehaviour
{
    [SerializeField] private GameObject batteryIndicator;
    [SerializeField] private AudioSource batteryIndicatorSound;
    private bool autonomousEnabled = true, dijkstraTest = false, waypointSet = false, calculatedPath = false, movingToDestination = false;
    public Transform forwardPoint;
    private List<Waypoint> path;
    public Vector3 eventPosition;
    private int nextPathIndex = 0;    

    IEnumerator PathfindingJob(){
        path = TrafficComponent.Instance.pathfinding.AStar(TrafficManager.Instance.GetClosestForwardWaypoint(this.gameObject, forwardPoint.position), GleyTrafficSystem.Manager.GetClosestWaypoint(eventPosition));
        calculatedPath = true;
        movingToDestination = true;
        yield break;
    }
    void Start()
    {
        MoveTrafficSystem.Instance.player = this.transform;
        EventManager.Instance.playerVehicle = this;
        EventManager.Instance.Initialize();
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
            if (!calculatedPath) StartCoroutine(PathfindingJob());
            if (movingToDestination && path != null){
                if (nextPathIndex == path.Count){
                    movingToDestination = false;
                    path = null;
                    Debug.Log("Arrived at destination!");
                } else {
                    if (!waypointSet){
                        GleyTrafficSystem.Manager.SetNextWaypoint(this.gameObject, path[nextPathIndex]);
                        Debug.Log(path[nextPathIndex].name);
                        waypointSet = true;
                    } 
                    if (nextPathIndex < path.Count){
                        if (Vector3.Distance(transform.position, path[nextPathIndex].position) < 2.0f){
                            nextPathIndex++;
                            waypointSet = false;
                        }
                    }
                }
                
            }
        }
    }

    public GameObject GetBatteryIndicator(){
        return batteryIndicator;
    }
    public AudioSource GetBatteryIndicatorSound(){
        return batteryIndicatorSound;
    }
}

