using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviourPunCallbacks
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

    [PunRPC]
    void itemrpc(string nama, string namaParent)
    {
        name = nama;
        transform.parent = GameObject.Find("Ammo Heart Spawn").transform;
        GetComponent<Rigidbody>().useGravity = false;
        GetComponent<Rigidbody>().isKinematic = true;
    }

    [PunRPC]
    void DestroyItem()
    {
        PhotonNetwork.Destroy(gameObject);
    }

    public IEnumerator kadaluarsa()
    {
        yield return new WaitForSeconds(15);
        if(PhotonNetwork.IsMasterClient)
        PhotonNetwork.Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        
        if (name.Contains("Hearts") && collision.collider.name.Contains(PhotonNetwork.NickName))
        {
            ExitGames.Client.Photon.Hashtable setHP = new ExitGames.Client.Photon.Hashtable();
            int myhp = (int)PhotonNetwork.LocalPlayer.CustomProperties["health"] + 10;
            if (myhp >= (int)PhotonNetwork.LocalPlayer.CustomProperties["maxhealth"]) myhp = (int)PhotonNetwork.LocalPlayer.CustomProperties["maxhealth"];
            setHP.Add("health", myhp);
            PhotonNetwork.LocalPlayer.SetCustomProperties(setHP, null);
            collision.collider.GetComponent<PhotonView>().RPC("getHearts", RpcTarget.All, collision.collider.name, myhp, (int)PhotonNetwork.LocalPlayer.CustomProperties["maxhealth"]);

            float myhpfloat = (float)myhp / (int)PhotonNetwork.LocalPlayer.CustomProperties["maxhealth"];
            GameObject.Find("Canvas").transform.Find("HealthBar").transform.Find("GreenBar").GetComponent<Image>().fillAmount = myhpfloat;
            GameObject.Find("Canvas").transform.Find("HealthBar").transform.Find("Text").GetComponent<Text>().text = myhp.ToString();

            AudioSource audio = GameObject.Find("GetHeal").GetComponent<AudioSource>();
            GameObject.Find("GetHeal").transform.position = transform.position;
            audio.Play();

            GetComponent<PhotonView>().RPC("DestroyItem", RpcTarget.MasterClient);
            
        }
        else if(name.Contains("Bullets") && collision.collider.name.Contains(PhotonNetwork.NickName))
        {
            ExitGames.Client.Photon.Hashtable setHP = new ExitGames.Client.Photon.Hashtable();
            int myammo = (int)PhotonNetwork.LocalPlayer.CustomProperties["ammo"] + 25;
            if (myammo >= (int)PhotonNetwork.LocalPlayer.CustomProperties["maxammo"]) myammo = (int)PhotonNetwork.LocalPlayer.CustomProperties["maxammo"];
            setHP.Add("ammo", myammo);
            PhotonNetwork.LocalPlayer.SetCustomProperties(setHP, null);

            float myammofloat = (float)myammo / (int)PhotonNetwork.LocalPlayer.CustomProperties["maxammo"];
            GameObject.Find("Canvas").transform.Find("AmmoBar").transform.Find("GreenBar").GetComponent<Image>().fillAmount = myammofloat;
            GameObject.Find("Canvas").transform.Find("AmmoBar").transform.Find("TextAmmo").GetComponent<Text>().text = myammo.ToString();

            AudioSource audio = GameObject.Find("GetAmmo").GetComponent<AudioSource>();
            GameObject.Find("GetAmmo").transform.position = transform.position;
            audio.Play();

            GetComponent<PhotonView>().RPC("DestroyItem", RpcTarget.MasterClient);
        }
        else if (name.Contains("PowerHaste") && collision.collider.name.Contains(PhotonNetwork.NickName))
        {
            ExitGames.Client.Photon.Hashtable setHP = new ExitGames.Client.Photon.Hashtable();
            int speed = (int)PhotonNetwork.LocalPlayer.CustomProperties["speed"] + 4;
            setHP.Add("speed", speed);
            PhotonNetwork.LocalPlayer.SetCustomProperties(setHP, null);

            AudioSource audio = GameObject.Find("GetPowerHaste").GetComponent<AudioSource>();
            GameObject.Find("GetPowerHaste").transform.position = transform.position;
            audio.Play();

            GetComponent<PhotonView>().RPC("DestroyItem", RpcTarget.MasterClient);
            GameObject.Find(collision.collider.name).GetComponent<PhotonView>().RPC("getRune", RpcTarget.All, "PowerHaste");
        }

    }

}
