using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTrafficSystem : MonoBehaviour
{
    private static MoveTrafficSystem _instance;
    public static MoveTrafficSystem Instance { get {return _instance; } }
    private Transform player;
    private bool initialized = false;
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }
    public void Initialize(Transform player){
        this.player = player;
        initialized = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!initialized) return;
        transform.position = new Vector3(player.position.x, player.position.y + 50, player.position.z);
    }
}
