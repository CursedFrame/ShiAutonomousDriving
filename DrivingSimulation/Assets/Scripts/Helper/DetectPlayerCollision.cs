using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectPlayerCollision : MonoBehaviour
{
    public System.Action enterAction, exitAction;
    public bool deleteGameObjectOnEnter = false, deleteGameObjectOnExit = false;
    void OnTriggerEnter(Collider collider){
        if (enterAction == null) return;
        if (collider.tag == "Player"){
            StartCoroutine(DoDelayed(() => {
                enterAction();
            }, deleteGameObjectOnEnter));
        }
    }

    void OnTriggerExit(Collider collider){
        if (exitAction == null) return;
        if (collider.tag == "Player") {
            StartCoroutine(DoDelayed(() => {
                exitAction();
            }, deleteGameObjectOnExit));
        }
    }

    IEnumerator DoDelayed(System.Action action, bool deleteAfterAction = false){
        yield return null;
        if (action != null) action();
        if (deleteAfterAction) Destroy(this.gameObject);
    }
}
