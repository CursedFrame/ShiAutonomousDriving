using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using GleyTrafficSystem;

public class MergeFailEvent : AutonomousEvent
{
    public const string TAG = "MergeFailEvent";

    public override void StartEvent()
    {
        Vector3 mergeFailStartPosition = new Vector3(2537.32f, 52.06f, 1898.08f);

        // start pathing job to event location
        EventManager.Instance.StartChildCoroutine(EventManager.Instance.PlayerVehicleAutonomous.Pathing(TrafficManager.Instance.GetClosestForwardWaypoint(
                EventManager.Instance.PlayerVehicle.gameObject, EventManager.Instance.PlayerVehicleAutonomous.GetForwardPoint().position), 
                GleyTrafficSystem.Manager.GetClosestWaypoint(mergeFailStartPosition), StartMergeFail));
        EventLogger.Log(TAG, "Vehicle pathing to merge fail event location.");
    }

    public override void StopEvent()
    {
        EventManager.Instance.StopWatch(TAG);
    }

    private void StartMergeFail()
    {
        Vector3 mergeFailEndPosition = new Vector3(2533.7f, 52.06f, 1825.7f);

        EventManager.Instance.StartWatch(TAG);
        
        GleyTrafficSystem.Manager.SetNextWaypoint(EventManager.Instance.PlayerVehicle, GleyTrafficSystem.Manager.GetClosestWaypoint(mergeFailEndPosition));
    }
}
