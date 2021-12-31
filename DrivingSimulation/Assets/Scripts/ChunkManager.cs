using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    public static string childColliderName = "CollisionDetection";
    public Terrain terrain;
    // Start is called before the first frame update
    void Start()
    {
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
        collisionDetection.chunkManager = this;
        collisionDetection.parentChunk = this.gameObject;

        // Adjust child collider bounds and properties
        TerrainCollider terrainCollider = gameObject.GetComponent<TerrainCollider>();
        Bounds terrainBounds = terrainCollider.bounds;
        loadChunkCollider.size = new Vector3(terrainCollider.bounds.size.x * 3, 250, terrainCollider.bounds.size.z * 3);
        loadChunkCollider.center = terrainCollider.bounds.center;
        loadChunkCollider.isTrigger = true;
    }
}
