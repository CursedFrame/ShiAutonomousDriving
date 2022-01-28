using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectPlayerCollision : MonoBehaviour
{
    public const string TAG = "DetectPlayerCollision";
    
    [HideInInspector] public System.Action EnterAction { get; set; }
    [HideInInspector] public System.Action ExitAction { get; set; }
    [HideInInspector] public bool DeleteGameObjectOnEnter { get; set; } = false;
    [HideInInspector] public bool DeleteGameObjectOnExit { get; set; } = false;

    private void OnTriggerEnter(Collider collider){
        if (EnterAction == null) return;
        if (collider.tag == "Player")
        {
            StartCoroutine(DoDelayed(() => {
                EnterAction();
            }, DeleteGameObjectOnEnter));
        }
    }

    private void OnTriggerExit(Collider collider){
        if (ExitAction == null) return;
        if (collider.tag == "Player") 
        {
            StartCoroutine(DoDelayed(() => {
                ExitAction();
            }, DeleteGameObjectOnExit));
        }
    }

    private IEnumerator DoDelayed(System.Action action, bool deleteAfterAction = false)
    {
        yield return null;
        if (action != null) action();
        if (deleteAfterAction) Destroy(this.gameObject);
    }
}
