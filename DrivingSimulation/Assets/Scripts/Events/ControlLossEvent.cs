using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GleyTrafficSystem;
using System.Diagnostics;
using System;

public class ControlLossEvent : AutonomousEvent, UpdateEvent
{
    public const string TAG = "ControlLossEvent";
    private const float waypointChangeTime = 1.0f;
    private bool controlLost = false;
    private float currentTime;

    public override void StartEvent()
    {
        controlLost = true;

        GleyTrafficSystem.Manager.SetNextWaypoint(EventManager.Instance.PlayerVehicle, GleyTrafficSystem.Manager.GetClosestWaypoint(EventManager.Instance.PlayerVehicle));
        EventManager.Instance.StartWatch(TAG);
    }

    public override void StopEvent()
    {
        controlLost = false;

        EventManager.Instance.StopWatch(TAG);
    }    

    public void UpdateEvent(){
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
