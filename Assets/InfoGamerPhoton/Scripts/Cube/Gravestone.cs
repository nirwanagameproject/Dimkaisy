using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gravestone : MonoBehaviourPunCallbacks
{
    Player1 player1;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(kadaluarsa());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator kadaluarsa()
    {
        yield return new WaitForSeconds(10f);
        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.Destroy(gameObject);
    }

    [PunRPC]
    void itemrpc(string nama, string namaParent, string namaPemilikHP)
    {
        transform.Find("NamaRIP").GetComponent<TextMesh>().text = namaPemilikHP;
        name = nama;
        transform.parent = GameObject.Find(namaParent).transform;
    }

    [PunRPC]
    void DestroyItem()
    {
        PhotonNetwork.Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        
        
    }

}
