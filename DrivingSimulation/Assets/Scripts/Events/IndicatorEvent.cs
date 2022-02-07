using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public static class IndicatorEvent
{
    public const string TAG = "IndicatorEvent";
    private const float blinkTime = 0.5f;
    private static bool indicatorOn = false;
    private static bool initialized = false;
    private static float currentTime;    
    private static GameObject indicator;
    private static AudioSource indicatorBeep;

    public static void Initialize()
    {
        indicator = EventManager.Instance.PlayerVehicleAutonomous.GetBatteryIndicator();
        indicatorBeep = EventManager.Instance.PlayerVehicleAutonomous.GetBatteryIndicatorSound();
        initialized = true;
    }

    public static void StartEvent()
    {
        if (!initialized) return;

        indicatorOn = true;
        EventManager.Instance.StartWatch(TAG);
    }

    public static void StopEvent()
    {
        indicatorOn = false;
        indicator.SetActive(false);
        EventManager.Instance.StopWatch(TAG);
    }

    public static void UpdateIndicator()
    {
        if (!initialized) return;

        if (indicatorOn)
        {
            if (Time.realtimeSinceStartup - currentTime > blinkTime)
            {
                currentTime = Time.realtimeSinceStartup;

                if (indicator.activeSelf)
                {
                    indicator.SetActive(false);
                } 
                else 
                {
                    indicator.SetActive(true);
                    indicatorBeep.Play();
                }
            }
        }
    }
}
