using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GleyTrafficSystem;

public static class CrashEvent
{
    public static string TAG = "CrashEvent";
    
    public static void StartCrashEvent(){
        GameObject railForCrash = GameObject.Find("Landscape 709/Barriers");
        Vector3 crashCarSpawnLocation = new Vector3(2533.11f, 53.1f, 2030.9f);
        
        if (railForCrash == null){
            Debug.Log(TAG + ": Crash railing could not be found.");
            return;
        }

        Debug.Log(TAG + ": Crash event started.");

        railForCrash.SetActive(true);
        
        GleyTrafficSystem.Manager.AddVehicleWithWaypoint(crashCarSpawnLocation, GleyTrafficSystem.VehicleTypes.Car, CrashCallback);
    }

    private static void CrashCallback(int vehicleIndex){
        SetVehicleCrashDestination(vehicleIndex);
        SetVehicleEmergencyLights(vehicleIndex);
    }

    private static void SetVehicleCrashDestination(int vehicleIndex){
        Vector3 crashLocation = new Vector3(2543.02f, 53.1f, 1997.3f);
        Waypoint crashWaypoint = GleyTrafficSystem.Manager.GetClosestWaypoint(crashLocation);

        if (crashWaypoint.listIndex == -1){
            Debug.Log(TAG + ": Crash location waypoint not found. Returning...");
            return;
        }
        
        GleyTrafficSystem.Manager.SetNextWaypoint(vehicleIndex, crashWaypoint);
        Debug.Log(TAG + ": Set crash waypoint " + crashWaypoint.name + " at " + crashWaypoint.position.ToString());
    }

    private static void SetVehicleEmergencyLights(int vehicleIndex){
        GleyTrafficSystem.Manager.SetVehicleBlinkerType(vehicleIndex, BlinkType.Emergency);
        Debug.Log(TAG + ": Crash vehicle emergency lights set.");
    }
}
