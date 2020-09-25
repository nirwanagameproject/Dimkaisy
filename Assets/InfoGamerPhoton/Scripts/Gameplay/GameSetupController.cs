using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSetupController : MonoBehaviourPunCallbacks
{
    /*
    [Header("UC Game Manager")]

    public Player1 PlayerPrefab;

    [HideInInspector]
    public Player1 LocalPlayer;*/

    [SerializeField]
    public string teamPlayer;
    [SerializeField]
    public static int jumlahBox;
    [SerializeField]
    GameObject go;
    [SerializeField]
    public GameObject go2;
    static public GameSetupController instance;

    [SerializeField]
    private GameObject chatplayerListingPrefab; //Instantiate to display each player in the room
    [SerializeField]
    private InputField chatfield; //display for the name of the room
    [SerializeField]
    private Transform chatplayersContainer; //used to display all the players in the current room

    public static bool startTimer = false;
    public static double timerIncrementValue;
    public static double startTime;
    [SerializeField] double timerRespawn = 10;
    public static ExitGames.Client.Photon.Hashtable CustomeValue;
    public static GameObject TimerRespawnOBJ;
    Collider[] SpawnBoxObj;

    [SerializeField]
    public int jumlahBase;
    public bool defeated;

    public GameObject loadingScreen;
    public GameObject CanvasloadingScreen;

    public GameObject WeaponList;
    public GameObject MenuSettings;
    public GameObject QuitNotif;

    public GameObject announcerText;
    public GameObject latencyImg;
    public GameObject latencyText;

    public GameObject SlotWeapon1;
    public GameObject SlotWeapon2;
    public GameObject SlotWeapon3;

    public AudioMixer mixer;

    public GameObject MenuObjective;
    public Text MenuObjectiveText;
    public int eliminaterobots;
    
    // This script will be added to any multiplayer scene
    void Start()
    {
        if (PlayerPrefs.GetInt("ActNumber") == 1 && PhotonNetwork.IsMasterClient)
        {
            ExitGames.Client.Photon.Hashtable setEliminateRobots = new ExitGames.Client.Photon.Hashtable();
            setEliminateRobots.Add("eliminaterobots", 0);
            PhotonNetwork.CurrentRoom.SetCustomProperties(setEliminateRobots, null);
        }else if (PlayerPrefs.GetInt("ActNumber") == 2)
        {
            ExitGames.Client.Photon.Hashtable getbike = new ExitGames.Client.Photon.Hashtable();
            getbike.Add("getbike", 0);
            PhotonNetwork.CurrentRoom.SetCustomProperties(getbike, null);
        }

        ExitGames.Client.Photon.Hashtable setLatency = new ExitGames.Client.Photon.Hashtable();
        setLatency.Add("latency", PhotonNetwork.GetPing());
        PhotonNetwork.LocalPlayer.SetCustomProperties(setLatency, null);

        MyKoin.Instance.mykoinsemua.SetActive(false);
        if(MainMenuController.Instance!=null)
        MainMenuController.Instance.gameObject.SetActive(false);
        MyMusic.Instance.GetComponent<AudioSource>().Stop();
        CanvasloadingScreen.SetActive(true);
        loadingScreen.SetActive(true);
        if (PlayerPrefs.HasKey("Sound"))
            GameObject.Find("Canvas").transform.Find("MenuSettingsPanel").Find("BotNotifUser").Find("SliderSound").GetComponent<Slider>().value = PlayerPrefs.GetFloat("Sound");
        if (PlayerPrefs.HasKey("Music"))
            GameObject.Find("Canvas").transform.Find("MenuSettingsPanel").Find("BotNotifUser").Find("SliderMusic").GetComponent<Slider>().value = PlayerPrefs.GetFloat("Music");
        if(PlayerPrefs.HasKey("GraphicQuality"))
            GameObject.Find("Canvas").transform.Find("MenuSettingsPanel").Find("BotNotifUser").Find("DropdownQualityGraphic").GetComponent<Dropdown>().value = PlayerPrefs.GetInt("GraphicQuality");
        defeated = false;
        jumlahBox = 0;
        instance = this;
        CreatePlayer(); //Create a networked player object for each player that loads into the multiplayer scenes.
        //StartCoroutine(CekKarakter());

        InvokeRepeating("CheckLatency",0f,5f);
    }

    void Update()
    {
        if (!startTimer) return;
        timerIncrementValue = PhotonNetwork.Time - startTime;
        double timerRSPWN = timerRespawn - timerIncrementValue;
        GameObject.Find("Canvas").transform.Find("RespawnText").GetComponent<TextMeshProUGUI>().text = "You will respawn in " + (int)timerRSPWN;
        if (timerIncrementValue >= timerRespawn)
        {
            TimerRespawnOBJ.SetActive(false);
            startTimer = false;
        }
        
    }

    void FixedUpdate()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            SpawnBoxObj = Physics.OverlapSphere(GameObject.Find("BoxSpawn").transform.position, 10, LayerMask.GetMask("Box"));
            if (jumlahBox > 8 && SpawnBoxObj.Length<10)
            {
                RespawnBox();
            }
        }
    }

    

    public void CheckLatency()
    {
        int mylatency = PhotonNetwork.GetPing();
        latencyText.GetComponent<TextMeshProUGUI>().text = mylatency + " MS";
        if(mylatency<=60)latencyImg.GetComponent<Image>().overrideSprite = Resources.Load<Sprite>("[UIFabrica]TrollNest_Free_v01/02.PNG/latency" + 4);
        else if (mylatency <= 100) latencyImg.GetComponent<Image>().overrideSprite = Resources.Load<Sprite>("[UIFabrica]TrollNest_Free_v01/02.PNG/latency" + 3);
        else if (mylatency <= 125) latencyImg.GetComponent<Image>().overrideSprite = Resources.Load<Sprite>("[UIFabrica]TrollNest_Free_v01/02.PNG/latency" + 2);
        else if (mylatency > 125) latencyImg.GetComponent<Image>().overrideSprite = Resources.Load<Sprite>("[UIFabrica]TrollNest_Free_v01/02.PNG/latency" + 1);

        ExitGames.Client.Photon.Hashtable setLatency = new ExitGames.Client.Photon.Hashtable();
        setLatency.Add("latency", mylatency);
        PhotonNetwork.LocalPlayer.SetCustomProperties(setLatency, null);

        Player playerPingBagus;
        playerPingBagus = PhotonNetwork.LocalPlayer;
        foreach (Player player in PhotonNetwork.PlayerList) //loop through each player and create a player listing
        {
            if(player.CustomProperties.ContainsKey("latency"))
            if((int)playerPingBagus.CustomProperties["latency"]>(int)player.CustomProperties["latency"])
            {
                playerPingBagus = player;
            }
        }

        if(!playerPingBagus.IsMasterClient)
        PhotonNetwork.SetMasterClient(playerPingBagus);

    }


    public IEnumerator ReDead()
    {
        Debug.Log("Respawn");
        yield return new WaitForSeconds(8.7f);
        
        go2.GetComponent<Controller>().enabled = true;
        string myTeam = (string)PhotonNetwork.LocalPlayer.CustomProperties["team"];
        Vector3 craftPos = Vector3.one;
        if (myTeam == "red") craftPos = GameObject.Find("BoxCraftRed#0").transform.position;
        else if (myTeam == "blue") craftPos = GameObject.Find("BoxCraftBlue#0").transform.position;
        else if (myTeam == "yellow") craftPos = GameObject.Find("BoxCraftYellow#0").transform.position;
        else if (myTeam == "green") craftPos = GameObject.Find("BoxCraftGreen#0").transform.position;
        go2.transform.position = new Vector3(craftPos.x + Random.Range(-4, 4), craftPos.y + 1f, craftPos.z + Random.Range(-4, 4));
        go2.transform.eulerAngles = new Vector3(0,0,0);
        go2.GetComponent<Rigidbody>().isKinematic = true;
        go2.GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<PhotonView>().RPC("respawnPlayer", RpcTarget.All, go2.name);
        go2.GetComponent<PhotonView>().RPC("soundpunch2", RpcTarget.All, go2.name);

        ExitGames.Client.Photon.Hashtable setDead = new ExitGames.Client.Photon.Hashtable();
        setDead.Add("health", (int)PhotonNetwork.LocalPlayer.CustomProperties["maxhealth"]);
        setDead.Add("dead", false);
        PhotonNetwork.LocalPlayer.SetCustomProperties(setDead, null);
        go2.GetComponent<PhotonView>().RPC("getHearts", RpcTarget.All, go2.name, (int)PhotonNetwork.LocalPlayer.CustomProperties["health"], (int)PhotonNetwork.LocalPlayer.CustomProperties["maxhealth"]);
        go2.GetComponent<PhotonView>().RPC("refreshBar", PhotonNetwork.LocalPlayer, 1f, (int)PhotonNetwork.LocalPlayer.CustomProperties["maxhealth"]);
        StopCoroutine(ReDead());
    }

    [PunRPC]
    void respawnPlayer(string namaObjPlayer)
    {
        GameObject gameObjects = FindInActiveObjectByName(namaObjPlayer);
        
        gameObjects.SetActive(true);
    }

    [PunRPC]
    void baseHancur(string team)
    {
        if ((string)PhotonNetwork.LocalPlayer.CustomProperties["team"] == team && !defeated)
        {
            defeated = true;
            if(PlayerPrefs.GetInt("ActNumber")==1 || PlayerPrefs.GetInt("ActNumber") == 2) GameObject.Find("Canvas").transform.Find("GameOver").Find("Win").GetComponent<Image>().overrideSprite = Resources.Load<Sprite>("[UIFabrica]TrollNest_Free_v01/02.PNG/defeated5");
            else
            GameObject.Find("Canvas").transform.Find("GameOver").Find("Win").GetComponent<Image>().overrideSprite = Resources.Load<Sprite>("[UIFabrica]TrollNest_Free_v01/02.PNG/defeated" + GameSetupController.instance.jumlahBase);
            AudioSource underAttack = GameObject.Find("GameOverAudio").GetComponent<AudioSource>();
            GameObject.Find("GameOverAudio").transform.position = transform.position;
            underAttack.Play();
            FindInActiveObjectByName("GameOver").SetActive(true);
        }
        jumlahBase--;
        if (jumlahBase == 1 && GameObject.Find("Base_"+(string)PhotonNetwork.LocalPlayer.CustomProperties["team"])!=null)
        {
            GameObject.Find("Canvas").transform.Find("GameOver").Find("Win").GetComponent<Image>().overrideSprite = Resources.Load<Sprite>("[UIFabrica]TrollNest_Free_v01/02.PNG/defeated" + GameSetupController.instance.jumlahBase);
            AudioSource underAttack = GameObject.Find("GameOverWinAudio").GetComponent<AudioSource>();
            GameObject.Find("GameOverWinAudio").transform.position = transform.position;
            underAttack.Play();
            FindInActiveObjectByName("GameOver").SetActive(true);
        }
        
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

    private void CreatePlayer()
    {
        Debug.Log("Creating Player "+PhotonNetwork.NickName);
        InvokeRepeating("CreateBox", 2f, 2f);

        string myTeam = (string)PhotonNetwork.LocalPlayer.CustomProperties["team"];
        Vector3 craftPos = Vector3.one;
        if (myTeam == "red") craftPos = GameObject.Find("BoxCraftRed#0").transform.position;
        else if (myTeam == "blue") craftPos = GameObject.Find("BoxCraftBlue#0").transform.position;
        else if (myTeam == "yellow") craftPos = GameObject.Find("BoxCraftYellow#0").transform.position;
        else if (myTeam == "green") craftPos = GameObject.Find("BoxCraftGreen#0").transform.position;
        Vector3 RespawnPos = new Vector3(craftPos.x + Random.Range(-4, 4), craftPos.y + 1f, craftPos.z + Random.Range(-4, 4));

        if ((string)PhotonNetwork.LocalPlayer.CustomProperties["character"] == "1")
        {
            go = PhotonNetwork.Instantiate(Path.Combine("Models/Patrick", "PlayerUnit"), RespawnPos, Quaternion.identity);
            go.name = "Player ("+PhotonNetwork.NickName+")";
            go.transform.parent = GameObject.Find("PlayerSpawn").transform;

            go2 = PhotonNetwork.Instantiate(Path.Combine("Models/Orang", "Player"), RespawnPos, Quaternion.identity);
            go2.name = "Player";
            go2.transform.parent = go.transform;

        }
        else if ((string)PhotonNetwork.LocalPlayer.CustomProperties["character"] == "2")
        {
            go = PhotonNetwork.Instantiate(Path.Combine("Models/Spongebob", "PlayerUnit2"), RespawnPos, Quaternion.identity);
            go.name = "Player (" + PhotonNetwork.NickName + ")";
            go.transform.parent = GameObject.Find("PlayerSpawn").transform;

            go2 = PhotonNetwork.Instantiate(Path.Combine("Models/Spongebob", "Player"), RespawnPos, Quaternion.identity);
            go2.name = "Player";
            go2.transform.parent = go.transform;

        }
        else if ((string)PhotonNetwork.LocalPlayer.CustomProperties["character"] == "3")
        {
            go = PhotonNetwork.Instantiate(Path.Combine("Models/Squidward", "PlayerUnit3"), RespawnPos, Quaternion.identity);
            go.name = "Player (" + PhotonNetwork.NickName + ")";
            go.transform.parent = GameObject.Find("PlayerSpawn").transform;

            go2 = PhotonNetwork.Instantiate(Path.Combine("Models/Squidward", "Player"), RespawnPos, Quaternion.identity);
            go2.name = "Player";
            go2.transform.parent = go.transform;

        }
        else if ((string)PhotonNetwork.LocalPlayer.CustomProperties["character"] == "4")
        {
            go = PhotonNetwork.Instantiate(Path.Combine("Models/Sandy", "PlayerUnit"), RespawnPos, Quaternion.identity);
            go.name = "Player (" + PhotonNetwork.NickName + ")";
            go.transform.parent = GameObject.Find("PlayerSpawn").transform;

            go2 = PhotonNetwork.Instantiate(Path.Combine("Models/Sandy", "Player"), RespawnPos, Quaternion.identity);
            go2.name = "Player";
            go2.transform.parent = go.transform;

        }
        else if ((string)PhotonNetwork.LocalPlayer.CustomProperties["character"] == "5")
        {
            go = PhotonNetwork.Instantiate(Path.Combine("Models/Mr Krabs", "PlayerUnit"), RespawnPos, Quaternion.identity);
            go.name = "Player (" + PhotonNetwork.NickName + ")";
            go.transform.parent = GameObject.Find("PlayerSpawn").transform;

            go2 = PhotonNetwork.Instantiate(Path.Combine("Models/Mr Krabs", "Player"), RespawnPos, Quaternion.identity);
            go2.name = "Player";
            go2.transform.parent = go.transform;

        }
        else if ((string)PhotonNetwork.LocalPlayer.CustomProperties["character"] == "6")
        {
            go = PhotonNetwork.Instantiate(Path.Combine("Models/Orang", "PlayerUnit"), RespawnPos, Quaternion.identity);
            go.name = "Player (" + PhotonNetwork.NickName + ")";
            go.transform.parent = GameObject.Find("PlayerSpawn").transform;

            go2 = PhotonNetwork.Instantiate(Path.Combine("Models/Orang", "Player"), RespawnPos, Quaternion.identity);
            go2.name = "Player";
            go2.transform.parent = go.transform;

        }
        else if ((string)PhotonNetwork.LocalPlayer.CustomProperties["character"] == "7")
        {
            go = PhotonNetwork.Instantiate(Path.Combine("Models/Larry", "PlayerUnit"), RespawnPos, Quaternion.identity);
            go.name = "Player (" + PhotonNetwork.NickName + ")";
            go.transform.parent = GameObject.Find("PlayerSpawn").transform;

            go2 = PhotonNetwork.Instantiate(Path.Combine("Models/Larry", "Player"), RespawnPos, Quaternion.identity);
            go2.name = "Player";
            go2.transform.parent = go.transform;

        }
        else if ((string)PhotonNetwork.LocalPlayer.CustomProperties["character"] == "8")
        {
            go = PhotonNetwork.Instantiate(Path.Combine("Models/Mrs Puff", "PlayerUnit"), RespawnPos, Quaternion.identity);
            go.name = "Player (" + PhotonNetwork.NickName + ")";
            go.transform.parent = GameObject.Find("PlayerSpawn").transform;

            go2 = PhotonNetwork.Instantiate(Path.Combine("Models/Mrs Puff", "Player"), RespawnPos, Quaternion.identity);
            go2.name = "Player";
            go2.transform.parent = go.transform;

        }
        //Player1.RefreshInstance(ref LocalPlayer, PlayerPrefab);
    }

    private void CreateBox()
    {
        if(jumlahBox>8) CancelInvoke();
        RespawnBox();
        jumlahBox++;
    }

    public static void RespawnBox()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            int randombox = Random.Range(1, 11);
            string warnabox = "Red";
            if (randombox > 7) warnabox = "Blue";
            Vector3 myBoxSpawnPos = GameObject.Find("BoxSpawn").transform.position;
            GameObject go = PhotonNetwork.InstantiateSceneObject(Path.Combine("Models", "Cube" + warnabox), new Vector3(myBoxSpawnPos.x+Random.Range(-5, 5), 10, myBoxSpawnPos.z + Random.Range(-5, 5)), Quaternion.identity);
            go.name = "Cube" + warnabox + " (" + go.GetPhotonView().ViewID + ")";
            go.transform.parent = GameObject.Find("BoxSpawn").transform;
            go.GetComponent<PhotonView>().RPC("boxrpc", RpcTarget.All, go.name, "BoxSpawn");
            go.GetComponent<PhotonView>().RPC("myjumlahbox", RpcTarget.All, jumlahBox);
        }
    }

    public void clickCloseMenuSettings()
    {
        MenuSettings.SetActive(false);
    }

    public void clickMenuObjective()
    {
        MenuObjective.SetActive(true);
    }

    public void clickCloseMenuObjective()
    {
        MenuObjective.SetActive(false);
    }

    public void clickMenuSettings()
    {
        MenuSettings.SetActive(true);
    }

    public void clickNoQuitNotif()
    {
        QuitNotif.SetActive(false);
    }

    public void clickQuitGame()
    {
        QuitNotif.SetActive(true);
    }

    public void rageQuit()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LeaveLobby();
        SceneManager.LoadScene("Endgame");
    }

    public void SetLevel()
    {
        float sliderValue = GameObject.Find("Canvas").transform.Find("MenuSettingsPanel").Find("BotNotifUser").Find("SliderSound").GetComponent<Slider>().value;
        mixer.SetFloat("SoundVol", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("Sound", sliderValue);
    }

    public void SetONOFFLevel()
    {
        if (GameObject.Find("Canvas").transform.Find("MenuSettingsPanel").Find("BotNotifUser").Find("SliderSound").GetComponent<Slider>().value > 0.01f)
        {
            PlayerPrefs.SetFloat("SoundTemp", PlayerPrefs.GetFloat("Sound"));
            GameObject.Find("Canvas").transform.Find("MenuSettingsPanel").Find("BotNotifUser").Find("SliderSound").GetComponent<Slider>().value = 0;
        }
        else
        {
            GameObject.Find("Canvas").transform.Find("MenuSettingsPanel").Find("BotNotifUser").Find("SliderSound").GetComponent<Slider>().value = PlayerPrefs.GetFloat("SoundTemp");
        }
    }

    public void SetLevelMusic()
    {
        float sliderValue = GameObject.Find("Canvas").transform.Find("MenuSettingsPanel").Find("BotNotifUser").Find("SliderMusic").GetComponent<Slider>().value;
        mixer.SetFloat("MusicVol", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("Music", sliderValue);
    }

    public void SetONOFFMusic()
    {
        if (GameObject.Find("Canvas").transform.Find("MenuSettingsPanel").Find("BotNotifUser").Find("SliderMusic").GetComponent<Slider>().value > 0.01f)
        {
            PlayerPrefs.SetFloat("MusicTemp", PlayerPrefs.GetFloat("Music"));
            GameObject.Find("Canvas").transform.Find("MenuSettingsPanel").Find("BotNotifUser").Find("SliderMusic").GetComponent<Slider>().value = 0;
        }
        else
        {
            GameObject.Find("Canvas").transform.Find("MenuSettingsPanel").Find("BotNotifUser").Find("SliderMusic").GetComponent<Slider>().value = PlayerPrefs.GetFloat("MusicTemp");
        }
    }

    public void OnChangeQuality() // change team
    {
        int dropdownval = GameObject.Find("Canvas").transform.Find("MenuSettingsPanel").Find("BotNotifUser").Find("DropdownQualityGraphic").GetComponent<Dropdown>().value;
        dropdownval++;

        QualitySettings.SetQualityLevel(dropdownval, true);
        dropdownval--;
        PlayerPrefs.SetInt("GraphicQuality", dropdownval);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)//called whenever a player leave the room
    {
        announcerText.SetActive(true);
        announcerText.GetComponent<TextMeshProUGUI>().text =  otherPlayer.NickName + " has left the game";
        StartCoroutine(DisableAnnouncer());
    }

    public IEnumerator DisableAnnouncer()
    {
        yield return new WaitForSeconds(3);
        announcerText.SetActive(false);
    }

    public void setWeapon(int slot)
    {
        if (!(bool)PhotonNetwork.LocalPlayer.CustomProperties["dead"])
        {
            if (slot == 1)
            {
                SlotWeapon1.GetComponent<Image>().overrideSprite = Resources.Load<Sprite>("[UIFabrica]TrollNest_Free_v01/02.PNG/kotakweaponclicked");
                SlotWeapon2.GetComponent<Image>().overrideSprite = Resources.Load<Sprite>("[UIFabrica]TrollNest_Free_v01/02.PNG/kotakweapon");
                SlotWeapon3.GetComponent<Image>().overrideSprite = Resources.Load<Sprite>("[UIFabrica]TrollNest_Free_v01/02.PNG/kotakweapon");


            }
            else
        if (slot == 2)
            {
                SlotWeapon1.GetComponent<Image>().overrideSprite = Resources.Load<Sprite>("[UIFabrica]TrollNest_Free_v01/02.PNG/kotakweapon");
                SlotWeapon2.GetComponent<Image>().overrideSprite = Resources.Load<Sprite>("[UIFabrica]TrollNest_Free_v01/02.PNG/kotakweaponclicked");
                SlotWeapon3.GetComponent<Image>().overrideSprite = Resources.Load<Sprite>("[UIFabrica]TrollNest_Free_v01/02.PNG/kotakweapon");
            }
            else
        if (slot == 3)
            {
                SlotWeapon1.GetComponent<Image>().overrideSprite = Resources.Load<Sprite>("[UIFabrica]TrollNest_Free_v01/02.PNG/kotakweapon");
                SlotWeapon2.GetComponent<Image>().overrideSprite = Resources.Load<Sprite>("[UIFabrica]TrollNest_Free_v01/02.PNG/kotakweapon");
                SlotWeapon3.GetComponent<Image>().overrideSprite = Resources.Load<Sprite>("[UIFabrica]TrollNest_Free_v01/02.PNG/kotakweaponclicked");
            }

            ExitGames.Client.Photon.Hashtable setWpn = new ExitGames.Client.Photon.Hashtable();
            setWpn.Add("weaponslotnumber", slot);
            setWpn.Add("weapon", (string)PhotonNetwork.LocalPlayer.CustomProperties["weaponslot" + slot]);
            setWpn.Add("weapontipe", (string)PhotonNetwork.LocalPlayer.CustomProperties["weaponslot" + slot + "weapontipe"]);
            setWpn.Add("aspd", (float)PhotonNetwork.LocalPlayer.CustomProperties["weaponslot" + slot + "aspd"]);
            setWpn.Add("damage", (int)PhotonNetwork.LocalPlayer.CustomProperties["weaponslot" + slot + "damage"]);
            setWpn.Add("needammo", (int)PhotonNetwork.LocalPlayer.CustomProperties["weaponslot" + slot + "needammo"]);
            PhotonNetwork.LocalPlayer.SetCustomProperties(setWpn, null);

            string weapon = (string)PhotonNetwork.LocalPlayer.CustomProperties["weapon"];

            go.GetComponent<PhotonView>().RPC("getWeaponAnimate", RpcTarget.All, go.name, weapon, PhotonNetwork.NickName, "", "");
        }
        
    }

    public void bukaWeaponList()
    {
        WeaponList.SetActive(true);
    }
    public void tutupWeaponList()
    {
        WeaponList.SetActive(false);
    }

    [PunRPC]
    public void getChat(string nickname, string chat, string team)
    {
        if (chatplayersContainer.childCount > 9) Destroy(chatplayersContainer.GetChild(0).gameObject);
        GameObject tempListing = Instantiate(chatplayerListingPrefab, chatplayersContainer);
        Text tempText = tempListing.transform.GetChild(0).GetComponent<Text>();
        if (team == "red") tempText.color = Color.red;
        if (team == "blue") tempText.color = Color.blue;
        if (team == "yellow") tempText.color = Color.yellow;
        if (team == "green") tempText.color = Color.green;
        if (nickname == "fandy")
            tempText.color = Color.magenta;
        tempText.text = nickname + ":";

        tempText = tempListing.transform.GetChild(1).GetComponent<Text>();
        tempText.text = chat;
    }

    public void sendChat()
    {
        GetComponent<PhotonView>().RPC("getChat", RpcTarget.All, PhotonNetwork.LocalPlayer.NickName, chatfield.text, PhotonNetwork.LocalPlayer.CustomProperties["team"]);
        chatfield.text = "";
        chatfield.Select();
        chatfield.ActivateInputField();
    }

    [PunRPC]
    public void updateObjective()
    {
        MenuObjectiveText.text = "- Find 4 bikes and bring it to park bike line near Monas (" + (int)PhotonNetwork.CurrentRoom.CustomProperties["getbike"] + "/4)\n- Protect The Monas";
    }

    [PunRPC]
    public void objectiveAct1part1()
    {
        eliminaterobots++;
        if (PhotonNetwork.IsMasterClient)
        {
            ExitGames.Client.Photon.Hashtable setEliminateRobots = new ExitGames.Client.Photon.Hashtable();
            setEliminateRobots.Add("eliminaterobots", (int)PhotonNetwork.CurrentRoom.CustomProperties["eliminaterobots"] + 1);
            PhotonNetwork.CurrentRoom.SetCustomProperties(setEliminateRobots, null);
        }

        MenuObjectiveText.text = "- Eliminate all robots ("+ (int)PhotonNetwork.CurrentRoom.CustomProperties["eliminaterobots"] + "/10)";
        if ((int)PhotonNetwork.CurrentRoom.CustomProperties["eliminaterobots"] == 10)
        {
            GameObject.Find("Canvas").transform.Find("GameOver").Find("Win").GetComponent<Image>().overrideSprite = Resources.Load<Sprite>("[UIFabrica]TrollNest_Free_v01/02.PNG/defeated1");
            AudioSource underAttack = GameObject.Find("GameOverWinAudio").GetComponent<AudioSource>();
            GameObject.Find("GameOverWinAudio").transform.position = transform.position;
            underAttack.Play();
            FindInActiveObjectByName("GameOver").SetActive(true);

            PlayerPrefs.SetString("Win", "ActNumber1");

            if(PhotonNetwork.IsMasterClient)
            foreach (Player player in PhotonNetwork.PlayerList) //loop through each player and create a player listing
            {
                StartCoroutine(dapetRewards(player.NickName, PlayerPrefs.GetInt("ActNumber")));
            }

        }
    }

    [PunRPC]
    public void winact2()
    {
        GameObject.Find("Canvas").transform.Find("GameOver").Find("Win").GetComponent<Image>().overrideSprite = Resources.Load<Sprite>("[UIFabrica]TrollNest_Free_v01/02.PNG/defeated1");
        AudioSource underAttack = GameObject.Find("GameOverWinAudio").GetComponent<AudioSource>();
        GameObject.Find("GameOverWinAudio").transform.position = transform.position;
        underAttack.Play();
        FindInActiveObjectByName("GameOver").SetActive(true);

        PlayerPrefs.SetString("Win", "ActNumber2");

        if (PhotonNetwork.IsMasterClient)
            foreach (Player player in PhotonNetwork.PlayerList) //loop through each player and create a player listing
            {
                StartCoroutine(dapetRewards(player.NickName, PlayerPrefs.GetInt("ActNumber")));
            }
    }

    IEnumerator dapetRewards(string nickname, int act)
    {
        //string LoginUrl = "http://192.168.0.103/dimkaisy/login.php";
        string LoginUrl = MyConnection.Instance.LoginUrl + "dimkaisy/winact.php";
        WWWForm Form = new WWWForm();
        Form.AddField("username", nickname);
        Form.AddField("password", PlayerPrefs.GetString("Password"));
        Form.AddField("act", act);

        using (UnityWebRequest www = UnityWebRequest.Post(LoginUrl, Form))
        {
            //www.certificateHandler = new AcceptAllCertificatesSignedWithASpecificKeyPublicKey();
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {

                if (www.downloadHandler.text == "success")
                {
                    Debug.Log("sukses rewards");

                }else Debug.Log(www.downloadHandler.text);
            }
        }


        StopCoroutine(dapetRewards(nickname, act));
        
    }

}