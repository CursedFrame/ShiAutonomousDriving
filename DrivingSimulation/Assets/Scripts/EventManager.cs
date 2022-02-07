using System.Collections;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public const string TAG = "EventManager";
    [SerializeField] private bool timerTest = false;
    [SerializeField] private float timeBeforeEvents = 3600f;
    [SerializeField] private float timeBetweenNextEventUpper = 900f;
    [SerializeField] private float timeBetweenNextEventLower = 300f;
    private static EventManager _instance;
    private Stopwatch timeElapsed;
    private bool initialized = false;
    private bool timerStarted = false;
    private AutonomousEvent currentEvent = AutonomousEvent.None;

    public static EventManager Instance { get { return _instance; } }
    public GameObject PlayerVehicle { get; set; }
    public AutonomousVehicle PlayerVehicleAutonomous { get; set; }

    public enum AutonomousEvent { None, ControlLossEvent, CrashEvent, IndicatorEvent, MergeFailEvent }
    
    public void Initialize(GameObject playerVehicle)
    {
        PlayerVehicle = playerVehicle;
        PlayerVehicleAutonomous = PlayerVehicle.GetComponent<AutonomousVehicle>();
        IndicatorEvent.Initialize();
        timeElapsed = new Stopwatch();
        initialized = true;
        
        UnityEngine.Debug.Log(TAG + ": Initialized");
    }

    // Allows for coroutines to be triggered from any class
    public void StartChildCoroutine(IEnumerator coroutine)
    {
        StartCoroutine(coroutine);
    }

    public void StartWatch(string eventTag)
    {
        if (!initialized) return;
        
        EventLogger.Write(eventTag + ": Event and stopwatch started.");
        timeElapsed.Start();
    }

    public void StopWatch(string eventTag)
    {
        if (!initialized) return;

        timeElapsed.Stop();
        TimeSpan ts = timeElapsed.Elapsed;
        string elapsedTime = String.Format("{0:00}:{1:00}", ts.Seconds, ts.Milliseconds / 10);
        EventLogger.Write(eventTag + ": " + elapsedTime + " to take control of autonomous vehicle.");
        timeElapsed.Reset();
    }

    // Stop stopwatch when user attempts to take control of vehicle
    public void HandleAutonomousDisabled()
    {
        if (currentEvent == AutonomousEvent.None) return;

        switch (currentEvent)
        {
            case AutonomousEvent.ControlLossEvent:
                ControlLossEvent.StopEvent();
                break;
            case AutonomousEvent.CrashEvent:
                CrashEvent.StopEvent();
                break;
            case AutonomousEvent.IndicatorEvent:
                IndicatorEvent.StopEvent();
                break;
            case AutonomousEvent.MergeFailEvent:
                MergeFailEvent.StopEvent();
                break;
        }

        currentEvent = AutonomousEvent.None;
    }

    // Manages event life cycle via time
    private IEnumerator Timer()
    {
        yield return new WaitForSeconds(timeBeforeEvents + UnityEngine.Random.Range(timeBetweenNextEventLower, timeBetweenNextEventUpper));
        yield return WaitForAutonomous();
        ControlLossEvent.StartEvent();
        currentEvent = AutonomousEvent.ControlLossEvent;

        yield return new WaitForSeconds(UnityEngine.Random.Range(timeBetweenNextEventLower, timeBetweenNextEventUpper));
        yield return WaitForAutonomous();
        IndicatorEvent.StartEvent();
        currentEvent = AutonomousEvent.IndicatorEvent;

        yield return new WaitForSeconds(UnityEngine.Random.Range(timeBetweenNextEventLower, timeBetweenNextEventUpper));
        yield return WaitForAutonomous();
        CrashEvent.StartEvent();
        currentEvent = AutonomousEvent.CrashEvent;

        yield return new WaitForSeconds(UnityEngine.Random.Range(timeBetweenNextEventLower, timeBetweenNextEventUpper));
        yield return WaitForAutonomous();
        MergeFailEvent.StartEvent();
        currentEvent = AutonomousEvent.MergeFailEvent;

        yield return null;
    }

    private IEnumerator WaitForAutonomous(){
        while (!PlayerVehicleAutonomous.IsInAutonomous) yield return null;
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
        if (!initialized || !PlayerVehicleAutonomous.IsInAutonomous) return;

        // if (!timerStarted)
        // {
        //     timerStarted = true;
        //     StartCoroutine(Timer());
        // }

        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            ControlLossEvent.StartEvent(); // sets random waypoints for autonomous vehicle to emulate control loss
            currentEvent = AutonomousEvent.ControlLossEvent;
        }

        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            IndicatorEvent.StartEvent(); // toggles the indicator event
            currentEvent = AutonomousEvent.IndicatorEvent;
        }

        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            CrashEvent.StartEvent(); // spawns car to crash at crash event location
            currentEvent = AutonomousEvent.CrashEvent;
        }

        if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            MergeFailEvent.StartEvent(); // moves car to merge fail event location
            currentEvent = AutonomousEvent.MergeFailEvent;
        }
        
        IndicatorEvent.UpdateIndicator();
        ControlLossEvent.UpdateControlLoss();
    }
}
