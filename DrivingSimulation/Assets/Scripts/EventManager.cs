using System.Collections;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public const string TAG = "EventManager";
    [SerializeField] private bool debugEnabled = false;
    [SerializeField] private bool shuffleModeEnabled = false;
    [SerializeField] private float timeBeforeEvents = 3600f;
    [SerializeField] private float timeBetweenNextEventUpper = 900f;
    [SerializeField] private float timeBetweenNextEventLower = 300f;
    [SerializeField] private List<UniqueEvent> eventOrder;
    private static EventManager _instance;
    private static System.Random rng = new System.Random();
    private List<AutonomousEvent> events;
    private UpdateEvent updateEvent;
    private int currentIndex;
    private bool initialized = false;
    
    public static EventManager Instance { get { return _instance; } }
    public bool EventInProgress { get; set; } = false;
    public Stopwatch TimeElapsed { get; set; }
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

            // Thanks to https://stackoverflow.com/questions/273313/randomize-a-listt
            if (shuffleModeEnabled)
            {
                events = events.OrderBy(a => rng.Next()).ToList();

                // Log event order for 
                UnityEngine.Debug.Log("Events order shuffled. Order of events are...");
                EventLogger.Log(TAG, "Events order shuffled. Order of events are...");
                for (int i = 0; i < events.Count; i++)
                {
                    EventLogger.Log(TAG, String.Format("{0}: {1}", i, events[i].Tag));
                    UnityEngine.Debug.Log(String.Format("{0}: {1}", i, events[i].Tag));
                }
            }

            StartCoroutine(QueueNextEvent(firstEvent: true));
        }
        
        TimeElapsed = new Stopwatch();
        initialized = true;
        
        UnityEngine.Debug.Log(TAG + ": Initialized");
    }

    // Allows for coroutines to be triggered from any class
    public void StartChildCoroutine(IEnumerator coroutine)
    {
        StartCoroutine(coroutine);
    }

    // Start stopwatch
    public void StartWatch()
    {
        if (!initialized) return;
        
        EventInProgress = true;
        TimeElapsed.Start();
        EventLogger.LogTimer(events[currentIndex].Tag, "Event and stopwatch started.", TimeElapsed.Elapsed);
    }

    // Stop and reset stopwatch
    public void StopWatch(string driverControlPreference)
    {
        if (!initialized) return;

        TimeElapsed.Stop();
        EventLogger.LogTimer(events[currentIndex].Tag, String.Format("Event stopped. Driver took control of autonomous vehicle using {0}.", driverControlPreference), TimeElapsed.Elapsed);
        TimeElapsed.Reset();
    }

    // Stop stopwatch and queue next event when user attempts to take control of vehicle
    public void HandleAutonomousDisabled(string driverControlPreference = "NONE")
    {
        if (!initialized || !EventInProgress)
        {
            EventLogger.Log(AutonomousVehicle.TAG, String.Format("Driver used the {0} to take over the vehicle outside of event.", driverControlPreference));
            return;
        }

        updateEvent = null;
        EventInProgress = false;

        if (debugEnabled)
        {
            events[currentIndex].StopEvent();
            StopWatch(driverControlPreference);
            return;
        }

        currentIndex = events.Count - 1;
        events[currentIndex].StopEvent();
        StopWatch(driverControlPreference);
        events.RemoveAt(currentIndex);

        if (events.Count <= 0) return; // stop event cycle if no more events are in list

        // queue next event
        if (events[currentIndex] is UpdateEvent) updateEvent = (UpdateEvent) events[currentIndex]; // set update event if update required
        StartCoroutine(QueueNextEvent());
    }

    // Queues the next/initial events after a random interval of time within a specified range has passed
    // and when the autonomous mode is enabled.
    private IEnumerator QueueNextEvent(bool firstEvent = false)
    {
        int seconds = (int) (firstEvent ? 
            timeBeforeEvents + UnityEngine.Random.Range(timeBetweenNextEventLower, timeBetweenNextEventUpper) : 
            UnityEngine.Random.Range(timeBetweenNextEventLower, timeBetweenNextEventUpper));
        EventLogger.Log(TAG, String.Format("Waiting for {0} seconds until queueing next event.", seconds));
        yield return new WaitForSeconds(seconds);
        while (!PlayerVehicleAutonomous.IsInAutonomous) yield return null;
        events[events.Count - 1].StartEvent();
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
                currentIndex = 0;
            }

            if (Input.GetKeyDown(KeyCode.Keypad2))
            {
                events[1].StartEvent(); // toggles the indicator event
                updateEvent = (UpdateEvent) events[1];
                currentIndex = 1;
            }

            if (Input.GetKeyDown(KeyCode.Keypad3))
            {
                events[2].StartEvent(); // spawns car to crash at crash event location
                updateEvent = (UpdateEvent) events[2];
                currentIndex = 2;
            }

            if (Input.GetKeyDown(KeyCode.Keypad4))
            {
                events[3].StartEvent(); // moves car to merge fail event location
                currentIndex = 3;
            }
        }
            
        if (updateEvent != null) updateEvent.UpdateEvent();
    }
}
