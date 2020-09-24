using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviourPunCallbacks
{
    [SerializeField]
    public string BoxInStasiun;
    [SerializeField]
    public int Nourut;
    // Start is called before the first frame update
    void Start()
    {
        BoxInStasiun = "";
        Nourut = -1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public string getBoxInStasiun()
    {
        return BoxInStasiun;
    }

    public int getNoUrut()
    {
        return Nourut;
    }

    [PunRPC]
    void boxrpc(string nama, string namaParent)
    {
        name = nama;
        transform.parent = GameObject.Find(namaParent).transform;
    }

    [PunRPC]
    void myjumlahbox(int jumlahbox)
    {
        GameSetupController.jumlahBox = jumlahbox;
    }

    [PunRPC]
    void setStasiun(string nama,int nourut)
    {
        BoxInStasiun = nama;
        Nourut = nourut;
    }

    public void OnCollisionEnter(Collision collision)
    {
        if(PhotonNetwork.IsMasterClient)
        if (collision.collider.tag.Equals("Obstacle") && BoxInStasiun=="")
        {
            GameObject boxLain = collision.collider.gameObject;
            if (boxLain.GetComponent<Cube>().BoxInStasiun != "" && boxLain.GetComponent<Cube>().Nourut<5 && boxLain.GetComponent<Cube>().Nourut >= 0 && !GameObject.Find(boxLain.GetComponent<Cube>().BoxInStasiun).GetComponent<BoxCrafting>().BoxPosition[boxLain.GetComponent<Cube>().Nourut + 4].isFilled)
            {
                GameObject.Find(boxLain.GetComponent<Cube>().BoxInStasiun).GetComponent<PhotonView>().RPC("dapatBoxRPC", RpcTarget.All, boxLain.GetComponent<Cube>().Nourut + 4, name);
            }
        }
    }

}
