using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class outsidemap : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(PhotonNetwork.IsMasterClient)
        collision.collider.transform.position = new Vector3(17.5f,10,17.5f);
    }

}
