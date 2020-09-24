using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Robot : MonoBehaviourPunCallbacks
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
    [SerializeField]
    public string owner;
    [SerializeField]
    public string team;
    [SerializeField]
    public Collider[] targetKill;
    // Start is called before the first frame update
    public float myFireFloat;

    //public bool bikindiMap;

    bool destroyed;
    bool myvehicle;
    bool moving;
    bool attacking;
    bool attackingDPS;

    Vector3 targetPosRand;
    Vector3 targetRotRand;

    Vector3 PreviousFramePosition = Vector3.zero; // Or whatever your initial position is
    float realspeed = 0f;


    void Start()
    {
        TempatDuduk = new List<string>();
        myvehicle = true;
        destroyed = false;
        moving = false;
        attacking = false;
        attackingDPS = false;

        myFireFloat = 0.8f;
        ExitGames.Client.Photon.Hashtable setHP = new ExitGames.Client.Photon.Hashtable();
        setHP.Add(name + "health", myHP);
        setHP.Add(name + "maxhealth", myHP);
        PhotonNetwork.CurrentRoom.SetCustomProperties(setHP, null);

        transform.Find("CanvasVehicle").transform.Find("MyBarVehicle").transform.Find("Health Bar1").transform.Find("GreenBar").GetComponent<Image>().fillAmount = 1;
        transform.Find("CanvasVehicle").transform.Find("MyBarVehicle").transform.Find("Health Bar1").transform.Find("Text").GetComponent<Text>().text = myHP.ToString();

        if(PhotonNetwork.IsMasterClient && !name.Contains("MachinegunTurret"))
        InvokeRepeating("randommovementAI", (float)Random.Range(3, 5), (float)Random.Range(3,5));
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

        if (Vector3.Distance(transform.position, targetPosRand) > 0.1f && moving)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosRand, speed * Time.deltaTime);

            Quaternion lookRot = Quaternion.Euler(targetRotRand);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, belokSpeed * Time.deltaTime);
        }
        else
        {
            if(PhotonNetwork.IsMasterClient && moving)
                GetComponent<PhotonView>().RPC("setMove", RpcTarget.All, false);
        }

        if (PhotonNetwork.IsMasterClient)
        {
            targetKill = Physics.OverlapSphere(transform.position, 10, LayerMask.GetMask("Player"));            
            if (targetKill.Length > 0 && !attacking)
            {
                GameObject targetku = targetKill[Random.Range(0, targetKill.Length)].gameObject;
                string targetNickName = targetku.name;
                foreach (Player player in PhotonNetwork.PlayerList) //loop through each player and create a player listing
                {
                    if ("Player (" + player.NickName + ")" == targetNickName)
                    {
                        if ((string)player.CustomProperties["team"] != team)
                        {
                            GetComponent<PhotonView>().RPC("ngelockTarget", RpcTarget.All, targetNickName);
                        }
                    }
                }
            }
            else attacking = false;
            targetKill = Physics.OverlapSphere(transform.position, 10, LayerMask.GetMask("Vehicle"));
            if (targetKill.Length > 0 && !attacking)
            {
                GameObject targetku = targetKill[Random.Range(0, targetKill.Length)].gameObject;
                string targetNickName = targetku.name;
                foreach (Player player in PhotonNetwork.PlayerList) //loop through each player and create a player listing
                {
                    if (targetku.tag.Equals("Vehicle"))
                    {
                        for (int i = 0; i < targetku.GetComponent<Vehicle>().TempatDuduk.Count; i++)
                        {
                            if (targetku.GetComponent<Vehicle>().TempatDuduk[i] == "Player (" + player.NickName + ")")
                            {
                                if ((string)player.CustomProperties["team"] != team)
                                {
                                    GetComponent<PhotonView>().RPC("ngelockTarget", RpcTarget.All, "Player (" + player.NickName + ")");
                                }
                            }
                        }
                    }
                }
            }
            else attacking = false;
            targetKill = Physics.OverlapSphere(transform.position, 10, LayerMask.GetMask("Base"));
            if (targetKill.Length > 0 && !attacking)
            {
                GameObject targetku = targetKill[Random.Range(0, targetKill.Length)].gameObject;
                if (targetku.tag.Equals("building"))
                {
                    GetComponent<PhotonView>().RPC("ngelockTarget", RpcTarget.All, targetku.name);
                }
            }
            else attacking = false;
        }
    }

    [PunRPC]
    void ngelockTarget(string target)
    {
        if(GameObject.Find(target)!=null)
        if(name.Contains("Turret")) transform.Find("TopTembakan").LookAt(new Vector3(GameObject.Find(target).transform.position.x, GameObject.Find(target).transform.position.y, GameObject.Find(target).transform.position.z));
        else
        transform.LookAt(new Vector3(GameObject.Find(target).transform.position.x,transform.position.y+Vector3.forward.y, GameObject.Find(target).transform.position.z));
        attacking = true;
        if (PhotonNetwork.IsMasterClient)
        {
            if (!attackingDPS)
                StartCoroutine(nyerangTarget(target));
            attackingDPS = true;
        }
    }

    public System.Collections.IEnumerator nyerangTarget(string target)
    {
        yield return new WaitForSeconds(0.5f);
        float angleku = Mathf.Round(transform.eulerAngles.y * 1000f) / 1000f;
        float tambahX = Mathf.Round(Mathf.Sin(angleku * Mathf.Deg2Rad) * 100000) / 100000;
        float tambahZ = Mathf.Round(Mathf.Cos(angleku * Mathf.Deg2Rad) * 100000) / 100000;
        string namaBullet = "GunU";
        if (name.Contains("Flame")) namaBullet = "GunFlame";
        else if (name.Contains("Gun")) namaBullet = "Gun";
        Vector3 PosNembak = new Vector3(transform.position.x + tambahX, transform.position.y + 0.5f, transform.position.z + tambahZ);
        Quaternion RotNembak = transform.rotation;
        if (name.Contains("Turret"))
        {
            namaBullet = "GunTurret";
            angleku = Mathf.Round(transform.Find("TopTembakan").eulerAngles.y * 1000f) / 1000f;
            tambahX = Mathf.Round(Mathf.Sin(angleku * Mathf.Deg2Rad) * 250000) / 100000;
            tambahZ = Mathf.Round(Mathf.Cos(angleku * Mathf.Deg2Rad) * 250000) / 100000;
            PosNembak = new Vector3(transform.Find("TopTembakan").position.x + tambahX, transform.Find("TopTembakan").position.y + 0.5f, transform.Find("TopTembakan").position.z + tambahZ);
            RotNembak = transform.Find("TopTembakan").rotation;
        }
        GameObject go = PhotonNetwork.InstantiateSceneObject(Path.Combine("Models", "bullet_"+namaBullet), PosNembak, RotNembak);
        go.name = "Bullet"+ namaBullet + " (" + go.GetPhotonView().ViewID + ")";
        go.GetComponent<PhotonView>().RPC("itemrpc", RpcTarget.All, go.name, "Projectile Spawn", transform.position, "");
        if (name.Contains("Turret"))
            go.GetComponent<PhotonView>().RPC("moveToTarget", RpcTarget.All, target);
        attackingDPS = false;
    }

    [PunRPC]
    void robotrpc(string nama, string namaParent, string teamku)
    {
        name = nama;
        transform.parent = GameObject.Find(namaParent).transform;
        team = teamku;
    }

    [PunRPC]
    void DestroyItem()
    {
        PhotonNetwork.Destroy(gameObject);
    }

    [PunRPC]
    void randomMoveRPC(Vector3 targetpos, Vector3 targetrot)
    {
        transform.eulerAngles = new Vector3(0f,transform.eulerAngles.y,0f);
        targetPosRand = targetpos;
        targetRotRand = targetrot;
        moving = true;

    }

    [PunRPC]
    void setMove(bool setmove)
    {
        moving = setmove;
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

        if (myhpfloat != 0 && myhpfloat < myFireFloat && team == (string)PhotonNetwork.LocalPlayer.CustomProperties["team"] && name.Contains("Turret"))
        {
            AudioSource underAttack = GameObject.Find("UnderAttackTurret").GetComponent<AudioSource>();
            GameObject.Find("UnderAttackTurret").transform.position = transform.position;
            underAttack.Play();
        }

        if (myhpfloat < myFireFloat)
        {
            myFireFloat -= 0.2f;
        }

        if (myhp == 0 && !destroyed)
        {
            destroyed = true;

            if (team == (string)PhotonNetwork.LocalPlayer.CustomProperties["team"] && name.Contains("Turret"))
            {
                AudioSource underAttack = GameObject.Find("FallenTurret").GetComponent<AudioSource>();
                GameObject.Find("FallenTurret").transform.position = transform.position;
                underAttack.Play();
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

    public void randommovementAI()
    {
        if (name.Contains("HitBase") && GameObject.Find("Building").transform.Find("Base_red")!=null)
        {
            Vector3 basepos =  GameObject.Find("Building").transform.Find("Base_red").transform.position;
            targetPosRand = new Vector3(basepos.x + Random.Range(-10,10), basepos.y, basepos.z + Random.Range(-10, 10));
        }
        else
            targetPosRand = new Vector3(transform.position.x + Random.Range(-3, 4), transform.position.y, transform.position.z + Random.Range(-3, 4));
        GameObject.Find("PMRrobot").transform.LookAt(targetPosRand);
        targetRotRand = GameObject.Find("PMRrobot").transform.rotation.eulerAngles;

        GetComponent<PhotonView>().RPC("randomMoveRPC", RpcTarget.All, targetPosRand, targetRotRand);
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

        if(name.Contains("Turret") && !PlayerPrefs.HasKey("ActNumber"))
        GameObject.Find("Base_"+team).GetComponent<Base>().invulnerable--;

        if (PhotonNetwork.IsMasterClient && PlayerPrefs.GetInt("ActNumber") == 1)
            GameSetupController.instance.gameObject.GetComponent<PhotonView>().RPC("objectiveAct1part1", RpcTarget.All);
        Destroy(gameObject);
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

}
