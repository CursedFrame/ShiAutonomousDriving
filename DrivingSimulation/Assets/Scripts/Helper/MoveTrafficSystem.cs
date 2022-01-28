using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTrafficSystem : MonoBehaviour
{
    public const string TAG = "MoveTrafficSystem";
    private static MoveTrafficSystem _instance;
    private Transform player;
    private bool initialized = false;

    public static MoveTrafficSystem Instance { get { return _instance; } }

    public void Initialize(Transform player)
    {
        this.player = player;
        initialized = true;
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } 
        else 
        {
            _instance = this;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (!initialized) return;
        transform.position = new Vector3(player.position.x, player.position.y + 50, player.position.z);
    }
}
