using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectPlayerCollision : MonoBehaviour
{
    public GameObject parentChunk;
    public ChunkManager chunkManager;
    void OnTriggerEnter(Collider collider){
        if (collider.tag == "Player"){
            StartCoroutine(DoDelayed(() => {
                Debug.Log("Player has collided! (enter)");
                chunkManager.terrain.enabled = true;
                for (int i = 0; i < parentChunk.transform.childCount; i++){
                    Transform child = parentChunk.transform.GetChild(i);
                    if (child.name == ChunkManager.childColliderName) continue;
                    child.gameObject.SetActive(true);
                }
            }));
        }
    }

    void OnTriggerExit(Collider collider){
        if (collider.tag == "Player") {
            StartCoroutine(DoDelayed(() => {
                Debug.Log("Player has collided! (exit)");
                chunkManager.terrain.enabled = false;
                for (int i = 0; i < parentChunk.transform.childCount; i++){
                    Transform child = parentChunk.transform.GetChild(i);
                    if (child.name == ChunkManager.childColliderName) continue;
                    child.gameObject.SetActive(false);
                }
            }));
        }
    }

    IEnumerator DoDelayed(System.Action action){
        yield return null;
        if (action != null) action();
    }
}
