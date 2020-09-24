using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player1 : MonoBehaviourPun, IPunObservable
{
    [HideInInspector]
    public InputStr Inputs;

    public struct InputStr
    {
        public float LookX;
        public float LookZ;
        public float RunX;
        public float RunZ;
        public float JoystickX;
        public float JoystickXAttack;
        public float JoystickZ;
        public float JoystickZAttack;
        public Vector3 pmrPos;
        public Vector3 pmrRot;
        public bool Jump;
        public bool moving;
        public bool dead;
        public bool attacking;
        public bool attackingDPS;
        public bool Angkat;
        public string boxname;
        public bool angkatbox;
        public bool Crack;
        public bool EnterVehicle;
        public bool movingWithVehicle;
        public bool movingwithJoystick;
        public bool movingwithJoystick2;
        public bool movingwithNebeng;

    }

    public static Player1 player1;

    private float smooth = 0.3f;
    private float height = 7;
    private Vector3 velocity = Vector3.zero;

    private List<PlayerStats> playerStats = new List<PlayerStats>();

    public const float Speed = 10f;
    [SerializeField]
    public float playerSpeed = 4f;
    public const float JumpForce = 5f;
    public LayerMask grounds;
    private GameObject playerMovePoint = PMR.Instance;
    private Transform pmr;


    protected Rigidbody Rigidbody;
    protected Animator Animator;
    protected Quaternion LookRotation;

    protected bool Grounded;

    public GameObject pemainMati;

    public bool selesai;

    [SerializeField]
    public GameObject myVehicle;

    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
        Animator = GetComponentInChildren<Animator>();

        GameSetupController.TimerRespawnOBJ = GameObject.Find("Canvas").transform.Find("RespawnText").gameObject;
        GameSetupController.TimerRespawnOBJ.SetActive(false);

        Inputs.movingWithVehicle = false;

        //destroy the controller if the player is not controlled by me
        if (!photonView.IsMine && GetComponent<Controller>() != null)
            Destroy(GetComponent<Controller>());

        if (photonView.IsMine)
        {
            Inputs.attackingDPS = true;
            Inputs.dead = false;
            Inputs.movingwithJoystick = false;
            Inputs.movingwithJoystick2 = false;
            Inputs.movingwithNebeng = false;

            Hashtable setRace = new Hashtable();
            setRace.Add("position", transform.position);
            PhotonNetwork.LocalPlayer.SetCustomProperties(setRace, null);

            Hashtable setASPD = new Hashtable();
            setASPD.Add("aspd", 20f);
            setASPD.Add("weaponslot1aspd", 20f);
            setASPD.Add("weaponslot2aspd", 20f);
            setASPD.Add("weaponslot3aspd", 20f);
            PhotonNetwork.LocalPlayer.SetCustomProperties(setASPD, null);

            Hashtable setPlayerSpeed = new Hashtable();
            setPlayerSpeed.Add("speed", 4);
            PhotonNetwork.LocalPlayer.SetCustomProperties(setPlayerSpeed, null);

            Hashtable setWpn = new Hashtable();
            setWpn.Add("weapon", "hand");
            setWpn.Add("weaponslotnumber", 1);
            setWpn.Add("weaponslot1", "hand");
            setWpn.Add("weaponslot2", "hand");
            setWpn.Add("weaponslot3", "hand");
            PhotonNetwork.LocalPlayer.SetCustomProperties(setWpn, null);

            Hashtable setWpnTipe = new Hashtable();
            setWpnTipe.Add("weapontipe", "melee");
            setWpnTipe.Add("weaponslot1weapontipe", "melee");
            setWpnTipe.Add("weaponslot2weapontipe", "melee");
            setWpnTipe.Add("weaponslot3weapontipe", "melee");
            PhotonNetwork.LocalPlayer.SetCustomProperties(setWpnTipe, null);

            Hashtable setAmmo = new Hashtable();
            setAmmo.Add("ammo", 100);
            setAmmo.Add("needammo", 0);
            setAmmo.Add("weaponslot1needammo", 0);
            setAmmo.Add("weaponslot2needammo", 0);
            setAmmo.Add("weaponslot3needammo", 0);
            setAmmo.Add("maxammo", 250);
            PhotonNetwork.LocalPlayer.SetCustomProperties(setAmmo, null);

            Hashtable setDmg = new Hashtable();
            setDmg.Add("damage", 5);
            setDmg.Add("weaponslot1damage", 5);
            setDmg.Add("weaponslot2damage", 5);
            setDmg.Add("weaponslot3damage", 5);
            PhotonNetwork.LocalPlayer.SetCustomProperties(setDmg, null);

            Hashtable setDead = new Hashtable();
            setDead.Add("dead", false);
            PhotonNetwork.LocalPlayer.SetCustomProperties(setDead, null);

            Hashtable setVehicle = new Hashtable();
            setVehicle.Add("vehicle", "");
            PhotonNetwork.LocalPlayer.SetCustomProperties(setVehicle, null);

            Hashtable setHP = new Hashtable();
            setHP.Add("health", 100);
            setHP.Add("maxhealth", 150);
            PhotonNetwork.LocalPlayer.SetCustomProperties(setHP, null);
            GetComponent<PhotonView>().RPC("getHearts", RpcTarget.All, name, (int)PhotonNetwork.LocalPlayer.CustomProperties["health"], (int)PhotonNetwork.LocalPlayer.CustomProperties["maxhealth"]);
            }

    }

    private void Start()
    {
        name = "Player (" + GetComponent<PhotonView>().Owner.NickName + ")";
        transform.parent = GameObject.Find("PlayerSpawn").transform;

        if (GetComponent<PhotonView>().IsMine)
        {
            string owner = GetComponent<PhotonView>().Owner.NickName;
            string colorteam = (string)GetComponent<PhotonView>().Owner.CustomProperties["team"];
            string nama = name;
            string namaParent = transform.parent.name;
            GetComponent<PhotonView>().RPC("init", RpcTarget.All, owner, nama, namaParent, colorteam);

            int myammo = (int)PhotonNetwork.LocalPlayer.CustomProperties["ammo"];
            float myammofloat = (float)myammo / (int)PhotonNetwork.LocalPlayer.CustomProperties["maxammo"];
            GameObject.Find("Canvas").transform.Find("AmmoBar").transform.Find("GreenBar").GetComponent<Image>().fillAmount = myammofloat;
            GameObject.Find("Canvas").transform.Find("AmmoBar").transform.Find("TextAmmo").GetComponent<Text>().text = myammo.ToString();
            int myhp = (int)PhotonNetwork.LocalPlayer.CustomProperties["health"];
            float myhpfloat = (float)myhp / (int)PhotonNetwork.LocalPlayer.CustomProperties["maxhealth"];
            GameObject.Find("Canvas").transform.Find("HealthBar").transform.Find("GreenBar").GetComponent<Image>().fillAmount = myhpfloat;
            GameObject.Find("Canvas").transform.Find("HealthBar").transform.Find("Text").GetComponent<Text>().text = myhp.ToString();


            /*foreach (Player player in PhotonNetwork.PlayerList) //loop through each player and create a player listing
            {
                playerStats.Add(new PlayerStats(player, (Vector3)player.CustomProperties["position"]));
            }*/
        }


    }

    

    [PunRPC]
    void init(string owner, string nama, string namaParent, string colorteam)
    {
        if (GetComponent<PhotonView>().Owner.NickName.Equals(owner))
        {
            name = nama;
            transform.parent = GameObject.Find(namaParent).transform;
            
            Color teamcolor = Color.red;
            if (colorteam == "red") teamcolor = Color.red;
            else
            if (colorteam == "blue") teamcolor = Color.blue;
            else
            if (colorteam == "yellow") teamcolor = Color.yellow;
            else
            if (colorteam == "green") teamcolor = Color.green;
            transform.Find("Canvas").transform.Find("MyBar").transform.Find("TextUsername").GetComponent<TextMeshProUGUI>().text = owner;
            transform.Find("Canvas").transform.Find("MyBar").transform.Find("Health Bar1").transform.Find("GreenBar").GetComponent<Image>().color = teamcolor;
            if (photonView.IsMine) GameObject.Find("Canvas").transform.Find("HealthBar").transform.Find("GreenBar").GetComponent<Image>().color = teamcolor;
            if (colorteam == "blue")
            {
                if(photonView.IsMine)
                GameObject.Find("Canvas").transform.Find("HealthBar").transform.Find("Text").GetComponent<Text>().color = Color.white;
                transform.Find("Canvas").transform.Find("MyBar").transform.Find("Health Bar1").transform.Find("Text").GetComponent<Text>().color = Color.white;
            }
        }
    }

    [PunRPC]
    void getWeaponAnimate(string nameOwnerObj, string weapon, string nickname, string lastweapon, string nameObjWepn)
    {
        GameObject ownerChanges = GameObject.Find("PlayerSpawn").transform.Find(nameOwnerObj).transform.Find("Player").gameObject;
        //GameObject ownerChangeWpn = GameObject.Find("PlayerSpawn").transform.Find(nameOwnerObj).transform.Find("Player"+weapon).gameObject;
        //GameObject OwnerlastWeapon = GameObject.Find("PlayerSpawn").transform.Find(nameOwnerObj).transform.Find("Player"+ lastweapon).gameObject;
        if (weapon == "hand") weapon = "";
        foreach (Player player in PhotonNetwork.PlayerList) //loop through each player and create a player listing
        {
            if (player.NickName == nickname)
            {
                Debug.Log("BERUBAH");
                //ownerChangeWpn.SetActive(true);
                //OwnerlastWeapon.SetActive(false);

                if (PhotonNetwork.IsMasterClient && nameObjWepn!="")
                    PhotonNetwork.Destroy(GameObject.Find(nameObjWepn).GetComponent<PhotonView>());
                if (GetComponent<PhotonView>().IsMine && name.Equals(nameOwnerObj))
                {
                    Debug.Log("BERUBAH 2 "+ ownerChanges.name);
                    PhotonNetwork.Destroy(ownerChanges.GetComponent<PhotonView>());
                    if ((string)PhotonNetwork.LocalPlayer.CustomProperties["character"] == "1")
                    {
                        GameObject go2 = PhotonNetwork.Instantiate(Path.Combine("Models/Patrick", "Player"+weapon), transform.position, transform.rotation);
                    }
                    else if ((string)PhotonNetwork.LocalPlayer.CustomProperties["character"] == "2")
                    {
                        GameObject go2 = PhotonNetwork.Instantiate(Path.Combine("Models/Spongebob", "Player"+weapon), transform.position, transform.rotation);
                    }
                    else if ((string)PhotonNetwork.LocalPlayer.CustomProperties["character"] == "3")
                    {
                        GameObject go2 = PhotonNetwork.Instantiate(Path.Combine("Models/Squidward", "Player"+weapon), transform.position, transform.rotation);
                    }
                    else if ((string)PhotonNetwork.LocalPlayer.CustomProperties["character"] == "4")
                    {
                        GameObject go2 = PhotonNetwork.Instantiate(Path.Combine("Models/Sandy", "Player" + weapon), transform.position, transform.rotation);
                    }
                    else if ((string)PhotonNetwork.LocalPlayer.CustomProperties["character"] == "5")
                    {
                        GameObject go2 = PhotonNetwork.Instantiate(Path.Combine("Models/Mr Krabs", "Player" + weapon), transform.position, transform.rotation);
                    }
                    else if ((string)PhotonNetwork.LocalPlayer.CustomProperties["character"] == "6")
                    {
                        GameObject go2 = PhotonNetwork.Instantiate(Path.Combine("Models/Orang", "Player" + weapon), transform.position, transform.rotation);
                    }
                    else if ((string)PhotonNetwork.LocalPlayer.CustomProperties["character"] == "7")
                    {
                        GameObject go2 = PhotonNetwork.Instantiate(Path.Combine("Models/Larry", "Player" + weapon), transform.position, transform.rotation);
                    }
                    else if ((string)PhotonNetwork.LocalPlayer.CustomProperties["character"] == "8")
                    {
                        GameObject go2 = PhotonNetwork.Instantiate(Path.Combine("Models/Mrs Puff", "Player" + weapon), transform.position, transform.rotation);
                    }
                }
            }
        }

        
    }

    [PunRPC]
    void destroyPlayer()
    {
    }

    [PunRPC]
    void losing()
    {
        transform.Find("Canvas").gameObject.SetActive(false);
        enabled = false;
    }

    [PunRPC]
    void EnterVehicleRPC(string nameEnter, string VehicleName)
    {
        if(name== nameEnter)
        {
            myVehicle = GameObject.Find(VehicleName).gameObject;
            
            for (var i = 0; i < myVehicle.GetComponent<Vehicle>().TempatDuduk.Count; i++)
            {
                Debug.Log("myposisi " + myVehicle.GetComponent<Vehicle>().TempatDuduk[i]);
                if (myVehicle.GetComponent<Vehicle>().TempatDuduk[i] == name)
                {
                    int myposisi = i + 1;
                    Vector3 vehicleJokPos = myVehicle.transform.Find("GameObject"+myposisi).transform.position;
                    Quaternion vehicleJokRot = myVehicle.transform.Find("GameObject"+myposisi).transform.rotation;
                    transform.position = vehicleJokPos;
                    transform.rotation = vehicleJokRot;
                    Debug.Log("myposisi " + myposisi);
                    if (myposisi > 1) Inputs.movingwithNebeng = true;
                    break;
                }
            }
            GetComponent<Rigidbody>().detectCollisions = false;
            GetComponent<Rigidbody>().isKinematic = true;
            
            Inputs.movingWithVehicle = true;

            if (myVehicle.name.Contains("tank"))
            {
                AudioSource audio = GameObject.Find(VehicleName).transform.Find("tankwalk").GetComponent<AudioSource>();
                audio.Play();
            }else if (myVehicle.name.Contains("bike"))
            {
                if (myVehicle.GetComponent<Vehicle>().TempatDuduk[0] == name)
                {
                    AudioSource audio = GameObject.Find(VehicleName).transform.Find("BikeStart").GetComponent<AudioSource>();
                    audio.Play();
                    audio = GameObject.Find(VehicleName).transform.Find("BikeNgegas").GetComponent<AudioSource>();
                    audio.Play();
                }
                myVehicle.transform.eulerAngles = new Vector3(myVehicle.transform.eulerAngles.x,myVehicle.transform.eulerAngles.y,0f);
            }

        }
    }

    [PunRPC]
    void OutVehicleRPC(string nameEnter, string VehicleName)
    {
        if (name == nameEnter)
        {
            myVehicle = GameObject.Find(VehicleName).gameObject;
            for(var i=0;i< myVehicle.GetComponent<Vehicle>().TempatDuduk.Count; i++)
            {
                if (myVehicle.GetComponent<Vehicle>().TempatDuduk[i] == name)
                {
                    int myposisi = i + 1;
                    Vector3 vehicleJokPos = myVehicle.transform.Find("GameObject"+myposisi).transform.position;
                    GetComponent<Rigidbody>().detectCollisions = true;
                    GetComponent<Rigidbody>().isKinematic = false;
                    transform.position = new Vector3(vehicleJokPos.x, vehicleJokPos.y + 1f, vehicleJokPos.z);
                    if (myposisi > 1) Inputs.movingwithNebeng = false;
                    break;
                }
            }
            
            GetComponent<Rigidbody>().detectCollisions = true;
            GetComponent<Rigidbody>().isKinematic = false;
            Inputs.movingWithVehicle = false;

            if (myVehicle.name.Contains("tank"))
            {
                AudioSource audio = GameObject.Find(VehicleName).transform.Find("tankwalk").GetComponent<AudioSource>();
                audio.Stop();
            }
            else if (myVehicle.name.Contains("bike"))
            {
                if (myVehicle.GetComponent<Vehicle>().TempatDuduk[0] == name && myVehicle.GetComponent<Vehicle>().TempatDuduk.Count==1)
                {
                    AudioSource audio = GameObject.Find(VehicleName).transform.Find("BikeNgegas").GetComponent<AudioSource>();
                    audio.Stop();
                }
            }

            if (GameObject.Find(name).GetComponent<PhotonView>().IsMine)
            {
                if (myVehicle.transform.Find("Honk") != null)
                    GetComponent<Controller>().ButtonThrowWeapon.transform.Find("GameObject").GetComponent<Image>().overrideSprite = Resources.Load<Sprite>("[UIFabrica]TrollNest_Free_v01/02.PNG/ThrowWeapon");
                else GetComponent<Controller>().ButtonThrowWeapon.gameObject.SetActive(true);
                GetComponent<Controller>().ButtonPickUpBox.gameObject.SetActive(true);
                GetComponent<Controller>().ButtonJump.gameObject.SetActive(true);
            }

            myVehicle = null;
        }
    }

    [PunRPC]
    void myBoxRPC(string nama, bool gravity, bool kinematic, bool detectcol)
    {
        GameObject.Find(nama).GetComponent<Rigidbody>().useGravity = gravity;
        GameObject.Find(nama).GetComponent<Rigidbody>().isKinematic = kinematic;
        GameObject.Find(nama).GetComponent<Rigidbody>().detectCollisions = detectcol;
    }
    

    [PunRPC]
    void VehicleHonk(string nama)
    {
        if (nama == name && GameObject.Find("Honk")!=null)
        {
            Vector3 PosHonk = transform.position;
            AudioSource audio = myVehicle.transform.Find("Honk").GetComponent<AudioSource>();
            GameObject.Find("Honk").transform.position = PosHonk;
            audio.Play();
        }
    }

    [PunRPC]
    void getHearts(string namaPemilikHP, int hp, int maxhp)
    {
        float myhp = (float)hp / maxhp;
        GameObject.Find(namaPemilikHP).transform.Find("Canvas").transform.Find("MyBar").transform.Find("Health Bar1").transform.Find("GreenBar").GetComponent<Image>().fillAmount = myhp;
        GameObject.Find(namaPemilikHP).transform.Find("Canvas").transform.Find("MyBar").transform.Find("Health Bar1").transform.Find("Text").GetComponent<Text>().text = hp.ToString();
    }

    [PunRPC]
    void refreshBar(float myhpfloat, int myhp)
    {
        GameObject.Find("Canvas").transform.Find("HealthBar").transform.Find("GreenBar").GetComponent<Image>().fillAmount = myhpfloat;
        GameObject.Find("Canvas").transform.Find("HealthBar").transform.Find("Text").GetComponent<Text>().text = myhp.ToString();
    }

    [PunRPC]
    void soundpunch2(string namaPemilikHP)
    {
        if (GameObject.Find(namaPemilikHP) != null)
        {
            AudioSource audio = GameObject.Find("Punch2").GetComponent<AudioSource>();
            GameObject.Find("Punch2").transform.position = GameObject.Find(namaPemilikHP).transform.position;
            audio.Play();
        }
    }

    [PunRPC]
    void getRune(string namaRune)
    {
        if (namaRune == "PowerHaste")
        {
            playerSpeed += 4;
            Invoke("RuneExpired",10f);
        }
    }

    public void RuneExpired()
    {
        playerSpeed -= 4;
    }

    [PunRPC]
    void attackingPlayer(string namaPemilikHP, int hp, int maxhp,Player player)
    {
        float myhp = (float)hp / maxhp;
        GameObject.Find(namaPemilikHP).transform.Find("Canvas").transform.Find("MyBar").transform.Find("Health Bar1").transform.Find("GreenBar").GetComponent<Image>().fillAmount = myhp;
        GameObject.Find(namaPemilikHP).transform.Find("Canvas").transform.Find("MyBar").transform.Find("Health Bar1").transform.Find("Text").GetComponent<Text>().text = hp.ToString();

        pemainMati = GameObject.Find(namaPemilikHP).gameObject;
        if (GameObject.Find(namaPemilikHP).GetComponent<PhotonView>().IsMine)
        {
            GameObject.Find("Canvas").transform.Find("HealthBar").transform.Find("GreenBar").GetComponent<Image>().fillAmount = myhp;
            GameObject.Find("Canvas").transform.Find("HealthBar").transform.Find("Text").GetComponent<Text>().text = hp.ToString();

            if (hp == 0)
            {
                pemainMati.GetComponent<Controller>().enabled = false;
                pemainMati.GetComponent<Player1>().Inputs.RunX = 0;
                pemainMati.GetComponent<Player1>().Inputs.RunZ = 0;
                pemainMati.GetComponent<Player1>().Inputs.JoystickX = 0;
                pemainMati.GetComponent<Player1>().Inputs.JoystickZ = 0;
                pemainMati.GetComponent<Player1>().Inputs.JoystickXAttack = 0;
                pemainMati.GetComponent<Player1>().Inputs.JoystickZAttack = 0;
                pemainMati.GetComponent<Player1>().Inputs.moving = false;
                pemainMati.GetComponent<Player1>().Inputs.Jump = false;
                pemainMati.GetComponent<Player1>().Inputs.Angkat = false;
                GameSetupController.TimerRespawnOBJ.SetActive(true);
                GameSetupController.CustomeValue = new ExitGames.Client.Photon.Hashtable();
                GameSetupController.startTime = PhotonNetwork.Time;
                GameSetupController.startTimer = true;
                GameSetupController.CustomeValue.Add("StartTime", GameSetupController.startTime);
                player.SetCustomProperties(GameSetupController.CustomeValue);
                if (PlayerPrefs.HasKey("ActNumber"))
                {
                    GameObject.Find("AdManager").GetComponent<AdManager>().RequestInterstitial();
                    selesai = false;
                }
                

                Vector3 PosMati = GameObject.Find(namaPemilikHP).transform.position;
                AudioSource audio = GameObject.Find("Dead").GetComponent<AudioSource>();
                GameObject.Find("Dead").transform.position = PosMati;
                Debug.Log("DEAD");
                audio.Play();
            }
        }
        if (hp == 0)
        {
            GameObject.Find(namaPemilikHP).transform.Find("Player").GetComponent<Animator>().SetInteger("condition", 2);
            StartCoroutine(Dead(namaPemilikHP,player));
        }
        
        if (hp == 0 && PhotonNetwork.IsMasterClient)
        {
            Vector3 PosMati = GameObject.Find(namaPemilikHP).transform.position;
            GameObject go = PhotonNetwork.InstantiateSceneObject(Path.Combine("Models", "Gravestone"), new Vector3(PosMati.x, PosMati.y+10, PosMati.z), Quaternion.identity);
            go.transform.Find("NamaRIP").GetComponent<TextMesh>().text = namaPemilikHP;
            go.name = "Gravestone (" + go.GetPhotonView().ViewID + ")";
            go.GetComponent<PhotonView>().RPC("itemrpc", RpcTarget.All, go.name, "Grave Spawn", getMyNickName(namaPemilikHP));
        }
    }

    public System.Collections.IEnumerator Dead(string namaPemilikHP,Player player)
    {
        GameObject pemainMati = GameObject.Find(namaPemilikHP);
        if (pemainMati.GetComponent<Player1>().myVehicle != null)
        {
            string myVehicleName = (string)player.CustomProperties["vehicle"];
            if (PhotonNetwork.IsMasterClient)
            {
                ExitGames.Client.Photon.Hashtable setRace = new ExitGames.Client.Photon.Hashtable();
                setRace.Add("weapon", player.CustomProperties["weaponTemp"]);
                setRace.Add("weapontipe", player.CustomProperties["weapontipeTemp"]);
                setRace.Add("needammo", player.CustomProperties["needammoTemp"]);
                setRace.Add("aspd", player.CustomProperties["aspdTemp"]);
                setRace.Add("vehicle", "");
                player.SetCustomProperties(setRace, null);
                pemainMati.GetComponent<PhotonView>().RPC("OutVehicleRPC", RpcTarget.All, pemainMati.name, myVehicleName);
                GameObject.Find(myVehicleName).GetComponent<PhotonView>().RPC("vehicleRemove", RpcTarget.All, pemainMati.name);
            }
            pemainMati.GetComponent<Player1>().Inputs.EnterVehicle = false;
        }

        yield return new WaitForSeconds(1.3f);
        if (GameObject.Find(namaPemilikHP).GetComponent<PhotonView>().IsMine)
        {
            GameSetupController.instance.go2 = GameObject.Find(namaPemilikHP).gameObject;
            Debug.Log("MAU respawn");
            GameSetupController.instance.StartCoroutine("ReDead");
        }
        GameObject.Find(namaPemilikHP).SetActive(false);


    }

    

    [PunRPC]
    void getAmmo(string namaPemilikAmmo, int ammo, int maxammo)
    {
        float myammo = (float)ammo / maxammo;
        GameObject.Find("Canvas").transform.Find("AmmoBar").transform.Find("GreenBar").GetComponent<Image>().fillAmount = myammo;
        GameObject.Find("Canvas").transform.Find("AmmoBar").transform.Find("TextAmmo").GetComponent<Text>().text = ammo.ToString();
    }

    [PunRPC]
    void ThrowBoxRPC(string myboxname, string namapelempar)
    {
        Debug.Log(myboxname);
        GameObject.Find("BoxSpawn").transform.Find(myboxname).GetComponent<Rigidbody>().useGravity = true;
        GameObject.Find("BoxSpawn").transform.Find(myboxname).GetComponent<Rigidbody>().isKinematic = false;
        GameObject.Find("BoxSpawn").transform.Find(myboxname).GetComponent<Rigidbody>().detectCollisions = true;
        GameObject.Find("BoxSpawn").transform.Find(myboxname).GetComponent<Rigidbody>().AddForce(GetComponent<Rigidbody>().transform.forward * 10, ForceMode.VelocityChange);
        if (namapelempar == name)
        {
            Inputs.Angkat = false;
            Inputs.angkatbox = false;
        }
    }

    [PunRPC]
    void ThrowWeaponRPC(string myweapon, float tambahX, float tambahZ)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GameObject go = PhotonNetwork.InstantiateSceneObject(Path.Combine("Models/Weapon", myweapon), new Vector3(transform.position.x + tambahX, transform.position.y + 0.5f, transform.position.z + tambahZ), transform.rotation);
            go.name = myweapon+" (" + go.GetPhotonView().ViewID + ")";
            go.GetComponent<PhotonView>().RPC("weaponrpcThrow", RpcTarget.All, go.name, "Weapon Street Spawn");
        }
    }

    [PunRPC]
    void crackBoxRPC(string nama, Vector3 posisi)
    {
        Inputs.Crack = false;
        Random rnd = new Random();
        if (PhotonNetwork.IsMasterClient)
        {
            if(GameObject.Find(nama)!=null)
            if (GameObject.Find(nama).GetComponent<Cube>().getBoxInStasiun() == "")
            {
                PhotonNetwork.Destroy(PhotonView.Find(GameObject.Find(nama).GetComponent<PhotonView>().ViewID));
                int randomitem = Random.Range(1, 3);
                if (randomitem == 1)
                {
                    if (nama.Contains("Blue"))
                    {
                        randomitem = Random.Range(2, 4);
                    }
                    else randomitem = 1;
                    for (var i = 0; i < randomitem; i++)
                    {
                        float randompos = Random.Range(-0.3f, 0.3f);
                        GameObject go = PhotonNetwork.InstantiateSceneObject(Path.Combine("Models", "Bullets"), new Vector3(posisi.x + randompos, posisi.y, posisi.z + randompos), Quaternion.identity);
                        go.name = "ItemBullets (" + go.GetPhotonView().ViewID + ")";
                        go.GetComponent<PhotonView>().RPC("itemrpc", RpcTarget.All, go.name, "Ammo Heart Spawn");
                    }
                }
                else
                {
                    if (nama.Contains("Blue"))
                    {
                        randomitem = Random.Range(2, 4);
                    }
                    else randomitem = 1;
                    for (var i = 0; i < randomitem; i++)
                    {
                        float randompos = Random.Range(-0.3f, 0.3f);
                        GameObject go = PhotonNetwork.InstantiateSceneObject(Path.Combine("Models", "Hearts"), new Vector3(posisi.x + randompos, posisi.y, posisi.z + randompos), Quaternion.identity);
                        go.name = "ItemHearts (" + go.GetPhotonView().ViewID + ")";
                        go.GetComponent<PhotonView>().RPC("itemrpc", RpcTarget.All, go.name, "Ammo Heart Spawn");
                    }
                }
            }
            else
            {
                GameObject.Find(GameObject.Find(nama).GetComponent<Cube>().getBoxInStasiun()).GetComponent<BoxCrafting>().crackInStation();
            }
            
        }

        int randomsound = Random.Range(1, 5);
        Debug.Log("MYSOUND: " + randomsound);
        AudioSource audio = GameObject.Find("BoxCrash" + randomsound).GetComponent<AudioSource>();
        GameObject.Find("BoxCrash" + randomsound).transform.position = posisi;
        audio.Play();


    }

    private void Update()
    {
        //Animator.SetBool("Grounded", Grounded);

        var localVelocity = Quaternion.Inverse(transform.rotation) * (Rigidbody.velocity / Speed);
        //Animator.SetFloat("RunX", localVelocity.x);
        //Animator.SetFloat("RunZ", localVelocity.z);
        if(PlayerPrefs.HasKey("ActNumber"))
        if (GameObject.Find("AdManager").GetComponent<AdManager>().interstitial != null)
            if (!selesai && GameObject.Find("AdManager").GetComponent<AdManager>().interstitial.IsLoaded())
            {
                GameObject.Find("AdManager").GetComponent<AdManager>().ShowInterstitial();
                selesai = true;
            }

    }

    void FixedUpdate()
    {
        /*if (Vector3.Distance(transform.position, (Vector3)GetComponent<PhotonView>().Owner.CustomProperties["position"]) > 1f)
        {
            transform.position = (Vector3)GetComponent<PhotonView>().Owner.CustomProperties["position"];
        }*/
        if (GetComponent<PhotonView>().IsMine)
        {
            Vector3 pos = new Vector3();
            pos.x = transform.position.x;
            pos.z = transform.position.z - 7f;
            pos.y = transform.position.y + height;
            Camera.main.transform.position = Vector3.SmoothDamp(Camera.main.transform.position, pos, ref velocity, smooth);
            Camera.main.transform.LookAt(transform);
        }

        var inputRun = Vector3.ClampMagnitude(new Vector3(Inputs.RunX, 0, Inputs.RunZ), 1);

        double worldScreenHeight = Screen.height;
        double worldScreenWidth = Screen.width;

        Vector3 posisi = Camera.main.WorldToScreenPoint(new Vector3(transform.position.x, transform.position.y + 2.2f, transform.position.z));

        transform.Find("Canvas").transform.Find("MyBar").localPosition = new Vector3((float)(((posisi.x) - worldScreenWidth / 2)), (float)(((posisi.y) - worldScreenHeight / 2)), 0);
        transform.Find("Canvas").transform.Find("MyBar").localScale = new Vector3((float)(worldScreenWidth / 1024), (float)(worldScreenHeight / 768), 1);

        //transform.rotation = LookRotation;
        
        if (Inputs.Jump)
        {
            Grounded = Physics.OverlapSphere(transform.position, 0.3f, LayerMask.GetMask("Ground")).Length != 0;
            if (Grounded)
            {
                Rigidbody.velocity = new Vector3(Rigidbody.velocity.x, JumpForce, Rigidbody.velocity.z);
            }
        }


        if (Inputs.movingwithNebeng) Inputs.moving = false;
        if (Inputs.moving)
        {
            if (Inputs.movingWithVehicle)
            {
                if(myVehicle.name.Contains("bike") && myVehicle.transform.eulerAngles.y>=45)
                myVehicle.transform.eulerAngles = new Vector3(myVehicle.transform.eulerAngles.x, myVehicle.transform.eulerAngles.y, 0f);
                Inputs.pmrPos = new Vector3(Inputs.pmrPos.x, myVehicle.transform.position.y, Inputs.pmrPos.z);
                if (Vector3.Distance(new Vector3(Inputs.pmrPos.x, myVehicle.transform.position.y, Inputs.pmrPos.z), myVehicle.transform.position) > 0.1f)
                {
                    transform.Find("Player").GetComponent<Animator>().SetInteger("condition", 0);

                    myVehicle.transform.position = Vector3.MoveTowards(myVehicle.transform.position, new Vector3(Inputs.pmrPos.x, myVehicle.transform.position.y, Inputs.pmrPos.z), myVehicle.GetComponent<Vehicle>().speed * Time.deltaTime);

                    Quaternion lookRot = Quaternion.Euler(Inputs.pmrRot.x, Inputs.pmrRot.y, Inputs.pmrRot.z);
                    myVehicle.transform.rotation = Quaternion.Slerp(myVehicle.transform.rotation, lookRot, myVehicle.GetComponent<Vehicle>().belokSpeed * Time.deltaTime);
                }
                else
                {
                    transform.Find("Player").GetComponent<Animator>().SetInteger("condition", 0);
                    Inputs.moving = false;
                }
            }
            else
            if (Vector3.Distance(Inputs.pmrPos, new Vector3(transform.position.x, Inputs.pmrPos.y, transform.position.z)) > 0.1f)
            {
                transform.Find("Player").GetComponent<Animator>().SetInteger("condition", 1);
                Inputs.pmrPos.y = transform.position.y;
                transform.position = Vector3.MoveTowards(transform.position, Inputs.pmrPos, playerSpeed * Time.deltaTime);
                transform.LookAt(Inputs.pmrPos);
                Rigidbody.angularVelocity = new Vector3(0, 0, 0);
            }
            else
            {
                transform.Find("Player").GetComponent<Animator>().SetInteger("condition", 0);
                transform.position = Inputs.pmrPos;
                Inputs.moving = false;
                Rigidbody.angularVelocity = new Vector3(0, 0, 0);

            }
        }



        if (Inputs.Angkat)
        {
            RealPickup();
        }

        if (Inputs.RunX != 0 || Inputs.RunZ != 0)
        {
            float angle = Mathf.Atan2(Inputs.RunX, Inputs.RunZ) * Mathf.Rad2Deg;
            Vector3 myvector = new Vector3(transform.position.x + Inputs.RunX * Speed * Time.deltaTime, transform.position.y, transform.position.z + Inputs.RunZ * Speed * Time.deltaTime);
            if (Inputs.movingWithVehicle)
                myvector = new Vector3(transform.position.x + Inputs.RunX * Speed * 15 * Time.deltaTime, transform.position.y, transform.position.z + Inputs.RunZ * 15 * Speed * Time.deltaTime);

            Inputs.pmrPos = myvector;
            if (Vector3.Distance(Inputs.pmrPos, transform.position) > 0)
            {
                Vector3 pmrFront = new Vector3(Inputs.pmrPos.x, transform.position.y, Inputs.pmrPos.z);
                Quaternion tempRot = transform.rotation;
                
                transform.LookAt(new Vector3(Inputs.pmrPos.x, transform.position.y, Inputs.pmrPos.z));
                Inputs.pmrRot = transform.eulerAngles;
                
                Inputs.moving = true;
                Inputs.movingwithJoystick = true;
            }
        }
        else
        if (Inputs.movingWithVehicle && Inputs.movingwithJoystick)
        {
            Inputs.moving = false;
            Inputs.movingwithJoystick = false;
        }

        if (Inputs.JoystickX != 0 && Inputs.JoystickZ != 0)
        {
            float angle = Mathf.Atan2(Inputs.JoystickX, Inputs.JoystickZ) * Mathf.Rad2Deg;
            Vector3 myvector = new Vector3(transform.position.x + Inputs.JoystickX * Speed * Time.deltaTime, transform.position.y, transform.position.z + Inputs.JoystickZ * Speed * Time.deltaTime);
            if (Inputs.movingWithVehicle)
                myvector = new Vector3(transform.position.x + Inputs.JoystickX * Speed * 15 * Time.deltaTime, transform.position.y, transform.position.z + Inputs.JoystickZ * 15 * Speed * Time.deltaTime);

            Inputs.pmrPos = myvector;
            if (Vector3.Distance(Inputs.pmrPos, transform.position) > 0)
            {
                Vector3 pmrFront = new Vector3(Inputs.pmrPos.x, transform.position.y, Inputs.pmrPos.z);
                Quaternion tempRot = transform.rotation;
                
                transform.LookAt(new Vector3(Inputs.pmrPos.x, transform.position.y, Inputs.pmrPos.z));
                Inputs.pmrRot = transform.eulerAngles;
                
                Inputs.moving = true;
                Inputs.movingwithJoystick2 = true;
            }
        }
        else
        if (Inputs.movingWithVehicle && Inputs.movingwithJoystick2)
        {
            Inputs.moving = false;
            Inputs.movingwithJoystick2 = false;
        }
        


        if (Inputs.JoystickXAttack != 0 || Inputs.JoystickZAttack != 0)
        {
            float angle = Mathf.Atan2(Inputs.JoystickXAttack, Inputs.JoystickZAttack) * Mathf.Rad2Deg;
            Vector3 myvector = new Vector3(transform.position.x + Inputs.JoystickXAttack * Speed * Time.deltaTime, transform.position.y, transform.position.z + Inputs.JoystickZAttack * Speed * Time.deltaTime);



            if (Vector3.Distance(myvector, transform.position) > 0)
            {
                
                if (Inputs.movingWithVehicle && (myVehicle.name.Contains("bike") || myVehicle.name.Contains("tank")))
                {
                    for (var i = 0; i < myVehicle.GetComponent<Vehicle>().TempatDuduk.Count; i++)
                    {
                        if (myVehicle.GetComponent<Vehicle>().TempatDuduk[i] == name)
                        {
                            int myposisi = i + 1;
                            if(myposisi==1 && myVehicle.name.Contains("bike"))
                            {
                                transform.LookAt(new Vector3(myvector.x, transform.position.y, myvector.z));
                            }else
                            {
                                myvector = transform.position + transform.forward;
                                GameObject.Find("PlayerMovePoint").transform.position = transform.position;
                                GameObject.Find("PlayerMovePoint").transform.LookAt(myvector);
                                Vector3 MyEuler = GameObject.Find("PlayerMovePoint").transform.eulerAngles;

                                Quaternion lookRot = myVehicle.transform.Find("TopTembakan" + myposisi).transform.rotation;
                                if (angle < 0) angle = 360 - Mathf.Abs(angle);

                                if (myVehicle.transform.Find("TopTembakan" + myposisi).transform.eulerAngles.y - angle >= 180)
                                    lookRot *= Quaternion.AngleAxis(30, Vector3.up);
                                else
                                if (myVehicle.transform.Find("TopTembakan" + myposisi).transform.eulerAngles.y - angle >= 0)
                                    lookRot *= Quaternion.AngleAxis(-30, Vector3.up);
                                else if (myVehicle.transform.Find("TopTembakan" + myposisi).transform.eulerAngles.y - angle <= -180)
                                    lookRot *= Quaternion.AngleAxis(-30, Vector3.up);
                                else lookRot *= Quaternion.AngleAxis(30, Vector3.up);

                                if (Mathf.Abs(myVehicle.transform.Find("TopTembakan" + myposisi).transform.eulerAngles.y - angle) > 5)
                                {
                                    myVehicle.transform.Find("TopTembakan" + myposisi).transform.rotation = Quaternion.Lerp(myVehicle.transform.Find("TopTembakan" + myposisi).transform.rotation, lookRot, myVehicle.GetComponent<Vehicle>().speedMuter * Time.deltaTime);
                                }
                            }
                            

                            break;
                        }
                    }
                }
                else
                {
                    transform.LookAt(new Vector3(myvector.x, transform.position.y, myvector.z));
                    Inputs.pmrRot = transform.eulerAngles;
                }

                GameObject firstActiveGameObject;

                string myNickName = getMyNickName(name);
                float myaspd = 0;

                foreach (Player player in PhotonNetwork.PlayerList) //loop through each player and create a player listing
                {
                    if (player.NickName == myNickName)
                    {
                        myaspd = (float)player.CustomProperties["aspd"];
                        myaspd = ((200f + (float)player.CustomProperties["aspd"]) * 0.01f) / 1.7f;
                        firstActiveGameObject = transform.Find("Player").gameObject;
                        firstActiveGameObject.GetComponent<Animator>().SetInteger("attacking", 1);
                        firstActiveGameObject.GetComponent<Animator>().speed = myaspd;

                        if ((string)player.CustomProperties["weapontipe"] == "melee")
                        {
                            int damage = (int)player.CustomProperties["damage"];
                            if (Inputs.attackingDPS) StartCoroutine(Attacking(damage,myaspd));
                            Inputs.attackingDPS = false;
                        }else if((string)player.CustomProperties["weapontipe"] == "range")
                        {
                            myaspd = (float)player.CustomProperties["aspd"];
                            myaspd = ((300f + (float)player.CustomProperties["aspd"]) * 0.01f) / 1.7f;
                            if (Inputs.attackingDPS) StartCoroutine(AttackingRange(player,myaspd));
                            Inputs.attackingDPS = false;
                        }
                        Inputs.attacking = true;
                        break;
                    }
                }
            }
        }
        else if (Inputs.attacking)
        {
            GameObject firstActiveGameObject;

            firstActiveGameObject = transform.Find("Player").gameObject;
            firstActiveGameObject.GetComponent<Animator>().SetInteger("attacking", 0);
            firstActiveGameObject.GetComponent<Animator>().speed = 1;
            Inputs.attacking = false;
                    
        }

        if (Inputs.movingWithVehicle)
        {
            for (var i = 0; i < myVehicle.GetComponent<Vehicle>().TempatDuduk.Count; i++)
            {
                if (myVehicle.GetComponent<Vehicle>().TempatDuduk[i] == name)
                {
                    int myposisi = i + 1;
                    transform.position = myVehicle.transform.Find("GameObject"+ myposisi).position;
                    if (myVehicle.name.Contains("tank"))
                        transform.rotation = myVehicle.transform.Find("TopTembakan"+myposisi).rotation;
                    else if (myVehicle.name.Contains("bike"))
                    {
                        if (myposisi == 1) transform.rotation = myVehicle.transform.Find("GameObject" + myposisi).rotation;
                        else transform.rotation = myVehicle.transform.Find("TopTembakan"+myposisi).rotation;
                    }
                    
                    break;
                }
            }
            
        }

        /*
        if (photonView.IsMine)
        {
            Hashtable setRace = new Hashtable();
            setRace.Add("position", transform.position);
            PhotonNetwork.LocalPlayer.SetCustomProperties(setRace, null);
        }*/

    }

    public System.Collections.IEnumerator AttackingRange(Player player, float myaspd)
    {
        yield return new WaitForSeconds(1/myaspd);
        if (!Inputs.attacking)
        {
            Inputs.attackingDPS = true;
            yield break;
        }
        string myWpn = (string)player.CustomProperties["weapon"];
        int myAmmo = (int)player.CustomProperties["ammo"];
        int myNeedAmmo = (int)player.CustomProperties["needammo"];
        if (myAmmo >= myNeedAmmo)
        {
            if(myWpn!="hand")
            GetComponent<PhotonView>().RPC("spawnBullet", RpcTarget.MasterClient, myWpn);
            myAmmo -= myNeedAmmo;
            Hashtable setRace = new Hashtable();
            setRace.Add("ammo", myAmmo);
            player.SetCustomProperties(setRace, null);

            float myammo = (float)myAmmo / (int)player.CustomProperties["maxammo"];
            GameObject.Find("Canvas").transform.Find("AmmoBar").transform.Find("GreenBar").GetComponent<Image>().fillAmount = myammo;
            GameObject.Find("Canvas").transform.Find("AmmoBar").transform.Find("TextAmmo").GetComponent<Text>().text = myAmmo.ToString();
        }
        else
        {
            AudioSource swosh = GameObject.Find("Outofammo").GetComponent<AudioSource>();
            GameObject.Find("Outofammo").transform.position = transform.position;
            swosh.Play();
        }
        Inputs.attackingDPS = true;
    }

    [PunRPC]
    void spawnBullet(string myWpn)
    {
        float angleku = Mathf.Round(transform.eulerAngles.y * 1000f) / 1000f;
        float tambahX = Mathf.Round(Mathf.Sin(angleku * Mathf.Deg2Rad) * 50000) / 100000;
        float tambahZ = Mathf.Round(Mathf.Cos(angleku * Mathf.Deg2Rad) * 50000) / 100000;
        float posisiTembakatas = 0.5f;
        int jumlahbullet = 1;

        if (Inputs.movingWithVehicle && myVehicle.name.Contains("tank"))
        {
            angleku = Mathf.Round(myVehicle.transform.Find("TopTembakan1").transform.eulerAngles.y * 1000f) / 1000f;
            tambahX = Mathf.Round(Mathf.Sin(angleku * Mathf.Deg2Rad) * 200000) / 100000;
            tambahZ = Mathf.Round(Mathf.Cos(angleku * Mathf.Deg2Rad) * 200000) / 100000;
            Vector3 TembakanPos = myVehicle.transform.Find("TopTembakan1").transform.position;
            Quaternion TembakanRot = myVehicle.transform.Find("TopTembakan1").transform.rotation;
            GameObject go = PhotonNetwork.InstantiateSceneObject(Path.Combine("Models", "bullet_" + myWpn), new Vector3(TembakanPos.x + tambahX, TembakanPos.y + 2f, TembakanPos.z + tambahZ), TembakanRot);
            go.name = "Bullet" + myWpn + " (" + go.GetPhotonView().ViewID + ")";
            go.GetComponent<PhotonView>().RPC("itemrpc", RpcTarget.All, go.name, "Projectile Spawn", transform.position, name);
        }
        else
        {
            if (Inputs.movingWithVehicle && myVehicle.name.Contains("bike"))
            {
                angleku = Mathf.Round(transform.eulerAngles.y * 1000f) / 1000f;
                tambahX = Mathf.Round(Mathf.Sin(angleku * Mathf.Deg2Rad) * 100000) / 100000;
                tambahZ = Mathf.Round(Mathf.Cos(angleku * Mathf.Deg2Rad) * 100000) / 100000;
            }
            if (myWpn.Contains("Rocket")) posisiTembakatas = 1.5f;
            if (myWpn.Contains("Flame")) jumlahbullet = 2;

            for (var i = 0; i < jumlahbullet; i++)
            {
                GameObject go = PhotonNetwork.InstantiateSceneObject(Path.Combine("Models", "bullet_" + myWpn), new Vector3(transform.position.x + tambahX, transform.position.y + posisiTembakatas, transform.position.z + tambahZ), transform.rotation);
                go.name = "Bullet" + myWpn + " (" + go.GetPhotonView().ViewID + ")";
                go.GetComponent<PhotonView>().RPC("itemrpc", RpcTarget.All, go.name, "Projectile Spawn", transform.position, name);
            }
        }
    }

        public System.Collections.IEnumerator Attacking(int damage, float myaspd)
    {
        List<GameObject> gos = new List<GameObject>();
        for (int jml = 0; jml < GameObject.FindGameObjectsWithTag("Player").Length; jml++) gos.Add(GameObject.FindGameObjectsWithTag("Player")[jml]);        
        for (int jml = 0; jml < GameObject.FindGameObjectsWithTag("building").Length; jml++) gos.Add(GameObject.FindGameObjectsWithTag("building")[jml]);        

        GameObject closest;
        var distance = 3f;
        var tempangle = 66.5f;
        var position = transform.position;

        AudioSource swosh = GameObject.Find("Swosh").GetComponent<AudioSource>();
        GameObject.Find("Swosh").transform.position = transform.position;
        swosh.Play();
        yield return new WaitForSeconds(1/myaspd);
        if (!Inputs.attacking)
        {
            Inputs.attackingDPS = true;
            yield break;
        }
        // Iterate through them and find the closest one
        foreach (GameObject go in gos)
        {
            if (go.transform != transform)
            {
                var diff = (go.transform.position - position);
                var curDistance = diff.sqrMagnitude;
                float angle = Vector3.Angle(transform.forward, diff);
                if (curDistance < distance && Mathf.Abs(angle) < tempangle)
                {
                    closest = go;
                    distance = curDistance;

                    if (closest.tag.Equals("building") && closest.layer.Equals(LayerMask.NameToLayer("Vehicle")))
                    {
                        ExitGames.Client.Photon.Hashtable setHP = new ExitGames.Client.Photon.Hashtable();
                        int myhp = (int)PhotonNetwork.CurrentRoom.CustomProperties[closest.name + "health"] - damage;
                        if (myhp <= 0) myhp = 0;
                        setHP.Add(closest.name + "health", myhp);

                        PhotonNetwork.CurrentRoom.SetCustomProperties(setHP, null);
                        float myhpfloat = (float)myhp / (int)PhotonNetwork.CurrentRoom.CustomProperties[closest.name + "maxhealth"];

                        if (closest.name.Contains("Robot")) closest.GetComponent<PhotonView>().RPC("getDamaged", RpcTarget.All, myhp, closest.GetComponent<Robot>().myFireFloat);
                        else closest.GetComponent<PhotonView>().RPC("getDamaged", RpcTarget.All, myhp, closest.GetComponent<Vehicle>().myFireFloat);
                        Debug.Log("kena vehicle");
                    }
                    else if(closest.tag.Equals("Player"))
                    {
                        string myNickName = getMyNickName(closest.name);
                        foreach (Player player in PhotonNetwork.PlayerList) //loop through each player and create a player listing
                        {
                            if (player.NickName == myNickName)
                            {

                                int myhp = (int)player.CustomProperties["health"];
                                int mymaxhp = (int)player.CustomProperties["maxhealth"];
                                if (myhp - damage >= 0) myhp -= damage;
                                else myhp = 0;

                                Hashtable setHP = new Hashtable();
                                setHP.Add("health", myhp);
                                player.SetCustomProperties(setHP, null);

                                if (!(bool)player.CustomProperties["dead"])
                                    GetComponent<PhotonView>().RPC("attackingPlayer", RpcTarget.All, closest.name, myhp, mymaxhp, player);
                                if (myhp == 0)
                                {
                                    Hashtable setDead = new Hashtable();
                                    setDead.Add("dead", true);
                                    player.SetCustomProperties(setDead, null);
                                }

                                int randomsound = Random.Range(1, 3);
                                AudioSource audio = GameObject.Find("Punch" + randomsound).GetComponent<AudioSource>();
                                GameObject.Find("Punch" + randomsound).transform.position = closest.transform.position;
                                audio.Play();
                                break;
                            }
                        }
                    }


                    


                }
            }
        }
        Inputs.attackingDPS = true;
    }

    public string getMyNickName(string gameObjName)
    {
        string[] myNickName1 = gameObjName.Split('(');
        myNickName1 = myNickName1[1].Split(')');
        string myNickName = myNickName1[0];
        return myNickName;
    }

    public void RealPickup()
    {
        if (Inputs.angkatbox)
        {

            float angle = Mathf.Round(transform.eulerAngles.y * 1000f) / 1000f;
            float tambahX = Mathf.Round(Mathf.Sin(angle * Mathf.Deg2Rad) * 100000) / 100000;
            float tambahZ = Mathf.Round(Mathf.Cos(angle * Mathf.Deg2Rad) * 100000) / 100000;
            GameObject.Find(Inputs.boxname).GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
            GameObject.Find(Inputs.boxname).GetComponent<Rigidbody>().angularVelocity = new Vector3(0, 0, 0);
            GameObject.Find(Inputs.boxname).transform.position = new Vector3(transform.position.x + tambahX, transform.position.y + 1f, transform.position.z + tambahZ);
            GameObject.Find(Inputs.boxname).transform.LookAt(new Vector3(this.transform.position.x, this.transform.position.y + 1f, this.transform.position.z));

            
        }
    }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(Inputs.RunX);
            stream.SendNext(Inputs.RunZ);
            stream.SendNext(Inputs.moving);
            stream.SendNext(Inputs.Jump);
            stream.SendNext(Inputs.Crack);
            stream.SendNext(Inputs.pmrPos.x);
            stream.SendNext(Inputs.pmrPos.y);
            stream.SendNext(Inputs.pmrPos.z);
            stream.SendNext(Inputs.pmrRot.x);
            stream.SendNext(Inputs.pmrRot.y);
            stream.SendNext(Inputs.pmrRot.z);
            //stream.SendNext(name);
            //stream.SendNext(transform.parent.name);
            stream.SendNext(Inputs.LookX);
            stream.SendNext(Inputs.LookZ);
            stream.SendNext(Inputs.Angkat);
            stream.SendNext(Inputs.boxname);
            stream.SendNext(Inputs.angkatbox);
            stream.SendNext(Inputs.JoystickX);
            stream.SendNext(Inputs.JoystickZ);
            stream.SendNext(Inputs.JoystickXAttack);
            stream.SendNext(Inputs.JoystickZAttack);
        }
        else
        {
            Inputs.RunX = (float)stream.ReceiveNext();
            Inputs.RunZ = (float)stream.ReceiveNext();
            Inputs.moving = (bool)stream.ReceiveNext();
            Inputs.Jump = (bool)stream.ReceiveNext();
            Inputs.Crack = (bool)stream.ReceiveNext();
            Inputs.pmrPos.x = (float)stream.ReceiveNext();
            Inputs.pmrPos.y = (float)stream.ReceiveNext();
            Inputs.pmrPos.z = (float)stream.ReceiveNext();
            Inputs.pmrRot.x = (float)stream.ReceiveNext();
            Inputs.pmrRot.y = (float)stream.ReceiveNext();
            Inputs.pmrRot.z = (float)stream.ReceiveNext();
            //name = (string)stream.ReceiveNext();
            //transform.parent = GameObject.Find((string)stream.ReceiveNext()).transform;
            Inputs.LookX = (float)stream.ReceiveNext();
            Inputs.LookZ = (float)stream.ReceiveNext();
            Inputs.Angkat = (bool)stream.ReceiveNext();
            Inputs.boxname = (string)stream.ReceiveNext();
            Inputs.angkatbox = (bool)stream.ReceiveNext();
            Inputs.JoystickX = (float)stream.ReceiveNext();
            Inputs.JoystickZ = (float)stream.ReceiveNext();
            Inputs.JoystickXAttack = (float)stream.ReceiveNext();
            Inputs.JoystickZAttack = (float)stream.ReceiveNext();
        }
    }

}

public class PlayerStats
{
    public PlayerStats(Player player, Vector3 posisi)
    {
        myplayer = player;
        Posisi = posisi;
    }

    public readonly Player myplayer;
    public Vector3 Posisi;
}