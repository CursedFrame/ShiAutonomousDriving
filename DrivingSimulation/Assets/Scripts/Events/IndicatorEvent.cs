using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndicatorEvent
{
    public static string TAG = "IndicatorEvent";
    private const float blinkTime = 0.5f;
    private bool indicatorOn = false;
    private float currentTime;    
    private GameObject indicator;
    private AudioSource indicatorBeep;

    public IndicatorEvent(GameObject indicator, AudioSource indicatorBeep){
        this.indicator = indicator;
        this.indicatorBeep = indicatorBeep;
    }

    public void ToggleIndicator(){
        indicatorOn = !indicatorOn;
    }

    public void UpdateIndicator()
    {
        if (indicatorOn)
        {
            if (Time.realtimeSinceStartup - currentTime > blinkTime)
            {
                currentTime = Time.realtimeSinceStartup;
                if (indicator.activeSelf){
                    indicator.SetActive(false);
                } else {
                    indicator.SetActive(true);
                    indicatorBeep.Play();
                }
            }
        }
    }

    public bool GetIndicatorOn(){
        return indicatorOn;
    }
}
