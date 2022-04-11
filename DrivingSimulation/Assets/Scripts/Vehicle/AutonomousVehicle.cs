using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GleyTrafficSystem;
using WindowsInput;
using WindowsInput.Native;
public class AutonomousVehicle : MonoBehaviour
{
    public const string TAG = "AutonomousVehicle";
    private const int LOGITECH_STEERING_WHEEL_INDEX = 0; // This may need to be changed, requires testing with wheel, which I do not have with me at the time of writing this
    private const int LOGITECH_STEERING_WHEEL_CROSS = 1;
    private const int LOGITECH_STEERING_WHEEL_SQUARE = 2;
    private const int LOGITECH_STEERING_WHEEL_CIRCLE = 3;
    private const int LOGITECH_STEERING_WHEEL_TRIANGLE = 4;
    
    
    private static InputSimulator inputSimulator;
    [SerializeField] private GameObject batteryIndicator;
    [SerializeField] private AudioSource batteryIndicatorSound;
    [SerializeField] private Camera playerCamera;
    private IEnumerator pathingRoutine;
    private Waypoint pathingEnd;
    private System.Action pathingCallbackFinish;
    private string buttonStatus;

    public bool IsInAutonomous { get; set; }
    public bool IsPathing { get; set; }
    public GameObject GetBatteryIndicator(){ return batteryIndicator; }
    public AudioSource GetBatteryIndicatorSound() { return batteryIndicatorSound; }
    
    public void StartPathing(Waypoint start, Waypoint end, System.Action callbackFinish = null)
    {
        IsPathing = true;

        pathingRoutine = Pathing(start, end, callbackFinish);
        pathingEnd = end;
        pathingCallbackFinish = callbackFinish;

        Debug.Log("Start pathing to event.");
        StartCoroutine(pathingRoutine);
    }

    public void ContinuePathing()
    {
        StartPathing(TrafficManager.Instance.GetForwardWaypoint(
            EventManager.Instance.PlayerVehicle.gameObject, EventManager.Instance.PlayerVehicle.transform.forward), 
            pathingEnd, pathingCallbackFinish);
        EventLogger.Log(TAG, "Continuing pathing to event.");
    }

    public void DisposePathing()
    {
        Debug.Log("Disposing of autonomous vehicle pathing.");

        IsPathing = false;

        pathingRoutine = null;
        pathingEnd = null;
        pathingCallbackFinish = null;
    }

    public IEnumerator Pathing(Waypoint start, Waypoint end, System.Action callback = null)
    {
        List<Waypoint> path = TrafficComponent.Instance.pathfinding.AStar(start, end);
        int nextPathIndex = 0;

        while (nextPathIndex != path.Count)
        {
            nextPathIndex++;
            if (nextPathIndex == path.Count) break;
            yield return GoToWaypoint(path[nextPathIndex]);
        }

        Debug.Log("Arrived at destination!");
        if (callback != null) callback();
        yield break;
    }

    private IEnumerator GoToWaypoint(Waypoint waypoint)
    {
        GleyTrafficSystem.Manager.SetNextWaypoint(this.gameObject, waypoint);
        Debug.Log(waypoint.name);
        while (Vector3.Distance(this.gameObject.transform.position, waypoint.position) > 5.0f) yield return null;
        yield break;
    }

    private void Start()
    {
        MoveTrafficSystem.Instance.Initialize(this.transform);
        EventManager.Instance.Initialize(this.gameObject);
        GameMaster.Instance.MainCamera = playerCamera;
        IsInAutonomous = true;
        IsPathing = false;
        inputSimulator = new InputSimulator();
    }

    public void Resume()
    {
        Time.timeScale = 1;
    }

    // Update is called once per frame
    private void Update()
    {
        // Manual toggle of autonomous mode
        if (Input.GetKeyDown(KeyCode.Keypad9) || LogitechGSDK.LogiButtonTriggered(LOGITECH_STEERING_WHEEL_INDEX, LOGITECH_STEERING_WHEEL_CIRCLE))
        {
            if (!IsInAutonomous)
            {   
                EventLogger.Log(TAG, "Driver used the TOGGLE BUTTON to enable autonomous mode.");
                
                // Continue pathing if should be pathing
                if (IsPathing)
                {
                    GleyTrafficSystem.Manager.StartPlayerVehicleDriving(this.gameObject, ContinuePathing);
                }
                else
                {
                    GleyTrafficSystem.Manager.StartPlayerVehicleDriving(this.gameObject);
                }

                IsInAutonomous = true;
            } 
            else 
            {
                DriverTakeControl("TOGGLE BUTTON");
            }
        }
        if(LogitechGSDK.LogiButtonTriggered(LOGITECH_STEERING_WHEEL_INDEX, LOGITECH_STEERING_WHEEL_CROSS))
        {
            Resume();
            AudioListener.pause = false;
        }

        // Due to limitations with the Vehicle Physics Pro package, we must simulate keyboard presses when
        // steering wheel buttons are pressed for toggles
        /*if (!IsInAutonomous)
        {
            // Vehicle start
            if (LogitechGSDK.LogiButtonIsPressed(LOGITECH_STEERING_WHEEL_INDEX, LOGITECH_STEERING_WHEEL_TRIANGLE))
            {
                inputSimulator.Keyboard.KeyDown(VirtualKeyCode.VK_K);
            }

            // Neutral
            if (LogitechGSDK.LogiButtonTriggered(LOGITECH_STEERING_WHEEL_INDEX, LOGITECH_STEERING_WHEEL_SQUARE))
            {
                inputSimulator.Keyboard.KeyPress(VirtualKeyCode.VK_N);
            }

            // Reverse
            if (LogitechGSDK.LogiButtonTriggered(LOGITECH_STEERING_WHEEL_INDEX, LOGITECH_STEERING_WHEEL_CROSS))
            {
                inputSimulator.Keyboard.KeyPress(VirtualKeyCode.VK_R);
            }

        }*/

        // Disables autonomous control when driver moves the steering wheel
        // or presses any pedal
        if (!IsInAutonomous) return;

        if (Input.GetAxis("Horizontal") > 0.1f)
        {
            DriverTakeControl("STEERING WHEEL - RIGHT");
            return;
        }

        if (Input.GetAxis("Horizontal") < -0.1f)
        {
            DriverTakeControl("STEERING WHEEL - LEFT");
            return;
        }

        if (Input.GetAxis("Vertical") > 0.1f)
        {
            DriverTakeControl("GAS PEDAL");
            return;
        }

        if (Input.GetAxis("Vertical") < -0.1f)
        {
            DriverTakeControl("BRAKE");
            return;
        }
    }

    private void DriverTakeControl(string methodTakenControl)
    {
        GleyTrafficSystem.Manager.StopVehicleDriving(this.gameObject);
        EventManager.Instance.HandleAutonomousDisabled(methodTakenControl);

        // Pause pathing if pathing
        if (IsPathing)
        {
            StopCoroutine(pathingRoutine);

            pathingRoutine = null;
            EventLogger.Log(TAG, "Pausing pathing to event.");
        }
        
        IsInAutonomous = false;
    }
}

