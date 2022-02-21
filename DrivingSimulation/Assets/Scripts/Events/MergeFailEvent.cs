using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using GleyTrafficSystem;

public class MergeFailEvent : AutonomousEvent
{
    public override string Tag { get { return "MergeFailEvent"; } }

    public override void StartEvent()
    {
        Vector3 mergeFailStartPosition = new Vector3(2537.32f, 52.06f, 1898.08f);

        // start pathing job to event location
        EventManager.Instance.PlayerVehicleAutonomous.StartPathing(TrafficManager.Instance.GetForwardWaypoint(
                EventManager.Instance.PlayerVehicle.gameObject, EventManager.Instance.PlayerVehicle.transform.forward), 
                GleyTrafficSystem.Manager.GetClosestWaypoint(mergeFailStartPosition), StartMergeFail);
        EventLogger.Log(Tag, "Vehicle pathing to merge fail event location.");
    }

    public override void StopEvent()
    {
        EventManager.Instance.PlayerVehicleAutonomous.DisposePathing();
    }

    private void StartMergeFail()
    {
        Vector3 mergeFailEndPosition = new Vector3(2533.7f, 52.06f, 1825.7f);

        EventManager.Instance.StartWatch();
        
        GleyTrafficSystem.Manager.SetNextWaypoint(EventManager.Instance.PlayerVehicle, GleyTrafficSystem.Manager.GetClosestWaypoint(mergeFailEndPosition));
    }
}
