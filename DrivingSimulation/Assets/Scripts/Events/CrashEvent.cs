using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GleyTrafficSystem;

public static class CrashEvent
{
    public const string TAG = "CrashEvent";
    private static bool crashSoundPlayed = false;
    private static GameObject carOne;
    private static GameObject carTwo;

    public static void StartEvent()
    {
        Vector3 crashEventPosition = new Vector3(1609.96f, 52.06f, 2793.8f);
        Vector3 despawnDetectPosition = new Vector3(1610.65f, 52.06f, 2741.06f);

        // start pathing job to event location and spawn crash event via callback
        EventManager.Instance.StartChildCoroutine(EventManager.Instance.PlayerVehicleAutonomous.Pathing(TrafficManager.Instance.GetClosestForwardWaypoint(
                EventManager.Instance.PlayerVehicle.gameObject, EventManager.Instance.PlayerVehicleAutonomous.GetForwardPoint().position), 
                GleyTrafficSystem.Manager.GetClosestWaypoint(crashEventPosition), OnAtEventPosition));
        
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

    public static void StopEvent(){
        UnityEngine.Object.Destroy(carOne);
        UnityEngine.Object.Destroy(carTwo);
    }

    public static void PlayCrashSound()
    {
        if (crashSoundPlayed) return;
        Vector3 crashEpicenter = new Vector3(1609.05f, 53.07f, 2839.13f);
        AudioSource.PlayClipAtPoint(Resources.Load<AudioClip>("Audio/Car_Crash"), 0.2f * crashEpicenter + 0.8f * EventManager.Instance.PlayerVehicle.transform.position);
        crashSoundPlayed = true;
    }

    private static void OnAtEventPosition()
    {
        EventManager.Instance.StartChildCoroutine(SpawnCrashEvent());
    }

    private static IEnumerator SpawnCrashEvent()
    {
        // car spawning
        Vector3 carOnePosition = new Vector3(1576.75f, 52.06f, 2837.51f), carOneQuaternion = new Vector3(0, 90f, 0);
        Vector3 carTwoPosition = new Vector3(1649.75f, 52.06f, 2840.94f), carTwoQuaternion = new Vector3(0, 265f, 0);

        carOne = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/CrashVehicles/CrashVehicle_Blue"), carOnePosition, Quaternion.Euler(carOneQuaternion));
        carTwo = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/CrashVehicles/CrashVehicle_Yellow"), carTwoPosition, Quaternion.Euler(carTwoQuaternion));

        yield return new WaitForSeconds(1); // waits for cars to spawn and be in position

        CrashVehicle carOneCV = carOne.GetComponent<CrashVehicle>();
        CrashVehicle carTwoCV = carTwo.GetComponent<CrashVehicle>();

        carOneCV.Mode = CrashVehicle.DriveMode.ACCELERATE;
        carTwoCV.Mode = CrashVehicle.DriveMode.ACCELERATE;

        Debug.Log(TAG + ": Crash event started.");
        yield break;
    }

    private static void OnPlayerCollisionEnter()
    {
        Vector3 crashEpicenter = new Vector3(1609.12f, 52.06f, 2839.1f);
        
        // limit spawning so vehicle won't spawn in crash event area
        GleyTrafficSystem.Manager.ClearTrafficOnArea(crashEpicenter, 100.0f, EventManager.Instance.PlayerVehicle);
        GleyTrafficSystem.Manager.SetTrafficDensity(1);

        Debug.Log(TAG + ": Traffic cleared.");
    }
}
