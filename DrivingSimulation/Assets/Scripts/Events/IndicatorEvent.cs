using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndicatorEvent
{
    public const string TAG = "IndicatorEvent";
    private const float blinkTime = 0.5f;
    private float currentTime;    
    private GameObject indicator;
    private AudioSource indicatorBeep;
    private bool indicatorOn = false;

    public IndicatorEvent(GameObject indicator, AudioSource indicatorBeep)
    {
        this.indicator = indicator;
        this.indicatorBeep = indicatorBeep;
    }

    public void ToggleIndicator()
    {
        if (indicatorOn) indicator.SetActive(false);
        indicatorOn = !indicatorOn;
    }

    public void UpdateIndicator()
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
