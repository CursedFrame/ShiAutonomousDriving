using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GleyTrafficSystem;

public class MergeFailEvent
{
    public const string TAG = "MergeFailEvent";
    private static readonly Vector3 mergeFailEventPosition = new Vector3(2536.88f, 53.1f, 2005.05f);
    private AutonomousVehicle playerVehicle;

    public MergeFailEvent(AutonomousVehicle playerVehicle)
    {
        this.playerVehicle = playerVehicle;
    }

    public void StartCrashEvent()
    {
        // start pathing job to event location
        EventManager.Instance.StartChildCoroutine(playerVehicle.Pathing(TrafficManager.Instance.GetClosestForwardWaypoint(
                playerVehicle.gameObject, playerVehicle.GetForwardPoint().position), GleyTrafficSystem.Manager.GetClosestWaypoint(mergeFailEventPosition)));
    }
}
