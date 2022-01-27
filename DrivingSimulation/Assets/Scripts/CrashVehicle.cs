using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GleyTrafficSystem;

public class CrashVehicle : MonoBehaviour
{
    public Rigidbody rBody;
    public VehicleLightsComponent vehicleLights;
    public BoxCollider boxCollider;
    public ParticleSystem engineSmoke;
    public float moveForce = 100.0f;
    private DriveMode mode = DriveMode.STOP;
    public enum DriveMode {
        ACCELERATE,
        DECELERATE,
        STOP
    }
    void Start(){
        vehicleLights.Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)){
            mode = DriveMode.ACCELERATE;
            Debug.Log("Changed crash vehicle drive mode to: ACCELERATE");
        }

        vehicleLights.UpdateLights(); 
    }
    void FixedUpdate(){
        switch(mode){
            case DriveMode.STOP:
                break;
            case DriveMode.ACCELERATE:
                Accelerate();
                break;
            case DriveMode.DECELERATE:
                Decelerate();
                break;
        }
    }

    public void Accelerate(){
        rBody.AddForce(transform.forward * rBody.mass * moveForce);
    }

    public void Decelerate(){
        rBody.velocity = rBody.velocity * 0.95f;
        if (Mathf.Approximately(rBody.velocity.magnitude, 0.0f)){
            mode = DriveMode.STOP;
            Debug.Log("Changed crash vehicle drive mode to: STOP");
        } 
    }

    public void OnTriggerEnter(Collider other){
        if (other.gameObject.tag == "CrashVehicle"){
            boxCollider.enabled = false;
            mode = DriveMode.DECELERATE;
            Debug.Log("Changed crash vehicle drive mode to: DECELERATE");
            vehicleLights.SetBlinker(BlinkType.Emergency);
            StartCoroutine(DoDelayedRandom(() => {
                engineSmoke.Play();
            }, 5.0f));
        }
    }

    public void SetDriveMode(DriveMode mode){
        this.mode = mode;
    }

    private IEnumerator DoDelayedRandom(System.Action action, float max){
        yield return new WaitForSeconds(Random.Range(0, max));
        if (action != null) action();
    }
}
