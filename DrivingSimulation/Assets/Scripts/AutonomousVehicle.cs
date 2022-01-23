using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GleyTrafficSystem;
public class AutonomousVehicle : MonoBehaviour
{
    [SerializeField] private GameObject batteryIndicator;
    [SerializeField] private AudioSource batteryIndicatorSound;
    private bool autonomousEnabled = true;
    public Transform forwardPoint;

    public IEnumerator Pathing(Waypoint start, Waypoint end){
        List<Waypoint> path = TrafficComponent.Instance.pathfinding.AStar(start, end);
        int nextPathIndex = 0;
        while (nextPathIndex != path.Count){
            nextPathIndex++;
            if (nextPathIndex == path.Count) break;
            yield return GoToWaypoint(path[nextPathIndex]);
        }
        Debug.Log("Arrived at destination!");
        yield break;
    }
    private IEnumerator GoToWaypoint(Waypoint waypoint){
        GleyTrafficSystem.Manager.SetNextWaypoint(this.gameObject, waypoint);
        Debug.Log(waypoint.name);
        while (Vector3.Distance(this.gameObject.transform.position, waypoint.position) > 5.0f) yield return null;
        yield break;
    }
    void Start()
    {
        MoveTrafficSystem.Instance.Initialize(this.transform);
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

