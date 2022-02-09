using System.Collections;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public const string TAG = "EventManager";
    [SerializeField] private bool debugEnabled = false;
    [SerializeField] private float timeBeforeEvents = 3600f;
    [SerializeField] private float timeBetweenNextEventUpper = 900f;
    [SerializeField] private float timeBetweenNextEventLower = 300f;
    [SerializeField] private List<UniqueEvent> eventOrder;
    private static EventManager _instance;
    private List<AutonomousEvent> events;
    private Stopwatch timeElapsed;
    private UpdateEvent updateEvent;
    private int currentDebugIndex = 0;
    private bool initialized = false;
    private bool eventInProgress = false;
    
    public static EventManager Instance { get { return _instance; } }
    public GameObject PlayerVehicle { get; set; }
    public AutonomousVehicle PlayerVehicleAutonomous { get; set; }

    public enum UniqueEvent { ControlLossEvent, CrashEvent, IndicatorEvent, MergeFailEvent }
    
    public void Initialize(GameObject playerVehicle)
    {
        EventLogger.Initialize();
        PlayerVehicle = playerVehicle;
        PlayerVehicleAutonomous = PlayerVehicle.GetComponent<AutonomousVehicle>();
        events = new List<AutonomousEvent>();

        if (debugEnabled)
        {
            events.Add(new ControlLossEvent());
            events.Add(new IndicatorEvent(PlayerVehicleAutonomous.GetBatteryIndicator(), PlayerVehicleAutonomous.GetBatteryIndicatorSound()));
            events.Add(new CrashEvent());
            events.Add(new MergeFailEvent());
        }
        else
        {
            // Insert backwards so we can easily remove events from array
            for (int i = eventOrder.Count - 1; i >= 0; i--)
            {
                switch (eventOrder[i])
                {
                    case UniqueEvent.ControlLossEvent:
                        events.Add(new ControlLossEvent());
                        break;
                    case UniqueEvent.IndicatorEvent:
                        events.Add(new IndicatorEvent(PlayerVehicleAutonomous.GetBatteryIndicator(), PlayerVehicleAutonomous.GetBatteryIndicatorSound()));
                        break;
                    case UniqueEvent.CrashEvent:
                        events.Add(new CrashEvent());
                        break;
                    case UniqueEvent.MergeFailEvent:
                        events.Add(new MergeFailEvent());
                        break;
                }
            }
            StartCoroutine(QueueNextEvent(firstEvent: true));
        }
        
        timeElapsed = new Stopwatch();
        initialized = true;
        
        UnityEngine.Debug.Log(TAG + ": Initialized");
    }

    // Allows for coroutines to be triggered from any class
    public void StartChildCoroutine(IEnumerator coroutine)
    {
        StartCoroutine(coroutine);
    }

    // Start stopwatch
    public void StartWatch(string eventTag)
    {
        if (!initialized) return;
        
        EventLogger.Log(eventTag, "Event and stopwatch started.");
        timeElapsed.Start();
    }

    // Stop and reset stopwatch
    public void StopWatch(string eventTag)
    {
        if (!initialized) return;

        timeElapsed.Stop();
        TimeSpan ts = timeElapsed.Elapsed;
        EventLogger.Log(eventTag, "Event stopped.");
        EventLogger.Log(eventTag, "Driver took " + ts.Seconds + " seconds and " + ts.Milliseconds + " milliseconds to take control of autonomous vehicle.");
        timeElapsed.Reset();
    }

    // Stop stopwatch and queue next event when user attempts to take control of vehicle
    public void HandleAutonomousDisabled()
    {
        if (!initialized || !eventInProgress) return;

        updateEvent = null;
        eventInProgress = false;

        if (debugEnabled)
        {
            events[currentDebugIndex].StopEvent();
            return;
        }

        int eventIndex = events.Count - 1;
        events[eventIndex].StopEvent();
        events.RemoveAt(eventIndex);

        if (events.Count <= 0) return; // stop event cycle if no more events are in list

        // queue next event
        if (events[eventIndex] is UpdateEvent) updateEvent = (UpdateEvent) events[eventIndex]; // set update event if update required
        StartCoroutine(QueueNextEvent());
        eventInProgress = false;
    }

    // Queues the next/initial events after a random interval of time within a specified range has passed
    // and when the autonomous mode is enabled.
    private IEnumerator QueueNextEvent(bool firstEvent = false)
    {
        yield return new WaitForSeconds(firstEvent ? 
            timeBeforeEvents + UnityEngine.Random.Range(timeBetweenNextEventLower, timeBetweenNextEventUpper) : 
            UnityEngine.Random.Range(timeBetweenNextEventLower, timeBetweenNextEventUpper));
        while (!PlayerVehicleAutonomous.IsInAutonomous) yield return null;
        events[events.Count - 1].StartEvent();
        eventInProgress = true;
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
        if (!initialized || !PlayerVehicleAutonomous.IsInAutonomous) return;

        if (debugEnabled)
        {
            if (Input.GetKeyDown(KeyCode.Keypad1))
            {
                events[0].StartEvent(); // sets random waypoints for autonomous vehicle to emulate control loss
                updateEvent = (UpdateEvent) events[0];
                eventInProgress = true;
                currentDebugIndex = 0;
            }

            if (Input.GetKeyDown(KeyCode.Keypad2))
            {
                events[1].StartEvent(); // toggles the indicator event
                updateEvent = (UpdateEvent) events[1];
                eventInProgress = true;
                currentDebugIndex = 1;
            }

            if (Input.GetKeyDown(KeyCode.Keypad3))
            {
                events[2].StartEvent(); // spawns car to crash at crash event location
                eventInProgress = true;
                currentDebugIndex = 2;
            }

            if (Input.GetKeyDown(KeyCode.Keypad4))
            {
                events[3].StartEvent(); // moves car to merge fail event location
                eventInProgress = true;
                currentDebugIndex = 3;
            }
        }
            
        if (updateEvent != null) updateEvent.UpdateEvent();
    }
}
