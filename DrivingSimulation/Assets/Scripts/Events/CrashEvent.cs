using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GleyTrafficSystem;

public class CrashEvent
{
    public const string TAG = "CrashEvent";
    private AutonomousVehicle playerVehicle;

    public CrashEvent(AutonomousVehicle playerVehicle)
    {
        this.playerVehicle = playerVehicle;
    }

    public void StartCrashEvent()
    {
        Vector3 crashEventPosition = new Vector3(1609.96f, 52.06f, 2793.8f);
        Vector3 despawnDetectPosition = new Vector3(1610.65f, 52.06f, 2741.06f);

        // start pathing job to event location and spawn crash event via callback
        EventManager.Instance.StartChildCoroutine(playerVehicle.Pathing(TrafficManager.Instance.GetClosestForwardWaypoint(
                playerVehicle.gameObject, playerVehicle.GetForwardPoint().position), GleyTrafficSystem.Manager.GetClosestWaypoint(crashEventPosition), OnAtEventPosition));
        
        // intialize player detection object for despawning other traffic vehicles
        GameObject despawnDetect = new GameObject("DespawnDetect");
        BoxCollider collider = despawnDetect.AddComponent<BoxCollider>();
        collider.center = despawnDetectPosition;
        collider.size = new Vector3(5f, 5f, 5f);
        collider.isTrigger = true;
        DetectPlayerCollision detectPlayerCollision = despawnDetect.AddComponent<DetectPlayerCollision>();
        detectPlayerCollision.EnterAction = OnPlayerCollisionEnter;
        detectPlayerCollision.DeleteGameObjectOnEnter = true;
    }

    private void OnAtEventPosition()
    {
        EventManager.Instance.StartChildCoroutine(SpawnCrashEvent());
    }

    private IEnumerator SpawnCrashEvent()
    {
        Vector3 carOnePosition = new Vector3(1576.75f, 52.06f, 2837.51f), carOneQuaternion = new Vector3(0, 90f, 0);
        Vector3 carTwoPosition = new Vector3(1649.75f, 52.06f, 2840.94f), carTwoQuaternion = new Vector3(0, 265f, 0);

        GameObject carOne = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/CrashVehicles/CrashVehicle_Blue"), carOnePosition, Quaternion.Euler(carOneQuaternion));
        GameObject carTwo = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/CrashVehicles/CrashVehicle_Yellow"), carTwoPosition, Quaternion.Euler(carTwoQuaternion));

        yield return new WaitForSeconds(1); // waits for cars to spawn and be in position

        carOne.GetComponent<CrashVehicle>().Mode = CrashVehicle.DriveMode.ACCELERATE;
        carTwo.GetComponent<CrashVehicle>().Mode = CrashVehicle.DriveMode.ACCELERATE;

        Debug.Log(TAG + ": Crash event started.");
        yield break;
    }

    private void OnPlayerCollisionEnter()
    {
        Vector3 crashEpicenter = new Vector3(1609.12f, 52.06f, 2839.1f);
        
        // limit spawning so vehicle won't spawn in crash event area
        GleyTrafficSystem.Manager.ClearTrafficOnArea(crashEpicenter, 40.0f);
        GleyTrafficSystem.Manager.SetTrafficDensity(1);

        Debug.Log(TAG + ": Traffic cleared.");
    }
}
