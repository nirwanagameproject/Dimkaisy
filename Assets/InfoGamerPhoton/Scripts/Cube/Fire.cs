using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fire : MonoBehaviourPunCallbacks
{
    Player1 player1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator kadaluarsa()
    {
        yield return new WaitForSeconds(1f);
        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.Destroy(gameObject);
    }

    [PunRPC]
    void itemrpc(string nama, string namaParent)
    {
        name = nama;
        transform.parent = GameObject.Find(namaParent).transform;
        if (name.Contains("FireExplode"))
            StartCoroutine(kadaluarsa());
    }

    [PunRPC]
    void DestroyItem()
    {
        PhotonNetwork.Destroy(gameObject);
    }

}
