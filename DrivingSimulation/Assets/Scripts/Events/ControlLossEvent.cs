using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GleyTrafficSystem;
using System.Diagnostics;
using System;

public class ControlLossEvent : AutonomousEvent
{
    public override string Tag { get { return "ControlLossEvent"; } }
    private const float waypointChangeTime = 1.0f;
    private bool controlLost = false;
    private float currentTime;

    public override void StartEvent()
    {
        controlLost = true;

        GleyTrafficSystem.Manager.SetNextWaypoint(EventManager.Instance.PlayerVehicle, GleyTrafficSystem.Manager.GetClosestWaypoint(EventManager.Instance.PlayerVehicle));
        EventManager.Instance.StartWatch();
    }

    public override void StopEvent()
    {
        controlLost = false;
    }    

    public override void UpdateEvent(){
        if (controlLost)
        {
            if (Time.realtimeSinceStartup - currentTime > waypointChangeTime)
            {
                currentTime = Time.realtimeSinceStartup;
                GleyTrafficSystem.Manager.SetNextWaypoint(EventManager.Instance.PlayerVehicle, GleyTrafficSystem.Manager.GetRandomWaypoint());
            }
        }
    }
}
