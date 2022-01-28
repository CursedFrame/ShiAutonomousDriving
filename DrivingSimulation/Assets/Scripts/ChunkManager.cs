using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    public static string childColliderName = "CollisionDetection";
    public Terrain terrain;
    public static float boundsMulti = 2.5f;
    // Start is called before the first frame update
    void Start()
    {
        if (gameObject.tag == "NonChunkDisable") return;
        // Disable rendering of terrain and objects
        terrain = gameObject.GetComponent<Terrain>();
        terrain.enabled = false;
        for (int i = 0; i < gameObject.transform.childCount; i++){
            Transform child = gameObject.transform.GetChild(i);
            if (child.name == ChunkManager.childColliderName) continue;
            child.gameObject.SetActive(false);
        }
        
        // Create child collider to detect if player is in range to load/unload chunk
        GameObject childCollider = new GameObject(childColliderName);
        childCollider.transform.parent = gameObject.transform;
        BoxCollider loadChunkCollider = childCollider.AddComponent<BoxCollider>();
        DetectPlayerCollision collisionDetection = childCollider.AddComponent<DetectPlayerCollision>();
        collisionDetection.enterAction = OnPlayerCollisionEnter;
        collisionDetection.exitAction = OnPlayerCollisionExit;

        // Adjust child collider bounds and properties
        TerrainCollider terrainCollider = gameObject.GetComponent<TerrainCollider>();
        Bounds terrainBounds = terrainCollider.bounds;
        loadChunkCollider.size = new Vector3(terrainCollider.bounds.size.x * boundsMulti, 250, terrainCollider.bounds.size.z * boundsMulti);
        loadChunkCollider.center = terrainCollider.bounds.center;
        loadChunkCollider.isTrigger = true;
    }

    private void OnPlayerCollisionEnter(){
        terrain.enabled = true;
        for (int i = 0; i < transform.childCount; i++){
            Transform child = transform.GetChild(i);
            if (child.name == ChunkManager.childColliderName || child.tag == "NonChunkEnable") continue;
            child.gameObject.SetActive(true);
        }
    }

    private void OnPlayerCollisionExit(){
        terrain.enabled = false;
        for (int i = 0; i < transform.childCount; i++){
            Transform child = transform.GetChild(i);
            if (child.name == ChunkManager.childColliderName || child.tag == "NonChunkEnable") continue;
            child.gameObject.SetActive(false);
        }
    }
}
