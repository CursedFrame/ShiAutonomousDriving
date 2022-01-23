using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GleyTrafficSystem;

public class MergeFailEvent
{
    public static string TAG = "MergeFailEvent";
    private AutonomousVehicle playerVehicle;
    private readonly Vector3 mergeFailEventPosition = new Vector3(2536.88f, 53.1f, 2005.05f);

    public MergeFailEvent(AutonomousVehicle playerVehicle){
        this.playerVehicle = playerVehicle;
    }

    public void StartCrashEvent(){
        // start pathing job to event location
        EventManager.Instance.StartChildCoroutine(playerVehicle.Pathing(TrafficManager.Instance.GetClosestForwardWaypoint(
                playerVehicle.gameObject, playerVehicle.forwardPoint.position), GleyTrafficSystem.Manager.GetClosestWaypoint(mergeFailEventPosition)));
    }
}
