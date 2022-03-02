using System;
using System.Collections;
using UnityEngine;
using GleyTrafficSystem;

public class CrashEvent : AutonomousEvent
{
    public override string Tag { get { return "CrashEvent"; } }
    private static bool crashSoundPlayed = false;
    private static bool vehiclesVisible = false;
    private static Vector3 crashEpicenter = new Vector3(1609.05f, 53.07f, 2839.13f);
    private GameObject carOne;
    private GameObject carTwo;
    private BoxCollider carOneBody;
    private BoxCollider carTwoBody;
    private GameObject despawnTrafficVehicles;
    private GameObject despawnCrashVehicles;

    public override void StartEvent()
    {
        Vector3 crashEventPosition = new Vector3(1609.96f, 52.06f, 2793.8f);
        Vector3 despawnTrafficVehiclesPosition = new Vector3(1610.65f, 52.06f, 2741.06f);
        Vector3 despawnCrashVehiclesPosition = new Vector3(1529.82f, 53.07f, 2838.74f);

        // start pathing job to event location and spawn crash event via callback
        EventManager.Instance.PlayerVehicleAutonomous.StartPathing(TrafficManager.Instance.GetForwardWaypoint(
                EventManager.Instance.PlayerVehicle.gameObject, EventManager.Instance.PlayerVehicle.transform.forward), 
                GleyTrafficSystem.Manager.GetClosestWaypoint(crashEventPosition), OnAtEventPosition);
        EventLogger.Log(Tag, "Vehicle pathing to crash event location.");

        // intialize player detection object for despawning other traffic vehicles
        if (!despawnTrafficVehicles)
        {
            despawnTrafficVehicles = new GameObject("DespawnTrafficVehicles");
            BoxCollider despawnTrafficVehicleCollider = despawnTrafficVehicles.AddComponent<BoxCollider>();
            despawnTrafficVehicleCollider.center = despawnTrafficVehiclesPosition;
            despawnTrafficVehicleCollider.size = new Vector3(6f, 6f, 6f);
            despawnTrafficVehicleCollider.isTrigger = true;
            DetectPlayerCollision detectPlayerCollisionTrafficVehicles = despawnTrafficVehicles.AddComponent<DetectPlayerCollision>();
            detectPlayerCollisionTrafficVehicles.EnterAction = DespawnTraffic;
            detectPlayerCollisionTrafficVehicles.DeleteGameObjectOnEnter = true;
        }
        

        // intialize player detection object for despawning other traffic vehicles
        if (!despawnCrashVehicles)
        {
            despawnCrashVehicles = new GameObject("DespawnCrashVehicles");
            BoxCollider despawnCrashVehiclesCollider = despawnCrashVehicles.AddComponent<BoxCollider>();
            despawnCrashVehiclesCollider.center = despawnCrashVehiclesPosition;
            despawnCrashVehiclesCollider.size = new Vector3(6f, 6f, 6f);
            despawnCrashVehiclesCollider.isTrigger = true;
            DetectPlayerCollision detectPlayerCollisionCrashVehicles = despawnCrashVehicles.AddComponent<DetectPlayerCollision>();
            detectPlayerCollisionCrashVehicles.EnterAction = OnEventFinish;
            detectPlayerCollisionCrashVehicles.DeleteGameObjectOnEnter = true;
        }
        
    }

    public override void StopEvent()
    {
        EventManager.Instance.PlayerVehicleAutonomous.DisposePathing();
    }

    public override void UpdateEvent()
    {
        if (!carOne || !carTwo || vehiclesVisible) return;

        CheckVehiclesInCameraView();
    }

    // Checks if crash vehicles are within camera view and objects are not obstructing
    public void CheckVehiclesInCameraView()
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(GameMaster.Instance.MainCamera);
        if (GeometryUtility.TestPlanesAABB(planes, carOneBody.bounds) || GeometryUtility.TestPlanesAABB(planes, carTwoBody.bounds))
        {
            LayerMask mask = (1 << 13) | (1 << 14);
            RaycastHit carOneHit;
            Debug.Log("Vehicles within camera view.");
            if (Physics.Raycast(EventManager.Instance.PlayerVehicle.transform.position, carOne.transform.position, out carOneHit, Mathf.Infinity, layerMask: mask))
            {
                Debug.Log(carOneHit.transform.tag);
                if (carOneHit.transform.tag == "CrashVehicle")
                {
                    Debug.Log("Vehicles not obstructed within camera view.");

                    vehiclesVisible = true;
                    EventLogger.LogTimer(Tag, "One or more crash vehicles are visible.", EventManager.Instance.TimeElapsed.Elapsed);
                    return;
                }
            }

            RaycastHit carTwoHit;
            if (Physics.Raycast(EventManager.Instance.PlayerVehicle.transform.position, carOne.transform.position, out carTwoHit, Mathf.Infinity, layerMask: mask))
            {
                Debug.Log(carOneHit.transform.tag);
                if (carTwoHit.transform.tag == "CrashVehicle")
                {
                    Debug.Log("Vehicles not obstructed within camera view.");

                    vehiclesVisible = true;
                    EventLogger.LogTimer(Tag, "One or more crash vehicles are visible.", EventManager.Instance.TimeElapsed.Elapsed);
                    return;
                }
            }
        }
    }

    public static void PlayCrashSound()
    {
        if (crashSoundPlayed) return;
        AudioSource.PlayClipAtPoint(Resources.Load<AudioClip>("Audio/Car_Crash"), 0.2f * crashEpicenter + 0.8f * EventManager.Instance.PlayerVehicle.transform.position);
        crashSoundPlayed = true;
    }

    public void OnEventFinish()
    {
        if (carOne) UnityEngine.Object.Destroy(carOne);
        if (carTwo) UnityEngine.Object.Destroy(carTwo);
        if (despawnTrafficVehicles) UnityEngine.Object.Destroy(despawnTrafficVehicles);
        if (despawnCrashVehicles) UnityEngine.Object.Destroy(despawnCrashVehicles);
        GleyTrafficSystem.Manager.SetTrafficDensity(20);
    }

    private void OnAtEventPosition()
    {
        EventManager.Instance.StartChildCoroutine(SpawnCrashEvent());
    }

    private IEnumerator SpawnCrashEvent()
    {
        EventManager.Instance.StartWatch();

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

        float distanceToCrashMeters = Vector3.Distance(crashEpicenter, EventManager.Instance.PlayerVehicle.transform.position);
        float distanceToCrashFeet = distanceToCrashMeters * 3.28084f;
        EventLogger.LogTimer(Tag, String.Format("Distance from vehicle to crash epicenter: {0} m / {1} ft", distanceToCrashMeters, distanceToCrashFeet), EventManager.Instance.TimeElapsed.Elapsed);

        carOneCV.Mode = CrashVehicle.DriveMode.ACCELERATE;
        carTwoCV.Mode = CrashVehicle.DriveMode.ACCELERATE;

        EventLogger.LogTimer(Tag, String.Format("Crash vehicles now spawned and moving toward each other."), EventManager.Instance.TimeElapsed.Elapsed);
        
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
}
