using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static string TAG = "EventManager";
    // events
    private IndicatorEvent indicatorEvent;
    private CrashEvent crashEvent;
    private ControlLossEvent controlLossEvent;
    // other
    public AutonomousVehicle playerVehicle;
    private static EventManager _instance;
    public static EventManager Instance { get {return _instance; } }
    public bool initialized = false;
    
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }

    public void Initialize(AutonomousVehicle playerVehicle){
        this.playerVehicle = playerVehicle;
        indicatorEvent = new IndicatorEvent(playerVehicle.BatteryIndicator, playerVehicle.BatteryIndicatorSound);
        crashEvent = new CrashEvent(playerVehicle);
        controlLossEvent = new ControlLossEvent(playerVehicle.gameObject);
        initialized = true;
        
        Debug.Log(TAG + ": Initialized");
    }

    // Update is called once per frame
    void Update()
    {
        if (!initialized || !playerVehicle.IsInAutonomousMode()) return;

        if (Input.GetKeyDown(KeyCode.Alpha5)){
            controlLossEvent.ToggleControlLoss(); // sets random waypoints for autonomous vehicle to emulate control loss
        }
        if (Input.GetKeyDown(KeyCode.Alpha6)){
            indicatorEvent.ToggleIndicator(); // toggles the indicator event
        }
        if (Input.GetKeyDown(KeyCode.Alpha7)){
            crashEvent.StartCrashEvent(); // spawns car to crash at crash event location
        }
        
        indicatorEvent.UpdateIndicator();
        controlLossEvent.UpdateControlLoss();
    }

    // Allows for coroutines to be triggered from any class
    public void StartChildCoroutine(IEnumerator coroutine){
        StartCoroutine(coroutine);
    }
}
