using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Vehicle : MonoBehaviourPunCallbacks
{
    [SerializeField]
    public List<string> TempatDuduk;
    [SerializeField]
    public int maxTempatDuduk;
    [SerializeField]
    public int myHP;
    [SerializeField]
    public int speed;
    [SerializeField]
    public int speedMuter;
    [SerializeField]
    public int belokSpeed;
    // Start is called before the first frame update
    public float myFireFloat;
    string team;
    bool destroyed;
    bool myvehicle;

    Vector3 PreviousFramePosition = Vector3.zero; // Or whatever your initial position is
    float realspeed = 0f;


    void Start()
    {
        TempatDuduk = new List<string>();
        myvehicle = true;
        destroyed = false;

        myFireFloat = 0.8f;
        ExitGames.Client.Photon.Hashtable setHP = new ExitGames.Client.Photon.Hashtable();
        setHP.Add(name + "health", myHP);
        setHP.Add(name + "maxhealth", myHP);
        PhotonNetwork.CurrentRoom.SetCustomProperties(setHP, null);

        transform.Find("CanvasVehicle").transform.Find("MyBarVehicle").transform.Find("Health Bar1").transform.Find("GreenBar").GetComponent<Image>().fillAmount = 1;
        transform.Find("CanvasVehicle").transform.Find("MyBarVehicle").transform.Find("Health Bar1").transform.Find("Text").GetComponent<Text>().text = myHP.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        double worldScreenHeight = Screen.height;
        double worldScreenWidth = Screen.width;

        Vector3 posisi = Camera.main.WorldToScreenPoint(new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z));
        if (!destroyed)
        {
            transform.Find("CanvasVehicle").transform.Find("MyBarVehicle").localPosition = new Vector3((float)(((posisi.x) - worldScreenWidth / 2)), (float)(((posisi.y) - worldScreenHeight / 2)), 0);
            transform.Find("CanvasVehicle").transform.Find("MyBarVehicle").localScale = new Vector3((float)(worldScreenWidth / 1024), (float)(worldScreenHeight / 768), 1);
        }

        
    }

    void FixedUpdate()
    {
        float movementPerFrame = Vector3.Distance(PreviousFramePosition, transform.position);
        realspeed = movementPerFrame / Time.deltaTime;
        PreviousFramePosition = transform.position;
    }

    [PunRPC]
    void vehicleOwned(string nama)
    {
        if(TempatDuduk.Count<maxTempatDuduk)
        TempatDuduk.Add(nama);
    }

    [PunRPC]
    void vehicleRemove(string nama)
    {
        if(TempatDuduk.Count>1)
        if (TempatDuduk[0] == nama && TempatDuduk[1] != null)
            {
                if (GameObject.Find(TempatDuduk[1]).GetComponent<PhotonView>().IsMine)
                {
                    GameObject.Find(TempatDuduk[1]).GetComponent<Controller>().ButtonThrowWeapon.transform.Find("GameObject").GetComponent<Image>().overrideSprite = Resources.Load<Sprite>("[UIFabrica]TrollNest_Free_v01/02.PNG/Honk");
                    GameObject.Find(TempatDuduk[1]).GetComponent<Controller>().ButtonThrowWeapon.SetActive(true);
                }
                GameObject.Find(TempatDuduk[1]).GetComponent<Player1>().Inputs.movingwithNebeng = false;
            }
        TempatDuduk.Remove(nama);
        
    }

    [PunRPC]
    void vehiclerpc(string nama, string namaParent)
    {
        name = nama;
        transform.parent = GameObject.Find(namaParent).transform;
        if (name.Contains("bikeMission"))
        {
            Material[] materials = transform.Find("ObjectModel").Find("Seat").GetComponent<MeshRenderer>().materials;
            materials[0].color = Color.red;
            transform.Find("ObjectModel").Find("Seat").GetComponent<MeshRenderer>().material = materials[0];
            materials = transform.Find("ObjectModel").Find("Box08").GetComponent<MeshRenderer>().materials;
            materials[0].color = Color.red;
            transform.Find("ObjectModel").Find("Seat").GetComponent<MeshRenderer>().material = materials[0];
        }
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
            transform.Find("CanvasVehicle").transform.Find("MyBarVehicle").transform.Find("Health Bar1").transform.Find("GreenBar").GetComponent<Image>().fillAmount = myhpfloat;
            transform.Find("CanvasVehicle").transform.Find("MyBarVehicle").transform.Find("Health Bar1").transform.Find("Text").GetComponent<Text>().text = myhp.ToString();
        }


        if (myhpfloat < myFireFloat)
        {
            myFireFloat -= 0.2f;
        }

        if (myhp == 0 && !destroyed)
        {
            destroyed = true;
            for(var i = 0; i < TempatDuduk.Count; i++)
            {
                if (GameObject.Find(TempatDuduk[i]).GetComponent<PhotonView>().IsMine)
                {
                    ExitGames.Client.Photon.Hashtable setRace = new ExitGames.Client.Photon.Hashtable();
                    setRace.Add("weapon", PhotonNetwork.LocalPlayer.CustomProperties["weaponTemp"]);
                    setRace.Add("weapontipe", PhotonNetwork.LocalPlayer.CustomProperties["weapontipeTemp"]);
                    setRace.Add("needammo", PhotonNetwork.LocalPlayer.CustomProperties["needammoTemp"]);
                    setRace.Add("aspd", PhotonNetwork.LocalPlayer.CustomProperties["aspdTemp"]);
                    setRace.Add("vehicle", "");
                    PhotonNetwork.LocalPlayer.SetCustomProperties(setRace, null);
                }
                
                GameObject.Find(TempatDuduk[i]).GetComponent<PhotonView>().RPC("OutVehicleRPC", RpcTarget.All, TempatDuduk[i], name);
                if (GameObject.Find(TempatDuduk[i]).GetComponent<Player1>().getMyNickName(TempatDuduk[i]) == PhotonNetwork.NickName)
                {
                    ExitGames.Client.Photon.Hashtable setRace = new ExitGames.Client.Photon.Hashtable();
                    setRace.Add("weapon", PhotonNetwork.LocalPlayer.CustomProperties["weaponTemp"]);
                    setRace.Add("weapontipe", PhotonNetwork.LocalPlayer.CustomProperties["weapontipeTemp"]);
                    setRace.Add("vehicle", "");
                    PhotonNetwork.LocalPlayer.SetCustomProperties(setRace, null);
                    GameObject.Find(TempatDuduk[i]).GetComponent<Player1>().Inputs.EnterVehicle = false;
                }
            }

            Destroy(GameObject.Find(name).transform.Find("CanvasVehicle").transform.Find("MyBarVehicle").gameObject);
            GetComponent<Rigidbody>().isKinematic = true;
            if (PhotonNetwork.IsMasterClient)
            {
                InvokeRepeating("Explode", 1f, 0.25f);
            }
            Invoke("GameOver", 3f);

        }
    }

    [PunRPC]
    void ExplodeRPC()
    {
        AudioSource underAttack = GameObject.Find("tankexplosion").GetComponent<AudioSource>();
        GameObject.Find("tankexplosion").transform.position = transform.position;
        if(!underAttack.isPlaying)
        underAttack.Play();

        Vector3 explosionPos = transform.position;
        Collider[] colliders = Physics.OverlapSphere(explosionPos, 5f);
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();

            if (rb != null && !rb.isKinematic)
            {
                rb.AddExplosionForce(rb.mass*300, explosionPos, 5f, 3.0F);
                if(hit.tag == "Player")
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
        GameObject go = PhotonNetwork.InstantiateSceneObject(Path.Combine("Models/Fire", "fx_fire_ball_b"), new Vector3(transform.position.x + Random.Range(-3f, 3f), transform.position.y + Random.Range(2, 4), transform.position.z + Random.Range(-3f, 3f)), transform.rotation);
        go.name = "FireExplode (" + go.GetPhotonView().ViewID + ")";
        go.GetComponent<PhotonView>().RPC("itemrpc", RpcTarget.All, go.name, name);
        GetComponent<PhotonView>().RPC("ExplodeRPC", RpcTarget.All);
    }

    public void GameOver()
    {
        if (PhotonNetwork.IsMasterClient)
            CancelInvoke("Explode");

        if (PlayerPrefs.GetInt("ActNumber") == 2)
        {
            int bikemisi = 1;
            if (name.Contains("bikeMission1")) bikemisi = 1;
            else if (name.Contains("bikeMission2")) bikemisi = 2;
            else if (name.Contains("bikeMission3")) bikemisi = 3;
            else if (name.Contains("bikeMission4")) bikemisi = 4;
            if (name.Contains("bikeMission" + bikemisi) && PhotonNetwork.IsMasterClient)
            {
                Vector3 mybike = GameObject.Find("SpawnMotor" + bikemisi).transform.position;
                GameObject go = PhotonNetwork.InstantiateSceneObject(Path.Combine("Models/Vehicle", "bike"), new Vector3(mybike.x, mybike.y + 0.5f, mybike.z), Quaternion.identity);
                go.name = "bikeMission" + bikemisi;
                go.GetComponent<PhotonView>().RPC("vehiclerpc", RpcTarget.All, go.name, "SpawnMotor" + bikemisi);
            }
        }
        
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (collision.collider.tag.Equals("Projectile"))
            {
                string owner = collision.collider.gameObject.GetComponent<Projectile>().owner;
                for (var i = 0; i < TempatDuduk.Count; i++)
                {
                    if (TempatDuduk[i] == owner)
                    {
                        myvehicle = false;
                        Physics.IgnoreCollision(collision.collider, GetComponent<BoxCollider>(), true);
                        break;
                    }
                }
            }
        }
        

        if (PhotonNetwork.IsMasterClient)
            if (collision.collider.tag.Equals("Projectile") && myvehicle && (collision.relativeVelocity.magnitude > 2 || collision.relativeVelocity.magnitude == 0 || collision.collider.name.Contains("Turret")))
            {
               

                ExitGames.Client.Photon.Hashtable setHP = new ExitGames.Client.Photon.Hashtable();
                int myhp = (int)PhotonNetwork.CurrentRoom.CustomProperties[name + "health"] - 
                    collision.collider.GetComponent<Projectile>().damage;
                if (myhp <= 0) myhp = 0;
                setHP.Add(name + "health", myhp);
                myHP = myhp;
                PhotonNetwork.CurrentRoom.SetCustomProperties(setHP, null);
                float myhpfloat = (float)myhp / (int)PhotonNetwork.CurrentRoom.CustomProperties[name + "maxhealth"];
                if (myhpfloat < myFireFloat)
                {
                    for (var i = 0; i < 2; i++)
                    {
                        GameObject go = PhotonNetwork.InstantiateSceneObject(Path.Combine("Models/Fire", "fx_fire_g"), new Vector3(transform.position.x + Random.Range(-0.5f, 0.5f), transform.position.y + Random.Range(1, 3), transform.position.z + Random.Range(-0.5f, 0.5f)), transform.rotation);
                        go.name = "Fire (" + go.GetPhotonView().ViewID + ")";
                        go.GetComponent<PhotonView>().RPC("itemrpc", RpcTarget.All, go.name, name);
                    }

                }

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
        myvehicle = true;

    }

    public override void OnPlayerLeftRoom(Player otherPlayer)//called whenever a player leave the room
    {
        for (var i = 0; i < TempatDuduk.Count; i++)
        {
            if (TempatDuduk[i] == "Player (" + otherPlayer.NickName + ")")
            {
                GetComponent<PhotonView>().RPC("vehicleRemove", RpcTarget.All, "Player ("+ otherPlayer.NickName + ")");
                Debug.Log("Remove "+ otherPlayer.NickName);
                break;
            }
        }
    }

}
