using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutonomousVehicle : MonoBehaviour
{
    private bool autonomousEnabled = false;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha9)){
            if (autonomousEnabled){
                GleyTrafficSystem.Manager.StartVehicleDriving(this.gameObject);
            } else {
                GleyTrafficSystem.Manager.StopVehicleDriving(this.gameObject);
            }
            autonomousEnabled = !autonomousEnabled;
        }
    }
}
