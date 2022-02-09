using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AutonomousEvent
{
    public abstract void StartEvent();
    public abstract void StopEvent();
    
}

public interface UpdateEvent
{
    void UpdateEvent();
}
