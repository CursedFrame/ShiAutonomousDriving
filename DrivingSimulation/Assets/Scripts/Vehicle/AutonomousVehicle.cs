using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GleyTrafficSystem;
public class AutonomousVehicle : MonoBehaviour
{
    public const string TAG = "AutonomousVehicle";
    [SerializeField] private GameObject batteryIndicator;
    [SerializeField] private AudioSource batteryIndicatorSound;
    [SerializeField] private Camera playerCamera;
    private IEnumerator pathingRoutine;
    private Waypoint pathingEnd;
    private System.Action pathingCallback;
    
    public bool IsInAutonomous { get; set; } = true;
    public bool IsPathing { get; set; } = false;
    public GameObject GetBatteryIndicator(){ return batteryIndicator; }
    public AudioSource GetBatteryIndicatorSound() { return batteryIndicatorSound; }
    
    public void StartPathing(Waypoint start, Waypoint end, System.Action callback = null)
    {
        pathingRoutine = Pathing(start, end, callback);
        pathingEnd = end;
        pathingCallback = callback;
        StartCoroutine(pathingRoutine);

        IsPathing = true;
    }

    public void DisposePathing()
    {
        IsPathing = false;

        pathingRoutine = null;
        pathingEnd = null;
        pathingCallback = null;
    }

    public IEnumerator Pathing(Waypoint start, Waypoint end, System.Action callback = null)
    {
        List<Waypoint> path = TrafficComponent.Instance.pathfinding.AStar(start, end);
        int nextPathIndex = 0;

        while (nextPathIndex != path.Count)
        {
            nextPathIndex++;
            if (nextPathIndex == path.Count) break;
            yield return GoToWaypoint(path[nextPathIndex]);
        }

        Debug.Log("Arrived at destination!");
        if (callback != null) callback();
        yield break;
    }

    private IEnumerator GoToWaypoint(Waypoint waypoint)
    {
        GleyTrafficSystem.Manager.SetNextWaypoint(this.gameObject, waypoint);
        Debug.Log(waypoint.name);
        while (Vector3.Distance(this.gameObject.transform.position, waypoint.position) > 5.0f) yield return null;
        yield break;
    }

    private void Start()
    {
        MoveTrafficSystem.Instance.Initialize(this.transform);
        EventManager.Instance.Initialize(this.gameObject);
        GameMaster.Instance.MainCamera = playerCamera;
    }

    // Update is called once per frame
    private void Update()
    {
        // Manual toggle of autonomous mode
        if (Input.GetKeyDown(KeyCode.Keypad9))
        {
            if (!IsInAutonomous)
            {   
                GleyTrafficSystem.Manager.StartPlayerVehicleDriving(this.gameObject);
                EventLogger.Log(TAG, "Driver used the toggle button to enable autonomous mode.");

                // Continue pathing if should be pathing
                if (IsPathing)
                {
                    StartPathing(TrafficManager.Instance.GetClosestForwardWaypoint(
                        EventManager.Instance.PlayerVehicle.gameObject, EventManager.Instance.PlayerVehicle.transform.forward), 
                        pathingEnd, pathingCallback);
                    EventLogger.Log(TAG, "Continuing pathing to event.");
                }

                IsInAutonomous = true;
            } 
            else 
            {
                DriverTakeControl("toggle button");
            }
        }

        // Disables autonomous control when driver moves the steering wheel
        // or presses any pedal
        if (!IsInAutonomous) return;

        if (Input.GetAxis("Horizontal") > 0.1f)
        {
            DriverTakeControl("steering wheel turned right");
            return;
        }

        if (Input.GetAxis("Horizontal") < -0.1f)
        {
            DriverTakeControl("steering wheel turned left");
            return;
        }

        if (Input.GetAxis("Vertical") > 0.1f)
        {
            DriverTakeControl("gas pedal");
            return;
        }

        if (Input.GetAxis("Vertical") < -0.1f)
        {
            DriverTakeControl("brake");
            return;
        }
    }

    private void DriverTakeControl(string methodTakenControl)
    {
        GleyTrafficSystem.Manager.StopVehicleDriving(this.gameObject);
        EventManager.Instance.HandleAutonomousDisabled(methodTakenControl);

        // Pause pathing if pathing
        if (IsPathing)
        {
            StopCoroutine(pathingRoutine);
            pathingRoutine = null;
            EventLogger.Log(TAG, "Pausing pathing to event.");
        }
        
        IsInAutonomous = false;
    }
}

