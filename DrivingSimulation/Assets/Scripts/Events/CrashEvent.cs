using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GleyTrafficSystem;

public class CrashEvent
{
    public static string TAG = "CrashEvent";
    
    public void StartCrashEvent(){
        GameObject railForCrash = GameObject.Find("Landscape/Landscape 709/Barriers");
        Vector3 crashCarSpawnLocation = new Vector3(2541.39f, 53.1f, 1998.23f);
        
        if (railForCrash == null){
            Debug.Log(TAG + ": Crash railing could not be found.");
            return;
        }

        Debug.Log(TAG + ": Crash event started.");

        railForCrash.SetActive(true);
        
        GleyTrafficSystem.Manager.AddVehicleWithWaypoint(crashCarSpawnLocation, GleyTrafficSystem.VehicleTypes.Car, CrashCallback);
    }

    private void CrashCallback(int vehicleIndex){
        SetVehicleCrashDestination(vehicleIndex);
        SetVehicleEmergencyLights(vehicleIndex);
    }

    private void SetVehicleCrashDestination(int vehicleIndex){
        Vector3 crashLocation = new Vector3(2549.58f, 53.1f, 1988.97f);
        Waypoint crashWaypoint = GleyTrafficSystem.Manager.GetClosestWaypoint(crashLocation);

        if (crashWaypoint.listIndex == -1){
            Debug.Log(TAG + ": Crash location waypoint not found. Returning...");
            return;
        }
        
        GleyTrafficSystem.Manager.SetNextWaypoint(vehicleIndex, crashWaypoint);
        Debug.Log(TAG + ": Set crash waypoint " + crashWaypoint.name + " at " + crashWaypoint.position.ToString());
    }

    private void SetVehicleEmergencyLights(int vehicleIndex){
        GleyTrafficSystem.Manager.SetVehicleBlinkerType(vehicleIndex, BlinkType.Emergency);
        Debug.Log(TAG + ": Crash vehicle emergency lights set.");
    }
}
