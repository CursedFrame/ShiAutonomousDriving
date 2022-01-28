using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GleyTrafficSystem;

public class CrashVehicle : MonoBehaviour
{
    public const string TAG = "CrashVehicle";
    [SerializeField] private Rigidbody rigidBody;
    [SerializeField] private VehicleLightsComponent vehicleLights;
    [SerializeField] private BoxCollider boxCollider;
    [SerializeField] private ParticleSystem engineSmoke;
    [SerializeField] private float moveForce = 100.0f;

    public DriveMode Mode { get; set; } = DriveMode.STOP;

    public enum DriveMode { ACCELERATE, DECELERATE, STOP }

    private void Start()
    {
        vehicleLights.Initialize();
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Mode = DriveMode.ACCELERATE;
            Debug.Log("Changed crash vehicle drive mode to: ACCELERATE");
        }

        vehicleLights.UpdateLights(); 
    }

    private void FixedUpdate()
    {
        switch (Mode)
        {
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

    private void Accelerate()
    {
        rigidBody.AddForce(transform.forward * rigidBody.mass * moveForce);
    }

    private void Decelerate()
    {
        rigidBody.velocity = rigidBody.velocity * 0.95f;

        if (Mathf.Approximately(rigidBody.velocity.magnitude, 0.0f))
        {
            Mode = DriveMode.STOP;
            Debug.Log("Changed crash vehicle drive mode to: STOP");
        } 
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "CrashVehicle")
        {
            boxCollider.enabled = false;
            Mode = DriveMode.DECELERATE;
            Debug.Log("Changed crash vehicle drive mode to: DECELERATE");
            vehicleLights.SetBlinker(BlinkType.Emergency);
            StartCoroutine(DoDelayedRandom(() => {
                engineSmoke.Play();
            }, 5.0f));
        }
    }

    private IEnumerator DoDelayedRandom(System.Action action, float max)
    {
        yield return new WaitForSeconds(Random.Range(0, max));
        if (action != null) action();
    }
}
