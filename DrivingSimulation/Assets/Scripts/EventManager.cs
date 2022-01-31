using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public const string TAG = "EventManager";
    [SerializeField] private bool timerTest = false;
    [SerializeField] private float timeBeforeEvents = 3600f;
    [SerializeField] private float timeBetweenNextEventUpper = 900f;
    [SerializeField] private float timeBetweenNextEventLower = 300f;
    private static EventManager _instance;
    private bool initialized = false;
    private bool timerStarted = false;

    public static EventManager Instance { get { return _instance; } }
    public GameObject PlayerVehicle { get; set; }
    public AutonomousVehicle PlayerVehicleAutonomous { get; set; }
    
    public void Initialize(GameObject playerVehicle)
    {
        PlayerVehicle = playerVehicle;
        PlayerVehicleAutonomous = PlayerVehicle.GetComponent<AutonomousVehicle>();
        IndicatorEvent.Initialize();
        initialized = true;
        
        Debug.Log(TAG + ": Initialized");
    }

    // Allows for coroutines to be triggered from any class
    public void StartChildCoroutine(IEnumerator coroutine)
    {
        StartCoroutine(coroutine);
    }

    // Manages event life cycle via time
    private IEnumerator Timer()
    {
        yield return new WaitForSeconds(timeBeforeEvents + Random.Range(timeBetweenNextEventLower, timeBetweenNextEventUpper));
        yield return WaitForAutonomous();
        ControlLossEvent.ToggleEvent();

        yield return new WaitForSeconds(Random.Range(timeBetweenNextEventLower, timeBetweenNextEventUpper));
        yield return WaitForAutonomous();
        IndicatorEvent.ToggleEvent();

        yield return new WaitForSeconds(Random.Range(timeBetweenNextEventLower, timeBetweenNextEventUpper));
        yield return WaitForAutonomous();
        CrashEvent.StartEvent();

        yield return new WaitForSeconds(Random.Range(timeBetweenNextEventLower, timeBetweenNextEventUpper));
        yield return WaitForAutonomous();
        MergeFailEvent.StartEvent();

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
            ControlLossEvent.ToggleEvent(); // sets random waypoints for autonomous vehicle to emulate control loss
        }

        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            IndicatorEvent.ToggleEvent(); // toggles the indicator event
        }

        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            CrashEvent.StartEvent(); // spawns car to crash at crash event location
        }

        if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            MergeFailEvent.StartEvent(); // spawns car to crash at crash event location
        }
        
        IndicatorEvent.UpdateIndicator();
        ControlLossEvent.UpdateControlLoss();
    }
}
