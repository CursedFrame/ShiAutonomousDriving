using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GleyTrafficSystem;

public class CrashEvent
{
    public static string TAG = "CrashEvent";
    private AutonomousVehicle playerVehicle;
    private readonly Vector3 crashEventPosition = new Vector3(2536.88f, 53.1f, 2005.05f);

    public CrashEvent(AutonomousVehicle playerVehicle){
        this.playerVehicle = playerVehicle;
    }

    public void StartCrashEvent(){
        // start pathing job to event location
        EventManager.Instance.StartChildCoroutine(playerVehicle.Pathing(TrafficManager.Instance.GetClosestForwardWaypoint(
                playerVehicle.gameObject, playerVehicle.forwardPoint.position), GleyTrafficSystem.Manager.GetClosestWaypoint(crashEventPosition)));
        // spawn crash event when within distance
        EventManager.Instance.StartChildCoroutine(SpawnCrashEvent());
    }

    private IEnumerator SpawnCrashEvent(){
        while (Vector3.Distance(playerVehicle.transform.position, crashEventPosition) > 350.0f) yield return new WaitForSeconds(1);

        GameObject railForCrash = GameObject.Find("Landscape/Landscape 709/Barriers");
        Vector3 carOnePosition = new Vector3(2532.22f, 53.1f, 1996.3f), carOneQuaternion = new Vector3(0, -276.319f, 0);
        Vector3 carTwoPosition = new Vector3(2540.19f, 53.1f, 2005.88f), carTwoQuaternion = new Vector3(0, -191.718f, 0);
        
        if (railForCrash == null){
            Debug.Log(TAG + ": Crash railing could not be found.");
            yield break;
        }

        railForCrash.SetActive(true);
        GameObject carOne = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/CrashVehicles/CrashVehicle_Blue"), carOnePosition, Quaternion.Euler(carOneQuaternion));
        GameObject carTwo = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/CrashVehicles/CrashVehicle_Yellow"), carTwoPosition, Quaternion.Euler(carTwoQuaternion));
        carOne.GetComponent<CrashVehicle>().SetDriveMode(CrashVehicle.DriveMode.ACCELERATE);
        carTwo.GetComponent<CrashVehicle>().SetDriveMode(CrashVehicle.DriveMode.ACCELERATE);

        Debug.Log(TAG + ": Crash event started.");

        yield break;
    }

    // private IEnumerator SpawnCrashEvent(){
    //     while (Vector3.Distance(playerVehicle.transform.position, crashEventPosition) > 350.0f) yield return new WaitForSeconds(1);

    //     GameObject railForCrash = GameObject.Find("Landscape/Landscape 709/Barriers");
    //     Vector3 crashCarSpawnLocation = new Vector3(2541.39f, 53.1f, 1998.23f);
        
    //     if (railForCrash == null){
    //         Debug.Log(TAG + ": Crash railing could not be found.");
    //         yield break;
    //     }

    //     Debug.Log(TAG + ": Crash event started.");

    //     railForCrash.SetActive(true);
        
    //     GleyTrafficSystem.Manager.AddVehicleWithWaypoint(crashCarSpawnLocation, GleyTrafficSystem.VehicleTypes.Car, CrashCallback);
    //     yield break;
    // }

    // private void CrashCallback(int vehicleIndex){
    //     SetVehicleCrashDestination(vehicleIndex);
    //     SetVehicleEmergencyLights(vehicleIndex);
    // }

    // private void SetVehicleCrashDestination(int vehicleIndex){
    //     Vector3 crashLocation = new Vector3(2549.58f, 53.1f, 1988.97f);
    //     Waypoint crashWaypoint = GleyTrafficSystem.Manager.GetClosestWaypoint(crashLocation);

    //     if (crashWaypoint.listIndex == -1){
    //         Debug.Log(TAG + ": Crash location waypoint not found. Returning...");
    //         return;
    //     }
        
    //     GleyTrafficSystem.Manager.SetNextWaypoint(vehicleIndex, crashWaypoint);
    //     Debug.Log(TAG + ": Set crash waypoint " + crashWaypoint.name + " at " + crashWaypoint.position.ToString());
    // }

    // private void SetVehicleEmergencyLights(int vehicleIndex){
    //     GleyTrafficSystem.Manager.SetVehicleBlinkerType(vehicleIndex, BlinkType.Emergency);
    //     Debug.Log(TAG + ": Crash vehicle emergency lights set.");
    // }
}
