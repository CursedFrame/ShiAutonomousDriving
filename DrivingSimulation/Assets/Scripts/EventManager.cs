using System.Collections;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public const string TAG = "EventManager";

    [Tooltip("Enables/disables debugging mode. Debugging mode allows for events to be called via the Numpad 1-4 keys and no timer is used to queue the next events.")]
    [SerializeField] private bool debugEnabled = false;

    [Tooltip("Enables/disables shuffling of event list. Useful for randomizing event order.")]
    [SerializeField] private bool shuffleModeEnabled = false;

    [Tooltip("The number of seconds before the first event starts.")]
    [SerializeField] private float timeBeforeEvents = 3600f;

    [Tooltip("The upper bound of the random range of time between event intervals.")]
    [SerializeField] private float timeBetweenNextEventUpper = 900f;

    [Tooltip("The lower bound of the random range of time between event intervals.")]
    [SerializeField] private float timeBetweenNextEventLower = 300f;

    [Tooltip("The event order/composition for simulation events.")]
    [SerializeField] private List<UniqueEvent> eventOrder;

    private static EventManager _instance;
    private static System.Random rng = new System.Random();
    private List<AutonomousEvent> events;
    private int currentIndex;
    private bool initialized = false;
    private int pauseCounter = 0;
    
    public static EventManager Instance { get { return _instance; } }
    public bool EventTimerStarted { get; set; } = false;
    public Stopwatch TimeElapsed { get; set; }
    public GameObject PlayerVehicle { get; set; }
    public AutonomousVehicle PlayerVehicleAutonomous { get; set; }

    public enum UniqueEvent { ControlLossEvent, CrashEvent, IndicatorEvent, MergeFailEvent }
    
    string URL = "https://docs.google.com/forms/d/e/1FAIpQLScwoRFutUp8B3sBv-I-GMDwjxyAYOwAu-gxCrWfx4gDzEQ4qg/viewform?usp=sf_link";
    public void Pause()
    {
        Time.timeScale = 0;
    }
    public void Resume()
    {
        Time.timeScale = 1;
    }
    public void OpenURL()
    {
        Application.OpenURL(URL);
    }

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

                // Log event order
                EventLogger.Log(TAG, "Events order shuffled. Order of events are...");
                for (int i = 0; i < events.Count; i++)
                {
                    EventLogger.Log(TAG, String.Format("{0}: {1}", events.Count - i, events[i].Tag));
                }
            }

            StartCoroutine(QueueNextEventWithDelay(firstEvent: true));
        }
        
        TimeElapsed = new Stopwatch();
        currentIndex = events.Count - 1;
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
        
        EventTimerStarted = true;
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
        if("CrashEvent" == events[currentIndex].Tag && pauseCounter == 2)
        {
        Pause();
        AudioListener.pause = true;
        OpenURL();
        }
        pauseCounter++;
    }

    // Stop stopwatch and queue next event when user attempts to take control of vehicle
    public void HandleAutonomousDisabled(string driverControlPreference = "NONE")
    {
        if (!initialized) return;

        if (!EventTimerStarted)
        {
            EventLogger.Log(AutonomousVehicle.TAG, String.Format("Driver used the {0} to take over the vehicle outside of event.", driverControlPreference));
            return;
        }

        // Stop and dispose of current event and queue the next event
        EventTimerStarted = false;
        events[currentIndex].StopEvent();
        StopWatch(driverControlPreference);

        if (debugEnabled) return;

        events.RemoveAt(currentIndex);
        currentIndex = events.Count - 1;

        if (events.Count == 0) return; // stop event cycle if no more events are in list

        // queue next event
        StartCoroutine(QueueNextEventWithDelay());
    }

    // 
    private IEnumerator QueueNextEventWithDelay(bool firstEvent = false)
    {
        // Wait for x seconds, where x is randomly picked from the specified range
        // If the first event is queued, adds the time specified before the first event starts 
        int seconds = (int) (firstEvent ? 
            timeBeforeEvents + UnityEngine.Random.Range(timeBetweenNextEventLower, timeBetweenNextEventUpper) : 
            UnityEngine.Random.Range(timeBetweenNextEventLower, timeBetweenNextEventUpper));
        EventLogger.Log(TAG, String.Format("Waiting for at least {0} seconds until queueing next event.", seconds));
        yield return new WaitForSecondsRealtime(seconds);

        // Queue next event on event finish
        yield return QueueNextEvent();
    }

    // Queues the next/initial events after a random interval of time within a specified range has passed
    // and when the autonomous mode is enabled.
    private IEnumerator QueueNextEvent(int eventDelay = 15)
    {
        // Delay event queue if not in autonomous
        if (!PlayerVehicleAutonomous.IsInAutonomous)
        {
            EventLogger.Log(TAG, "Vehicle is not in autonomous mode. Waiting for user to enable autonomous before starting next event.");

            int timeRemain = eventDelay;
            bool prevAutonomousState = PlayerVehicleAutonomous.IsInAutonomous;

            // Checks if vehicle is autonomous once every second
            while (timeRemain >= 0)
            {
                // If vehicle is now in autonomous, start counting down timer to queue event
                if (PlayerVehicleAutonomous.IsInAutonomous)
                {
                    if (prevAutonomousState != PlayerVehicleAutonomous.IsInAutonomous)
                    {
                        EventLogger.Log(TAG, String.Format("User has enabled autonomous mode, now waiting {0} seconds before starting next event.", eventDelay));
                    }

                    timeRemain--;
                }
                // Else, we reset the timer if driver regains vehicle control before event start
                else
                {
                    if (prevAutonomousState != PlayerVehicleAutonomous.IsInAutonomous)
                    {
                        EventLogger.Log(TAG, String.Format("User has disabled autonomous mode before event started. Resetting delay timer."));
                    }
                    
                    timeRemain = eventDelay;
                }

                prevAutonomousState = PlayerVehicleAutonomous.IsInAutonomous;
                yield return new WaitForSecondsRealtime(1);
            }
        }

        events[currentIndex].StartEvent();
        yield break;
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
                currentIndex = 0;
            }

            if (Input.GetKeyDown(KeyCode.Keypad2))
            {
                events[1].StartEvent(); // toggles the indicator event
                currentIndex = 1;
            }

            if (Input.GetKeyDown(KeyCode.Keypad3))
            {
                events[2].StartEvent(); // spawns car to crash at crash event location
                currentIndex = 2;
            }

            if (Input.GetKeyDown(KeyCode.Keypad4))
            {
                events[3].StartEvent(); // moves car to merge fail event location
                currentIndex = 3;
            }
        }

        if (EventTimerStarted) events[currentIndex].UpdateEvent();
    }
}
