using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GleyTrafficSystem;
using System.Diagnostics;
using System;

public static class ControlLossEvent
{
    public const string TAG = "ControlLossEvent";
    private const float waypointChangeTime = 1.0f;
    private static bool controlLost = false;
    private static float currentTime;

    public static void StartEvent()
    {
        controlLost = true;

        GleyTrafficSystem.Manager.SetNextWaypoint(EventManager.Instance.PlayerVehicle, GleyTrafficSystem.Manager.GetClosestWaypoint(EventManager.Instance.PlayerVehicle));
        EventManager.Instance.StartWatch(TAG);
    }

    public static void StopEvent()
    {
        controlLost = false;

        EventManager.Instance.StopWatch(TAG);
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
