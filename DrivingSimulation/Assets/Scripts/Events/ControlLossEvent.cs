using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GleyTrafficSystem;

public class ControlLossEvent
{
    public static string TAG = "ControlLossEvent";
    private bool controlLost = false;
    private const float waypointChangeTime = 1.0f;
    private float currentTime;
    private GameObject playerVehicle;

    public ControlLossEvent(GameObject playerVehicle){
        this.playerVehicle = playerVehicle;
    }
    public void ToggleControlLoss(){
        if (controlLost) GleyTrafficSystem.Manager.SetNextWaypoint(playerVehicle, GleyTrafficSystem.Manager.GetClosestWaypoint(playerVehicle));
        controlLost = !controlLost;
    }

    public void UpdateControlLoss(){
        if (controlLost)
        {
            if (Time.realtimeSinceStartup - currentTime > waypointChangeTime)
            {
                currentTime = Time.realtimeSinceStartup;
                Waypoint waypoint = GleyTrafficSystem.Manager.GetRandomWaypoint();
                GleyTrafficSystem.Manager.SetNextWaypoint(playerVehicle, waypoint);
            }
        }
    }
}
