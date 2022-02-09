using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class IndicatorEvent : AutonomousEvent, UpdateEvent
{
    public const string TAG = "IndicatorEvent";
    private const float blinkTime = 0.5f;
    private float currentTime;    
    private GameObject indicator;
    private AudioSource indicatorBeep;

    public IndicatorEvent(GameObject indicator, AudioSource indicatorBeep)
    {
        this.indicator = indicator;
        this.indicatorBeep = indicatorBeep;
    }

    public override void StartEvent()
    {
        EventManager.Instance.StartWatch(TAG);
    }

    public override void StopEvent()
    {
        indicator.SetActive(false);
        EventManager.Instance.StopWatch(TAG);
    }

    public void UpdateEvent()
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
