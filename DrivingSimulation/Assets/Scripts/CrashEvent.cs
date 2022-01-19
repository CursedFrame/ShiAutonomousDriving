using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GleyTrafficSystem;

public static class CrashEvent
{
    public static string TAG = "CrashEvent";
    
    public static void StartCrashEvent(Vector3 crashCarSpawnLocation, Vector3 crashLocation){
        GameObject railForCrash = GameObject.Find("Landscape 709/Barriers");

        if (crashLocation == null || crashCarSpawnLocation == null || railForCrash == null){
            Debug.Log(TAG + ": One or more objects are null.");
        }

        railForCrash.SetActive(true);
        int crashWaypointIndex = GleyTrafficSystem.Manager.GetClosestWaypointIndex(crashLocation);

        if (crashWaypointIndex == -1){
            Debug.Log(TAG + ": Crash location waypoint not found. Returning...");
        }

        GleyTrafficSystem.Manager.AddVehicleWithWaypoint(crashCarSpawnLocation, GleyTrafficSystem.VehicleTypes.Car, crashWaypointIndex, SetVehicleEmergencyLights);
    }

    public static void SetVehicleEmergencyLights(int vehicleIndex){
        GleyTrafficSystem.Manager.SetVehicleBlinkerType(vehicleIndex, BlinkType.Emergency);
        Debug.Log("WOW!");
    }
}
