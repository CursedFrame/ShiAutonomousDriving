using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public const string TAG = "EventManager";
    public bool timerTest = false;
    private IndicatorEvent indicatorEvent;
    private CrashEvent crashEvent;
    private ControlLossEvent controlLossEvent;
    private static EventManager _instance;
    private bool initialized = false;
    private bool timerStarted = false;

    public static EventManager Instance { get {return _instance; } }
    public AutonomousVehicle PlayerVehicle { get; set; }
    
    public void Initialize(AutonomousVehicle playerVehicle)
    {
        PlayerVehicle = playerVehicle;
        indicatorEvent = new IndicatorEvent(playerVehicle.GetBatteryIndicator(), playerVehicle.GetBatteryIndicatorSound());
        crashEvent = new CrashEvent(playerVehicle);
        controlLossEvent = new ControlLossEvent(playerVehicle.gameObject);
        initialized = true;
        
        Debug.Log(TAG + ": Initialized");
    }

    // Allows for coroutines to be triggered from any class
    public void StartChildCoroutine(IEnumerator coroutine)
    {
        StartCoroutine(coroutine);
    }

    // Manages event life cycle via time
    public IEnumerator Timer()
    {
        yield return new WaitForSeconds(3600 + Random.Range(300, 900));
        controlLossEvent.ToggleControlLoss();
        yield return new WaitForSeconds(Random.Range(300, 900));
        indicatorEvent.ToggleIndicator();
        yield return new WaitForSeconds(Random.Range(300, 900));
        crashEvent.StartCrashEvent();
        yield return null;
    }

    // For debugging purposes
    public IEnumerator TimerTest()
    {
        yield return new WaitForSeconds(10 + Random.Range(5, 10));
        controlLossEvent.ToggleControlLoss();
        yield return new WaitForSeconds(Random.Range(5, 10));
        indicatorEvent.ToggleIndicator();
        yield return new WaitForSeconds(Random.Range(5, 10));
        crashEvent.StartCrashEvent();
        yield return null;
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } 
        else 
        {
            _instance = this;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (!initialized || !PlayerVehicle.IsInAutonomous) return;

        // if (!timerStarted)
        // {
        //     timerStarted = true;
        //     StartCoroutine(timerTest ? TimerTest() : Timer());
        // }

        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            controlLossEvent.ToggleControlLoss(); // sets random waypoints for autonomous vehicle to emulate control loss
        }

        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            indicatorEvent.ToggleIndicator(); // toggles the indicator event
        }

        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            crashEvent.StartCrashEvent(); // spawns car to crash at crash event location
        }
        
        indicatorEvent.UpdateIndicator();
        controlLossEvent.UpdateControlLoss();
    }
}
