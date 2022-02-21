using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class IndicatorEvent : AutonomousEvent, UpdateEvent
{
    public override string Tag { get { return "IndicatorEvent"; } }
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
        EventManager.Instance.StartWatch();
    }

    public override void StopEvent()
    {
        indicator.SetActive(false);
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
