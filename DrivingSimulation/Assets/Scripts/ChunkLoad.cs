using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkLoad : MonoBehaviour
{
    public GameObject player;
    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(this.transform.GetChild(0).position, player.transform.position) < GameMaster.Instance.chunkLoadDistance){
            this.transform.GetChild(0).gameObject.SetActive(true);
        } else {
            this.transform.GetChild(0).gameObject.SetActive(false);
        }
    }
}
