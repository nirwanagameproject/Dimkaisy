using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Bangunan : MonoBehaviourPunCallbacks
{
    [SerializeField]
    public int myHP;
    // Start is called before the first frame update
    float myFireFloat;
    string team;
    bool destroyed;
    List<List<Color>> myChilds;
    List<Color> myObjectsColor;

    Vector3 PreviousFramePosition = Vector3.zero; // Or whatever your initial position is
    float realspeed = 0f;


    void Start()
    {
        myChilds = new List<List<Color>>();
        destroyed = false;

        myFireFloat = 0.8f;
        ExitGames.Client.Photon.Hashtable setHP = new ExitGames.Client.Photon.Hashtable();
        setHP.Add(name + "health", myHP);
        setHP.Add(name + "maxhealth", myHP);
        PhotonNetwork.CurrentRoom.SetCustomProperties(setHP, null);

        //transform.Find("CanvasVehicle").transform.Find("MyBarVehicle").transform.Find("Health Bar1").transform.Find("GreenBar").GetComponent<Image>().fillAmount = 1;
        //transform.Find("CanvasVehicle").transform.Find("MyBarVehicle").transform.Find("Health Bar1").transform.Find("Text").GetComponent<Text>().text = myHP.ToString();

        for(var i=0;i< transform.Find("ObjectModel").childCount; i++)
        {
            myObjectsColor = new List<Color>();
            for (var j = 0; j < transform.Find("ObjectModel").GetComponentInChildren<MeshRenderer>().materials.Length; j++)
            {
                myObjectsColor.Add(transform.Find("ObjectModel").GetComponentsInChildren<MeshRenderer>()[i].materials[j].color);
            }
            myChilds.Add(myObjectsColor);
            
        }

    }

    // Update is called once per frame
    void Update()
    {


        /*
        double worldScreenHeight = Screen.height;
        double worldScreenWidth = Screen.width;
        Vector3 posisi = Camera.main.WorldToScreenPoint(new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z));
        if (!destroyed)
        {
            transform.Find("CanvasVehicle").transform.Find("MyBarVehicle").localPosition = new Vector3((float)(((posisi.x) - worldScreenWidth / 2)), (float)(((posisi.y) - worldScreenHeight / 2)), 0);
            transform.Find("CanvasVehicle").transform.Find("MyBarVehicle").localScale = new Vector3((float)(worldScreenWidth / 1024), (float)(worldScreenHeight / 768), 1);
        }*/


    }

    void FixedUpdate()
    {
        float movementPerFrame = Vector3.Distance(PreviousFramePosition, transform.position);
        realspeed = movementPerFrame / Time.deltaTime;
        PreviousFramePosition = transform.position;
    }

    [PunRPC]
    void setkinematicRPC()
    {
        GetComponent<Rigidbody>().isKinematic = true;
    }

    [PunRPC]
    void pohonrpc(string nama, string namaParent)
    {
        name = nama;
        transform.parent = GameObject.Find(namaParent).transform;
    }

    [PunRPC]
    void DestroyItem()
    {
        PhotonNetwork.Destroy(gameObject);
    }

    [PunRPC]
    void getDamaged(int myhp, float myFireFloat1)
    {
        myFireFloat = myFireFloat1;
        float myhpfloat = (float)myhp / (int)PhotonNetwork.CurrentRoom.CustomProperties[name + "maxhealth"];
        if (!destroyed)
        {
            //transform.Find("CanvasVehicle").transform.Find("MyBarVehicle").transform.Find("Health Bar1").transform.Find("GreenBar").GetComponent<Image>().fillAmount = myhpfloat;
            //transform.Find("CanvasVehicle").transform.Find("MyBarVehicle").transform.Find("Health Bar1").transform.Find("Text").GetComponent<Text>().text = myhp.ToString();
        }

        AudioSource underAttack = GameObject.Find("Punch2").GetComponent<AudioSource>();
        GameObject.Find("Punch2").transform.position = transform.position;
        underAttack.Play();

        for (var i = 0; i < transform.Find("ObjectModel").childCount; i++)
        {
            for (var j = 0; j < transform.Find("ObjectModel").GetComponentInChildren<MeshRenderer>().materials.Length; j++)
            {
                transform.Find("ObjectModel").GetComponentsInChildren<MeshRenderer>()[i].materials[j].color = new Color(1, 0, 0);
            }

        }

        Invoke("GantiWarnaAsli", 1f);

        if (myhp == 0 && !destroyed)
        {
            destroyed = true;

            //Destroy(GameObject.Find(name).transform.Find("CanvasVehicle").transform.Find("MyBarVehicle").gameObject);
            //GetComponent<Rigidbody>().isKinematic = true;
            if (PhotonNetwork.IsMasterClient)
            {
                InvokeRepeating("Tumbang", 1f, 0.1f);
            }
            Invoke("Destroying", 3f);

        }
    }

    public void GantiWarnaAsli()
    {
        for (var i = 0; i < transform.Find("ObjectModel").childCount; i++)
        {
            for (var j = 0; j < transform.Find("ObjectModel").GetComponentInChildren<MeshRenderer>().materials.Length; j++)
            {
                transform.Find("ObjectModel").GetComponentsInChildren<MeshRenderer>()[i].materials[j].color = myChilds[i][j];
            }

        }
    }

    [PunRPC]
    void ExplodeRPC()
    {
        //transform.position = new Vector3(transform.position.x, transform.position.y - 0.125f, transform.position.z);

        AudioSource underAttack = GameObject.Find("tankexplosion").GetComponent<AudioSource>();
        underAttack.transform.position = transform.position;
        if(!underAttack.isPlaying)
        underAttack.Play();

    }

    public void Tumbang()
    {
        if (name.Contains("Bench") || name.Contains("Lamp"))
        {
            GameObject go = PhotonNetwork.InstantiateSceneObject(Path.Combine("Models/Fire", "fx_fire_ball_b"), new Vector3(transform.position.x + Random.Range(-1, 1), transform.position.y + Random.Range(1, 2), transform.position.z + Random.Range(-1, 1)), transform.rotation);
            go.name = "FireExplode (" + go.GetPhotonView().ViewID + ")";
            go.GetComponent<PhotonView>().RPC("itemrpc", RpcTarget.All, go.name, "Building");
        }
        else if (name.Contains("Cabin") || name.Contains("HouseRusak"))
        {
            GameObject go = PhotonNetwork.InstantiateSceneObject(Path.Combine("Models/Fire", "fx_fire_ball_b"), new Vector3(transform.position.x + Random.Range(-2, 2), transform.position.y + Random.Range(1, 6), transform.position.z + Random.Range(-2, 2)), transform.rotation);
            go.name = "FireExplode (" + go.GetPhotonView().ViewID + ")";
            go.GetComponent<PhotonView>().RPC("itemrpc", RpcTarget.All, go.name, "Building");
        }
        
        GetComponent<PhotonView>().RPC("ExplodeRPC", RpcTarget.All);
    }

    public void Destroying()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            CancelInvoke("Tumbang");

            int randomitem = Random.Range(1, 3);
            for (var i = 0; i < randomitem; i++)
            {
                float randompos = Random.Range(-0.5f, 0.5f);
                GameObject go = PhotonNetwork.InstantiateSceneObject(Path.Combine("Models", "Hearts"), new Vector3(transform.position.x + randompos, transform.position.y, transform.position.z + randompos), Quaternion.identity);
                go.name = "ItemHearts (" + go.GetPhotonView().ViewID + ")";
                go.GetComponent<PhotonView>().RPC("itemrpc", RpcTarget.All, go.name, "Ammo Heart Spawn");
            }
            randomitem = Random.Range(1, 3);
            for (var i = 0; i < randomitem; i++)
            {
                float randompos = Random.Range(-0.5f, 0.5f);
                GameObject go = PhotonNetwork.InstantiateSceneObject(Path.Combine("Models", "Bullets"), new Vector3(transform.position.x + randompos, transform.position.y, transform.position.z + randompos), Quaternion.identity);
                go.name = "ItemBullets (" + go.GetPhotonView().ViewID + ")";
                go.GetComponent<PhotonView>().RPC("itemrpc", RpcTarget.All, go.name, "Ammo Heart Spawn");
            }
            randomitem = Random.Range(1, 3);
            if (randomitem==1)
            {
                float randompos = Random.Range(-0.5f, 0.5f);
                GameObject go = PhotonNetwork.InstantiateSceneObject(Path.Combine("Models", "PowerHaste"), new Vector3(transform.position.x + randompos, transform.position.y, transform.position.z + randompos), Quaternion.identity);
                go.name = "PowerHaste (" + go.GetPhotonView().ViewID + ")";
                go.GetComponent<PhotonView>().RPC("itemrpc", RpcTarget.All, go.name, "Ammo Heart Spawn");
            }

            float randomposkuX = Random.Range(-20f, 37f);
            float randomposkuZ = Random.Range(-10f, 45f);
            randomitem = Random.Range(1, 5);
            GameObject treego = PhotonNetwork.InstantiateSceneObject(Path.Combine("Models/Tree", "TreeLow"+ randomitem), new Vector3(randomposkuX, transform.position.y+10,randomposkuZ), Quaternion.identity);
            treego.name = "TreeLow"+randomitem+" (" + treego.GetPhotonView().ViewID + ")";
            treego.GetComponent<PhotonView>().RPC("pohonrpc", RpcTarget.All, treego.name, "Obstacles");
        }
            
        if(name.Contains("(") && PhotonNetwork.IsMasterClient) PhotonNetwork.Destroy(gameObject);
        else Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (PhotonNetwork.IsMasterClient)
            if (collision.collider.tag.Equals("Projectile") && (collision.relativeVelocity.magnitude > 2 || collision.relativeVelocity.magnitude == 0 || collision.collider.name.Contains("Turret")))
            {
                ExitGames.Client.Photon.Hashtable setHP = new ExitGames.Client.Photon.Hashtable();
                int myhp = (int)PhotonNetwork.CurrentRoom.CustomProperties[name + "health"] - 
                    collision.collider.GetComponent<Projectile>().damage;
                if (myhp <= 0) myhp = 0;
                setHP.Add(name + "health", myhp);
                myHP = myhp;
                PhotonNetwork.CurrentRoom.SetCustomProperties(setHP, null);

                GetComponent<PhotonView>().RPC("getDamaged", RpcTarget.All, myhp, myFireFloat);
                collision.collider.GetComponent<PhotonView>().RPC("DestroyItem", RpcTarget.MasterClient);
            }

        if (collision.collider.tag.Equals("Player") && realspeed > 4)
            {
                if (collision.collider.GetComponent<PhotonView>().IsMine)
                {
                    ExitGames.Client.Photon.Hashtable setHP = new ExitGames.Client.Photon.Hashtable();
                    int myhp = (int)PhotonNetwork.LocalPlayer.CustomProperties["health"] - 10;
                    int mymaxhp = (int)PhotonNetwork.LocalPlayer.CustomProperties["maxhealth"];
                    if (myhp <= 0)
                    {
                        myhp = 0;
                    }
                    setHP.Add("health", myhp);
                    PhotonNetwork.LocalPlayer.SetCustomProperties(setHP, null);
                    //collision.collider.GetComponent<PhotonView>().RPC("getHearts", RpcTarget.All, collision.collider.name, myhp, (int)PhotonNetwork.LocalPlayer.CustomProperties["maxhealth"]);

                    foreach (Player player in PhotonNetwork.PlayerList) //loop through each player and create a player listing
                    {
                        if (player.NickName == PhotonNetwork.NickName)
                        {
                            if (!(bool)player.CustomProperties["dead"])
                                collision.collider.GetComponent<PhotonView>().RPC("attackingPlayer", RpcTarget.All, collision.collider.name, myhp, mymaxhp, player);
                            if (myhp == 0)
                            {
                                ExitGames.Client.Photon.Hashtable setDead = new ExitGames.Client.Photon.Hashtable();
                                setDead.Add("dead", true);
                                player.SetCustomProperties(setDead, null);
                            }
                            break;
                        }
                    }


                    float myhpfloat = (float)myhp / (int)PhotonNetwork.LocalPlayer.CustomProperties["maxhealth"];
                    GameObject.Find("Canvas").transform.Find("HealthBar").transform.Find("GreenBar").GetComponent<Image>().fillAmount = myhpfloat;
                    GameObject.Find("Canvas").transform.Find("HealthBar").transform.Find("Text").GetComponent<Text>().text = myhp.ToString();

                    AudioSource audio = GameObject.Find("Punch2").GetComponent<AudioSource>();
                    GameObject.Find("Punch2").transform.position = collision.collider.transform.position;
                    audio.Play();
                }
            }

    }


}
