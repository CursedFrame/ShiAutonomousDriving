using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GleyTrafficSystem;

public static class ControlLossEvent
{
    public const string TAG = "ControlLossEvent";
    private const float waypointChangeTime = 1.0f;
    private static float currentTime;
    private static bool controlLost = false;

    public static void ToggleEvent()
    {
        if (controlLost) GleyTrafficSystem.Manager.SetNextWaypoint(EventManager.Instance.PlayerVehicle, GleyTrafficSystem.Manager.GetClosestWaypoint(EventManager.Instance.PlayerVehicle));
        controlLost = !controlLost;
    }

    public static void UpdateControlLoss(){
        if (controlLost)
        {
            if (Time.realtimeSinceStartup - currentTime > waypointChangeTime)
            {
                currentTime = Time.realtimeSinceStartup;
                Waypoint waypoint = GleyTrafficSystem.Manager.GetRandomWaypoint();
                GleyTrafficSystem.Manager.SetNextWaypoint(EventManager.Instance.PlayerVehicle, waypoint);
            }
        }
    }
}
