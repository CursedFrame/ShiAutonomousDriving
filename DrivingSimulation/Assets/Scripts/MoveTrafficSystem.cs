using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTrafficSystem : MonoBehaviour
{
    private static MoveTrafficSystem _instance;
    public static MoveTrafficSystem Instance { get {return _instance; } }
    public Transform player;
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (player.position != null) transform.position = new Vector3(player.position.x, player.position.y + 50, player.position.z);
    }
}
