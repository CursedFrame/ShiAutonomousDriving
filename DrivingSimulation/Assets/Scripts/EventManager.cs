using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static string TAG = "EventManager";
    // events
    private IndicatorEvent indicatorEvent;
    private CrashEvent crashEvent;
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

    public void Initialize(){
        indicatorEvent = new IndicatorEvent(playerVehicle.GetBatteryIndicator(), playerVehicle.GetBatteryIndicatorSound());
        crashEvent = new CrashEvent();
        initialized = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!initialized) return;

        if (Input.GetKeyDown(KeyCode.Alpha6)){
            indicatorEvent.ToggleIndicator(); // toggles the indicator event
        }
        if (Input.GetKeyDown(KeyCode.Alpha7)){
            crashEvent.StartCrashEvent(); // spawns car to crash at crash event location
        }

        if (indicatorEvent.GetIndicatorOn()) indicatorEvent.UpdateIndicator();
    }
}
