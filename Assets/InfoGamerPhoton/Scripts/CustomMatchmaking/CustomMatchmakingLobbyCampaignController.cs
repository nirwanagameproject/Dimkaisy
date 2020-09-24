using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CustomMatchmakingLobbyCampaignController : MonoBehaviourPunCallbacks
{
    //[SerializeField]
    //private GameObject lobbyConnectButton; //button used for joining a Lobby.
    [SerializeField]
    private GameObject lobbyPanel; //panel for displaying lobby.
    [SerializeField]
    private GameObject notifpanel; //panel for displaying lobby.
    /*[SerializeField]
    private GameObject mainPanel; //panel for displaying the main menu
    [SerializeField]
    private GameObject loginPanel; //panel for displaying the main menu
    [SerializeField]
    private InputField playerNameInput; //Input field so player can change their NickName

    [SerializeField]
    private GameObject notifpanel; //panel for displaying the main menu
    [SerializeField]
    private GameObject loadingpanel; //panel for displaying the main menu
    */

    private string roomName; //string for saving room name
    private int roomSize; //int for saving room size

    private List<RoomInfo> roomListings; //list of current rooms
    [SerializeField]
    private Transform roomsContainer; //container for holding all the room listings
    [SerializeField]
    private GameObject roomListingPrefab; //prefab for displayer each room in the lobby

    public bool exitlobby;

    void Awake()
    {
        if(!PhotonNetwork.IsConnectedAndReady)
        notifpanel.SetActive(true);
    }

    void Start()
    {
        Debug.Log("MY USERNAME"+ PlayerPrefs.GetString("Username"));
        PhotonNetwork.NickName = PlayerPrefs.GetString("Username");
        PlayerPrefs.SetString("NickName", PlayerPrefs.GetString("Username"));
        roomName = "";
        if (PlayerPrefs.GetString("myserver") == "asia") MyKoin.Instance.buttonserver.transform.Find("Text").GetComponent<Text>().text = "Server Asia";
        else if (PlayerPrefs.GetString("myserver") == "us") MyKoin.Instance.buttonserver.transform.Find("Text").GetComponent<Text>().text = "Server US";
        MyKoin.Instance.buttonserver.SetActive(true);
        MyConnection.Instance.ingame = true;

        FirstConnect();
    }

    public void FirstConnect() //Callback function for when the first connection is established successfully.
    {
        PhotonNetwork.GameVersion = "v1.0";
        PhotonNetwork.AutomaticallySyncScene = true; //Makes it so whatever scene the master client has loaded is the scene all other clients will load
        //lobbyConnectButton.SetActive(true); //activate button for connecting to lobby
        //loadingpanel.SetActive(false);
        roomListings = new List<RoomInfo>(); //initializing roomListing
        roomName = "";
        roomSize = 5;
        ClearRoomListings();
        roomListings.Clear();

        notifpanel.SetActive(false);

        //check for player name saved to player prefs
        if (PlayerPrefs.HasKey("NickName"))
        {
            if (PlayerPrefs.GetString("NickName") == "")
            {
                PhotonNetwork.NickName = "Player " + UnityEngine.Random.Range(0, 1000); //random player name when not set
            }
            else
            {
                PhotonNetwork.NickName = PlayerPrefs.GetString("NickName"); //get saved player name
                PhotonNetwork.LocalPlayer.CustomProperties["character"] = PlayerPrefs.GetString("Character"); //get saved player name
            }
        }
        else
        {
            PhotonNetwork.NickName = "Player " + UnityEngine.Random.Range(0, 1000); //random player name when not set
        }
        StartCoroutine(CekKarakter());
        if (PhotonNetwork.IsConnectedAndReady)
            PhotonNetwork.JoinLobby(); //First tries to join a lobby
    }

    IEnumerator CekKarakter()
    {
        //string LoginUrl = "http://192.168.0.103/dimkaisy/login.php";
        string LoginUrl = MyConnection.Instance.LoginUrl + "dimkaisy/login.php";
        WWWForm Form = new WWWForm();
        Form.AddField("username", PlayerPrefs.GetString("Username"));
        Form.AddField("password", PlayerPrefs.GetString("Password"));
        Form.AddField("appversion", Application.version);

        UnityWebRequest www = UnityWebRequest.Post(LoginUrl, Form);
        www.certificateHandler = new AcceptAllCertificatesSignedWithASpecificKeyPublicKey();
        yield return www.SendWebRequest();
        MyPlayer myObject = JsonUtility.FromJson<MyPlayer>(www.downloadHandler.text);

        Debug.Log(www.downloadHandler.text);
        if (myObject.message == "success")
        {
            ExitGames.Client.Photon.Hashtable setChar = new ExitGames.Client.Photon.Hashtable();
            setChar.Add("character", myObject.character.ToString());
            PhotonNetwork.LocalPlayer.SetCustomProperties(setChar, null);
        }
        else if (myObject.message == "Game is outdated, need update first" || myObject.message == "Server is under maintenance" || myObject.message == "Password cann't empty")
        {
            ExitGames.Client.Photon.Hashtable setChar = new ExitGames.Client.Photon.Hashtable();
            setChar.Add("character", myObject.character.ToString());
            PhotonNetwork.LocalPlayer.SetCustomProperties(setChar, null);
        }
        StopCoroutine(CekKarakter());
    }



    void ClearRoomListings()
    {
        for (int i = roomsContainer.childCount - 1; i >= 0; i--) //loop through all child object of the playersContainer, removing each child
        {
            
            DestroyImmediate(roomsContainer.GetChild(i).gameObject);
        }
    }


    public override void OnRoomListUpdate(List<RoomInfo> roomList) //Once in lobby this function is called every time there is an update to the room list
    {
        int tempIndex;
        
        foreach (RoomInfo room in roomList) //loop through each room in room list
        {
            if (room.CustomProperties.ContainsKey("campaign"))
            {
                if ((int)room.CustomProperties["campaign"] == PlayerPrefs.GetInt("ActNumber"))
                {
                    if (roomListings != null) //try to find existing room listing
                    {
                        tempIndex = roomListings.FindIndex(ByName(room.Name));
                    }
                    else
                    {
                        tempIndex = -1;
                    }
                    if (tempIndex != -1) //remove listing because it has been closed
                    {
                        roomListings.RemoveAt(tempIndex);
                        DestroyImmediate(roomsContainer.GetChild(tempIndex).gameObject);
                        //Debug.Log("DESTROY ROOM");
                    }
                    if (room.PlayerCount > 0) //add room listing because it is new
                    {

                        roomListings.Add(room);
                        ListRoom(room);

                        // Debug.Log("CREATE ROOM");
                    }
                }
            }
                
            
            
        }
    }


    static System.Predicate<RoomInfo> ByName(string name) //predicate function for seach through room list
    {
        return delegate (RoomInfo room)
        {
            return room.Name == name;
        };
    }

    void ListRoom(RoomInfo room) //displays new room listing for the current room
    {
        if (room.IsOpen && room.IsVisible && (int)room.CustomProperties["campaign"] == PlayerPrefs.GetInt("ActNumber"))
        {
            GameObject tempListing = Instantiate(roomListingPrefab, roomsContainer);
            RoomButton tempButton = tempListing.GetComponent<RoomButton>();
            tempButton.SetRoom(room.Name, room.MaxPlayers, room.PlayerCount);
        }
    }

    public void RoomNameInputChanged(string nameIn) //input function for changing room name. paired to room name input field
    {
        roomName = nameIn;
    }
    public void OnRoomSizeInputChanged(string sizeIn) //input function for changing room size. paired to room size input field
    {
        roomSize = int.Parse(sizeIn);
    }

    public void CreateRoomOnClick() //function paired to the create room button
    {
        AudioSource audio = GameObject.Find("Clicked").GetComponent<AudioSource>();
        audio.Play();

        if (PhotonNetwork.NetworkClientState.ToString() != "JoiningLobby")
        {
            if (roomName == "") roomName = "Room " + PhotonNetwork.NickName;
            Debug.Log("Creating room now " + roomName + "-" + PhotonNetwork.NickName);


            //PhotonNetwork.CreateRoom(chosenLevel, true, true, 10, customPropertiesToSet, customPropertiesForLobby);

            RoomOptions roomOps = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = (byte)roomSize, BroadcastPropsChangeToAll = true };
            //TypedLobby typedLobby = new TypedLobby() { Name = "campaign", Type = LobbyType.Default };
            roomOps.CustomRoomPropertiesForLobby = new string[] { "campaign", "ai" };
            roomOps.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "campaign", PlayerPrefs.GetInt("ActNumber") } };
            PhotonNetwork.CreateRoom(roomName, roomOps); //attempting to create a new room

            if (PhotonNetwork.CurrentRoom == null)
            {
                notifpanel.SetActive(true);
                notifpanel.transform.Find("BotNotif").transform.Find("NotifTunggu").gameObject.SetActive(true);
                notifpanel.transform.Find("BotNotif").transform.Find("IsiNotif").gameObject.SetActive(false);
                notifpanel.transform.Find("BotNotif").transform.Find("Button").gameObject.SetActive(false);
            }
        }
        
    }

    public override void OnCreateRoomFailed(short returnCode, string message) //create room will fail if room already exists
    {
        Debug.Log("Tried to create a new room but failed, there must already be a room with the same name");
        notifpanel.SetActive(true);
        notifpanel.transform.Find("BotNotif").transform.Find("NotifTunggu").gameObject.SetActive(false);
        notifpanel.transform.Find("BotNotif").transform.Find("ButtonOK").gameObject.SetActive(false);
        notifpanel.transform.Find("BotNotif").transform.Find("IsiNotif").GetComponent<Text>().text = "Failed to create room. Please try with different name !!";
        notifpanel.transform.Find("BotNotif").transform.Find("IsiNotif").gameObject.SetActive(true);
        notifpanel.transform.Find("BotNotif").transform.Find("Button").gameObject.SetActive(true);
    }

    

    public void MatchmakingCancelOnClick() //Paired to the cancel button. Used to go back to the main menu
    {
        AudioSource audio = GameObject.Find("Clicked").GetComponent<AudioSource>();
        audio.Play();
        //lobbyPanel.SetActive(false);
        //PhotonNetwork.LeaveLobby();
        //PhotonNetwork.Disconnect();
        MyConnection.Instance.ingame = false;
        MyKoin.Instance.buttonserver.SetActive(false);
        SceneManager.LoadScene("Campaign");
    }

    public void NotifOnClickOK() //Paired to the cancel button. Used to go back to the main menu
    {
        AudioSource audio = GameObject.Find("Clicked").GetComponent<AudioSource>();
        audio.Play();

        notifpanel.transform.Find("BotNotif").transform.Find("NotifTunggu").gameObject.SetActive(true);
        notifpanel.transform.Find("BotNotif").transform.Find("IsiNotif").gameObject.SetActive(false);
        notifpanel.transform.Find("BotNotif").transform.Find("Button").gameObject.SetActive(false);
        notifpanel.SetActive(false);
    }

}