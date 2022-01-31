using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class IndicatorEvent
{
    public const string TAG = "IndicatorEvent";
    private const float blinkTime = 0.5f;
    private static float currentTime;    
    private static GameObject indicator;
    private static AudioSource indicatorBeep;
    private static bool indicatorOn = false;

    public static void Initialize()
    {
        indicator = EventManager.Instance.PlayerVehicleAutonomous.GetBatteryIndicator();
        indicatorBeep = EventManager.Instance.PlayerVehicleAutonomous.GetBatteryIndicatorSound();
    }

    public static void ToggleEvent()
    {
        if (indicatorOn) indicator.SetActive(false);
        indicatorOn = !indicatorOn;
    }

    public static void UpdateIndicator()
    {
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
