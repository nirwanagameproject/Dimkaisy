using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Weapon : MonoBehaviourPunCallbacks
{
    Player1 player1;

    [SerializeField]
    public int damage;
    public int needammo;
    public bool used;
    // Start is called before the first frame update
    void Start()
    {
        used = false;
        StartCoroutine(kadaluarsa());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [PunRPC]
    void weaponrpc(string nama, string namaParent)
    {
        name = nama;
        transform.parent = GameObject.Find(namaParent).transform;
    }

    [PunRPC]
    void weaponrpcThrow(string nama, string namaParent)
    {
        name = nama;
        transform.parent = GameObject.Find(namaParent).transform;
        GetComponent<Rigidbody>().AddForce(GetComponent<Rigidbody>().transform.forward * 10, ForceMode.VelocityChange);
    }

    [PunRPC]
    void DestroyItem()
    {
        PhotonNetwork.Destroy(gameObject);
    }

    [PunRPC]
    void usedWeapon()
    {
        used = true;
    }

    public IEnumerator kadaluarsa()
    {
        yield return new WaitForSeconds(15);
        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        
        if (collision.collider.name.Contains(PhotonNetwork.NickName))
        {
            string[] splitArray = name.Split(char.Parse(" "));
            string myTakeWPN = splitArray[0];
            int myslot = 1;

            if(!used)
            for(int i=1;i<4;i++)
                if ((string)PhotonNetwork.LocalPlayer.CustomProperties["weaponslot"+i]=="hand")
                {
                    used = true;
                    GetComponent<PhotonView>().RPC("usedWeapon", RpcTarget.All);

                    string lastweapon = (string)PhotonNetwork.LocalPlayer.CustomProperties["weapon"];
                    if (lastweapon == "hand") lastweapon = "";
                    ExitGames.Client.Photon.Hashtable setRace = new ExitGames.Client.Photon.Hashtable();
                    setRace.Add("weaponslot" + i, myTakeWPN);
                    PhotonNetwork.LocalPlayer.SetCustomProperties(setRace, null);
                    if (myTakeWPN.Contains("Baseball"))
                    {
                        ExitGames.Client.Photon.Hashtable setWpnTipe = new ExitGames.Client.Photon.Hashtable();
                        setWpnTipe.Add("weaponslot" + i + "weapontipe", "melee");
                        PhotonNetwork.LocalPlayer.SetCustomProperties(setWpnTipe, null);

                        ExitGames.Client.Photon.Hashtable setDmg = new ExitGames.Client.Photon.Hashtable();
                        setDmg.Add("weaponslot" + i + "damage", damage);
                        setDmg.Add("weaponslot" + i+"needammo", 0);
                        PhotonNetwork.LocalPlayer.SetCustomProperties(setDmg, null);
                    }
                    else
                    {
                        ExitGames.Client.Photon.Hashtable setWpnTipe = new ExitGames.Client.Photon.Hashtable();
                        setWpnTipe.Add("weaponslot" + i + "weapontipe", "range");
                        PhotonNetwork.LocalPlayer.SetCustomProperties(setWpnTipe, null);

                        ExitGames.Client.Photon.Hashtable setDmg = new ExitGames.Client.Photon.Hashtable();
                        setDmg.Add("weaponslot" + i + "damage", damage);
                        setDmg.Add("weaponslot" + i + "needammo", needammo);
                        if(myTakeWPN.Contains("Flame")) setDmg.Add("weaponslot" + i + "aspd", 300f);
                        PhotonNetwork.LocalPlayer.SetCustomProperties(setDmg, null);
                    }
                

                    string weapon = (string)PhotonNetwork.LocalPlayer.CustomProperties["weapon"];

                    if (i == 1)
                    {
                        GameSetupController.instance.SlotWeapon1.transform.Find("GameObject").GetComponent<Image>().color = Color.white;
                        GameSetupController.instance.SlotWeapon1.transform.Find("GameObject").GetComponent<Image>().overrideSprite = Resources.Load<Sprite>("Models/Weapon/Image/" + myTakeWPN);
                    }
                    else if (i == 2)
                    {
                        GameSetupController.instance.SlotWeapon2.transform.Find("GameObject").GetComponent<Image>().color = Color.white;
                        GameSetupController.instance.SlotWeapon2.transform.Find("GameObject").GetComponent<Image>().overrideSprite = Resources.Load<Sprite>("Models/Weapon/Image/" + myTakeWPN);
                    }
                    else if (i == 3)
                    {
                        GameSetupController.instance.SlotWeapon3.transform.Find("GameObject").GetComponent<Image>().color = Color.white;
                        GameSetupController.instance.SlotWeapon3.transform.Find("GameObject").GetComponent<Image>().overrideSprite = Resources.Load<Sprite>("Models/Weapon/Image/" + myTakeWPN);
                    }
                    if ((int)PhotonNetwork.LocalPlayer.CustomProperties["weaponslotnumber"] == i)
                        GameSetupController.instance.setWeapon(i);

                    GetComponent<PhotonView>().RPC("DestroyItem", RpcTarget.MasterClient);
                    break;
                }

        }
        
    }

}
