using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GleyTrafficSystem;
public class AutonomousVehicle : MonoBehaviour
{
    [SerializeField] private GameObject batteryIndicator;
    [SerializeField] private AudioSource batteryIndicatorSound;
    private bool autonomousEnabled = true;
    private bool pathfindingStarted;
    public Transform forwardPoint;
    public Vector3 eventPosition;

    IEnumerator PathingJob(PathingData data){
        List<Waypoint> path = TrafficComponent.Instance.pathfinding.AStar(data.startWaypoint, data.endWaypoint);
        int nextPathIndex = 0;
        while (nextPathIndex != path.Count){
            if (Vector3.Distance(transform.position, path[nextPathIndex].position) < 2.0f){
                nextPathIndex++;
                if (nextPathIndex == path.Count) break;
                GleyTrafficSystem.Manager.SetNextWaypoint(this.gameObject, path[nextPathIndex]);
                Debug.Log(path[nextPathIndex].name);
            }
            yield return null;
        }
        Debug.Log("Arrived at destination!");
        yield break;
    }
    class PathingData {
        public Waypoint startWaypoint, endWaypoint;
        public PathingData(Waypoint startWaypoint, Waypoint endWaypoint){
            this.startWaypoint = startWaypoint;
            this.endWaypoint = endWaypoint;
        }
    }
    void Start()
    {
        MoveTrafficSystem.Instance.player = this.transform;
        EventManager.Instance.Initialize(this);
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
            StartCoroutine(PathingJob(new PathingData(TrafficManager.Instance.GetClosestForwardWaypoint(this.gameObject, forwardPoint.position), GleyTrafficSystem.Manager.GetClosestWaypoint(eventPosition))));
        }
    }

    public GameObject GetBatteryIndicator(){
        return batteryIndicator;
    }
    public AudioSource GetBatteryIndicatorSound(){
        return batteryIndicatorSound;
    }
    public GameObject GetPlayerGameObject(){
        return this.gameObject;
    }
    public bool IsInAutonomousMode(){
        return autonomousEnabled;
    }
}

