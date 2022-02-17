using System;
using System.Collections;
using UnityEngine;
using GleyTrafficSystem;

public class CrashEvent : AutonomousEvent, UpdateEvent
{
    public override string Tag { get { return "CrashEvent"; } }
    private static bool crashSoundPlayed = false;
    private static Vector3 crashEpicenter = new Vector3(1609.05f, 53.07f, 2839.13f);
    private GameObject carOne;
    private BoxCollider carOneBody;
    private GameObject carTwo;
    private BoxCollider carTwoBody;
    private GameObject despawnTrafficVehicles;
    private GameObject despawnCrashVehicles;
    private bool vehiclesVisible = false;

    public override void StartEvent()
    {
        Vector3 crashEventPosition = new Vector3(1609.96f, 52.06f, 2793.8f);
        Vector3 despawnTrafficVehiclesPosition = new Vector3(1610.65f, 52.06f, 2741.06f);
        Vector3 despawnCrashVehiclesPosition = new Vector3(1529.82f, 53.07f, 2838.74f);

        // start pathing job to event location and spawn crash event via callback
        EventManager.Instance.PlayerVehicleAutonomous.StartPathing(TrafficManager.Instance.GetClosestForwardWaypoint(
                EventManager.Instance.PlayerVehicle.gameObject, EventManager.Instance.PlayerVehicle.transform.forward), 
                GleyTrafficSystem.Manager.GetClosestWaypoint(crashEventPosition), OnAtEventPosition, DisposeEvent);
        EventLogger.Log(Tag, "Vehicle pathing to crash event location.");

        // intialize player detection object for despawning other traffic vehicles
        despawnTrafficVehicles = new GameObject("DespawnTrafficVehicles");
        BoxCollider despawnTrafficVehicleCollider = despawnTrafficVehicles.AddComponent<BoxCollider>();
        despawnTrafficVehicleCollider.center = despawnTrafficVehiclesPosition;
        despawnTrafficVehicleCollider.size = new Vector3(6f, 6f, 6f);
        despawnTrafficVehicleCollider.isTrigger = true;
        DetectPlayerCollision detectPlayerCollisionTrafficVehicles = despawnTrafficVehicles.AddComponent<DetectPlayerCollision>();
        detectPlayerCollisionTrafficVehicles.EnterAction = DespawnTraffic;
        detectPlayerCollisionTrafficVehicles.DeleteGameObjectOnEnter = true;

        // intialize player detection object for despawning other traffic vehicles
        despawnCrashVehicles = new GameObject("DespawnCrashVehicles");
        BoxCollider despawnCrashVehiclesCollider = despawnCrashVehicles.AddComponent<BoxCollider>();
        despawnCrashVehiclesCollider.center = despawnCrashVehiclesPosition;
        despawnCrashVehiclesCollider.size = new Vector3(6f, 6f, 6f);
        despawnCrashVehiclesCollider.isTrigger = true;
        DetectPlayerCollision detectPlayerCollisionCrashVehicles = despawnCrashVehicles.AddComponent<DetectPlayerCollision>();
        detectPlayerCollisionCrashVehicles.EnterAction = DespawnCrash;
        detectPlayerCollisionCrashVehicles.DeleteGameObjectOnEnter = true;
    }

    public override void StopEvent()
    {
        EventManager.Instance.StopWatch();
        EventManager.Instance.PlayerVehicleAutonomous.DisposePathing();
    }

    public void UpdateEvent()
    {
        if (!carOne || !carTwo || vehiclesVisible) return;

        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(GameMaster.Instance.MainCamera);
        if (GeometryUtility.TestPlanesAABB(planes, carOneBody.bounds) || GeometryUtility.TestPlanesAABB(planes, carTwoBody.bounds))
        {
            vehiclesVisible = true;
            EventLogger.Log(Tag, "One or more crash vehicles are visible.");
        }
    }

    public static void PlayCrashSound()
    {
        if (crashSoundPlayed) return;
        AudioSource.PlayClipAtPoint(Resources.Load<AudioClip>("Audio/Car_Crash"), 0.2f * crashEpicenter + 0.8f * EventManager.Instance.PlayerVehicle.transform.position);
        crashSoundPlayed = true;
    }

    public void DisposeEvent()
    {
        if (carOne) UnityEngine.Object.Destroy(carOne);
        if (carTwo) UnityEngine.Object.Destroy(carTwo);
        if (despawnTrafficVehicles) UnityEngine.Object.Destroy(despawnTrafficVehicles);
        if (despawnCrashVehicles) UnityEngine.Object.Destroy(despawnCrashVehicles);
    }

    private void OnAtEventPosition()
    {
        EventManager.Instance.StartChildCoroutine(SpawnCrashEvent());
    }

    private IEnumerator SpawnCrashEvent()
    {
        // car spawning
        Vector3 carOnePosition = new Vector3(1576.75f, 52.06f, 2837.51f), carOneQuaternion = new Vector3(0, 90f, 0);
        Vector3 carTwoPosition = new Vector3(1649.75f, 52.06f, 2840.94f), carTwoQuaternion = new Vector3(0, 265f, 0);

        carOne = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/CrashVehicles/CrashVehicle_Blue"), carOnePosition, Quaternion.Euler(carOneQuaternion));
        carOneBody = carOne.transform.GetComponentInChildren<BoxCollider>();
        carTwo = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/CrashVehicles/CrashVehicle_Yellow"), carTwoPosition, Quaternion.Euler(carTwoQuaternion));
        carTwoBody = carTwo.transform.GetComponentInChildren<BoxCollider>();

        yield return new WaitForSeconds(1); // waits for cars to spawn and be in position

        CrashVehicle carOneCV = carOne.GetComponent<CrashVehicle>();
        CrashVehicle carTwoCV = carTwo.GetComponent<CrashVehicle>();

        carOneCV.Mode = CrashVehicle.DriveMode.ACCELERATE;
        carTwoCV.Mode = CrashVehicle.DriveMode.ACCELERATE;

        EventManager.Instance.StartWatch();
        
        float distanceToCrashMeters = Vector3.Distance(crashEpicenter, EventManager.Instance.PlayerVehicle.transform.position);
        float distanceToCrashFeet = distanceToCrashMeters * 3.28084f;

        EventLogger.Log(Tag, String.Format("Distance from vehicle to crash epicenter: {0} m / {1} ft", distanceToCrashMeters, distanceToCrashFeet));
        EventLogger.Log(Tag, "Crash vehicles now spawned and moving toward each other.");
        
        UnityEngine.Debug.Log(Tag + ": Crash event started.");

        yield break;
    }

    private void DespawnTraffic()
    {
        Vector3 crashEpicenter = new Vector3(1609.12f, 52.06f, 2839.1f);
        
        // limit spawning so vehicle won't spawn in crash event area
        GleyTrafficSystem.Manager.ClearTrafficOnArea(crashEpicenter, 100.0f, EventManager.Instance.PlayerVehicle);
        GleyTrafficSystem.Manager.SetTrafficDensity(1);

        UnityEngine.Debug.Log(Tag + ": Traffic cleared.");
    }

    private void DespawnCrash()
    {
        DisposeEvent();
    }
}
