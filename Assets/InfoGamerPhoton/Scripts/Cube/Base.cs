using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Base : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    int myHP;
    float myFireFloat;
    string team;
    bool destroyed;
    [SerializeField]
    public int invulnerable;
    void Start()
    {
        string[] teamku = name.Split('_');
        team = teamku[1];
        destroyed = false;
        if (PlayerPrefs.GetInt("ActNumber") == 1 || PlayerPrefs.GetInt("ActNumber") == 2) invulnerable = 0;
        else invulnerable = 3;

        if (PlayerPrefs.GetInt("ActNumber") == 2) myHP = 500;
        else myHP = 100;
        myFireFloat = 0.8f;
        ExitGames.Client.Photon.Hashtable setHP = new ExitGames.Client.Photon.Hashtable();
        setHP.Add(name + "health", myHP);
        setHP.Add(name + "maxhealth", myHP);
        PhotonNetwork.CurrentRoom.SetCustomProperties(setHP, null);

        transform.Find("CanvasBase").transform.Find("MyBarBase").transform.Find("Health Bar1").transform.Find("GreenBar").GetComponent<Image>().fillAmount = 1;
        transform.Find("CanvasBase").transform.Find("MyBarBase").transform.Find("Health Bar1").transform.Find("Text").GetComponent<Text>().text = myHP.ToString();

    }


    // Update is called once per frame
    void Update()
    {
        double worldScreenHeight = Screen.height;
        double worldScreenWidth = Screen.width;

        Vector3 posisi = Camera.main.WorldToScreenPoint(new Vector3(transform.position.x, transform.position.y + 2.2f, transform.position.z));
        if (!destroyed)
        {
            transform.Find("CanvasBase").transform.Find("MyBarBase").localPosition = new Vector3((float)(((posisi.x) - worldScreenWidth / 2)), (float)(((posisi.y) - worldScreenHeight / 2)), 0);
            transform.Find("CanvasBase").transform.Find("MyBarBase").localScale = new Vector3((float)(worldScreenWidth / 1024), (float)(worldScreenHeight / 768), 1);
        }
    }

    [PunRPC]
    void getDamaged(int myhp, float myFireFloat1)
    {
        myFireFloat = myFireFloat1;
        float myhpfloat = (float)myhp / (int)PhotonNetwork.CurrentRoom.CustomProperties[name + "maxhealth"];
        if (!destroyed)
        {
            transform.Find("CanvasBase").transform.Find("MyBarBase").transform.Find("Health Bar1").transform.Find("GreenBar").GetComponent<Image>().fillAmount = myhpfloat;
            transform.Find("CanvasBase").transform.Find("MyBarBase").transform.Find("Health Bar1").transform.Find("Text").GetComponent<Text>().text = myhp.ToString();
        }

        if (myhpfloat < myFireFloat && team==(string)PhotonNetwork.LocalPlayer.CustomProperties["team"])
        {
            AudioSource underAttack = GameObject.Find("UnderAttack").GetComponent<AudioSource>();
            GameObject.Find("UnderAttack").transform.position = transform.position;
            underAttack.Play();
        }

        if (myhpfloat < myFireFloat)
        {
            myFireFloat -= 0.2f;
        }

        if (myhp == 0 && !destroyed)
        {
            destroyed = true;
            AudioSource underAttack = GameObject.Find("Explosion").GetComponent<AudioSource>();
            GameObject.Find("Explosion").transform.position = transform.position;
            underAttack.Play();
            Destroy(GameObject.Find(name).transform.Find("CanvasBase").transform.Find("MyBarBase").gameObject);
            GetComponent<Rigidbody>().isKinematic = true;
            if (PhotonNetwork.IsMasterClient)
            {
                InvokeRepeating("Explode",1f,0.25f);
            }
            Invoke("GameOver", 10f);
            
        }
    }

    [PunRPC]
    void ExplodeRPC()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y - 0.125f, transform.position.z);

        Vector3 explosionPos = transform.position;
        Collider[] colliders = Physics.OverlapSphere(explosionPos, 5f);
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();

            if (rb != null && !rb.isKinematic)
            {

                rb.AddExplosionForce(rb.mass * 300, explosionPos, 5f, 3.0F);
                if (hit.tag == "Player")
                {
                    string myNickName = hit.GetComponent<Player1>().getMyNickName(hit.name);
                    foreach (Player player in PhotonNetwork.PlayerList) //loop through each player and create a player listing
                    {
                        if (player.NickName == myNickName)
                        {
                            int randomsound = Random.Range(1, 3);
                            AudioSource audio = GameObject.Find("Punch" + randomsound).GetComponent<AudioSource>();
                            GameObject.Find("Punch" + randomsound).transform.position = hit.transform.position;
                            audio.Play();

                            int damage = 2;
                            int myhp = (int)player.CustomProperties["health"];
                            int mymaxhp = (int)player.CustomProperties["maxhealth"];
                            if (myhp - damage >= 0) myhp -= damage;
                            else myhp = 0;

                            ExitGames.Client.Photon.Hashtable setHP = new ExitGames.Client.Photon.Hashtable();
                            setHP.Add("health", myhp);
                            player.SetCustomProperties(setHP, null);
                            if (!(bool)player.CustomProperties["dead"])
                                hit.GetComponent<PhotonView>().RPC("attackingPlayer", RpcTarget.All, hit.name, myhp, mymaxhp, player);
                            if (myhp == 0)
                            {
                                ExitGames.Client.Photon.Hashtable setDead = new ExitGames.Client.Photon.Hashtable();
                                setDead.Add("dead", true);
                                player.SetCustomProperties(setDead, null);
                            }
                            break;
                        }
                    }
                }

            }

        }
    }

        public void Explode()
    {
        GameObject go = PhotonNetwork.InstantiateSceneObject(Path.Combine("Models/Fire", "fx_fire_ball_b"), new Vector3(transform.position.x + Random.Range(-2, 2), transform.position.y + Random.Range(1, 6), transform.position.z + Random.Range(-2, 2)), transform.rotation);
        go.name = "FireExplode (" + go.GetPhotonView().ViewID + ")";
        go.GetComponent<PhotonView>().RPC("itemrpc", RpcTarget.All, go.name, "Building");
        GetComponent<PhotonView>().RPC("ExplodeRPC", RpcTarget.All);
    }

    public void GameOver()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            CancelInvoke("Explode");
            GameSetupController.instance.GetComponent<PhotonView>().RPC("baseHancur", RpcTarget.All, team);
        }

        Destroy(gameObject);
    }

    GameObject FindInActiveObjectByName(string name)
    {
        Transform[] objs = Resources.FindObjectsOfTypeAll<Transform>() as Transform[];
        for (int i = 0; i < objs.Length; i++)
        {
            if (objs[i].hideFlags == HideFlags.None)
            {
                if (objs[i].name == name)
                {
                    return objs[i].gameObject;
                }
            }
        }
        return null;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(PhotonNetwork.IsMasterClient)
            if (invulnerable==0 && collision.collider.tag.Equals("Projectile") && (collision.relativeVelocity.magnitude > 2 || collision.relativeVelocity.magnitude == 0 || collision.collider.name.Contains("Turret")))
            {
                ExitGames.Client.Photon.Hashtable setHP = new ExitGames.Client.Photon.Hashtable();
                int myhp = (int)PhotonNetwork.CurrentRoom.CustomProperties[name+"health"] - collision.collider.GetComponent<Projectile>().damage;
                if (myhp <= 0) myhp = 0;
                setHP.Add(name + "health", myhp);
                myHP = myhp;
                PhotonNetwork.CurrentRoom.SetCustomProperties(setHP, null);
                float myhpfloat = (float)myhp / (int)PhotonNetwork.CurrentRoom.CustomProperties[name + "maxhealth"];
                if (myhpfloat < myFireFloat)
                {
                    for(var i = 0; i < 3; i++)
                    {
                        GameObject go = PhotonNetwork.InstantiateSceneObject(Path.Combine("Models/Fire", "fx_fire_g"), new Vector3(transform.position.x + Random.Range(-1,2), transform.position.y +Random.Range(1, 4), transform.position.z + Random.Range(-1, 2)), transform.rotation);
                        go.name = "Fire (" + go.GetPhotonView().ViewID + ")";
                        go.GetComponent<PhotonView>().RPC("itemrpc", RpcTarget.All, go.name, name);
                    }
                    
                }

                GetComponent<PhotonView>().RPC("getDamaged", RpcTarget.All, myhp, myFireFloat);
                collision.collider.GetComponent<PhotonView>().RPC("DestroyItem", RpcTarget.MasterClient);
            }
        
    }

    

}
