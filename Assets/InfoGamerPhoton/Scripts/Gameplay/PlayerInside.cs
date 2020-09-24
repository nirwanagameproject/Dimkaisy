using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerInside : MonoBehaviourPun
{
    // Start is called before the first frame update
    void Start()
    {
        if (GetComponent<PhotonView>().Owner != null)
        {
            transform.parent = GameObject.Find("Player (" + GetComponent<PhotonView>().Owner.NickName + ")").transform;
            name = "Player";
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
