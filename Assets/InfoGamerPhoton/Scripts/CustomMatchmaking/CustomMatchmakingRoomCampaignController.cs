using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class CustomMatchmakingRoomCampaignController : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private int multiPlayerSceneIndex; //scene index for loading multiplayer scene

    [SerializeField]
    private GameObject lobbyPanel; //display for when in lobby
    [SerializeField]
    private GameObject roomPanel; //display for when in room
    [SerializeField]
    private GameObject notifpanel;

    [SerializeField]
    private GameObject startButton; //only for the master client. used to start the game and load the multiplayer scene

    [SerializeField]
    private Transform playersContainer; //used to display all the players in the current room
    [SerializeField]
    private Transform chatplayersContainer; //used to display all the players in the current room
    [SerializeField]
    private GameObject playerListingPrefab; //Instantiate to display each player in the room
    [SerializeField]
    private GameObject chatplayerListingPrefab; //Instantiate to display each player in the room
    [SerializeField]
    private InputField chatfield; //display for the name of the room

    [SerializeField]
    private Text roomactdisplay; //display for the name of the room
    [SerializeField]
    private Text roomNameDisplay; //display for the name of the room
    [SerializeField]
    private Text numberPlayer; //display for the name of the room
    [SerializeField]
    private Dropdown dropdownTeam;
    [SerializeField]
    private int maxPlayerPerTeam;
    

    void ClearPlayerListings()
    {
        for (int i = playersContainer.childCount - 1; i >= 0; i--) //loop through all child object of the playersContainer, removing each child
        {
            Destroy(playersContainer.GetChild(i).gameObject);
        }
    }

    void ListPlayers() 
    {
        foreach (Player player in PhotonNetwork.PlayerList) //loop through each player and create a player listing
        {
            GameObject tempListing = Instantiate(playerListingPrefab, playersContainer);
            Text tempText = tempListing.transform.GetChild(0).GetComponent<Text>();
            tempText.text = player.NickName;

            Image tempImage = tempListing.transform.GetChild(1).GetComponent<Image>();
            if((string)player.CustomProperties["team"]=="red") tempImage.color = Color.red;
            if((string)player.CustomProperties["team"]=="blue") tempImage.color = Color.blue;
            if((string)player.CustomProperties["team"]=="yellow") tempImage.color = Color.yellow;
            if((string)player.CustomProperties["team"]=="green") tempImage.color = Color.green;
        }

        numberPlayer.text = "Player List ("+PhotonNetwork.CurrentRoom.PlayerCount+"/5)";
        
    }

    [PunRPC]
    public void getChat(string nickname, string chat, string team)
    {
        if(chatplayersContainer.childCount>15) Destroy(chatplayersContainer.GetChild(0).gameObject);
        GameObject tempListing = Instantiate(chatplayerListingPrefab, chatplayersContainer);
        Text tempText = tempListing.transform.GetChild(0).GetComponent<Text>();
        if(team=="red")tempText.color = Color.red;
        if(team=="blue")tempText.color = Color.blue;
        if (team == "yellow") tempText.color = Color.yellow;
        if(team=="green")tempText.color = Color.green;
        tempText.text = nickname+":";

        tempText = tempListing.transform.GetChild(1).GetComponent<Text>();
        if (nickname == "fandy")
            tempText.color = Color.magenta;
        tempText.text = chat;
    } 

    public void sendChat()
    {
        GetComponent<PhotonView>().RPC("getChat", RpcTarget.All, PhotonNetwork.LocalPlayer.NickName, chatfield.text, PhotonNetwork.LocalPlayer.CustomProperties["team"]);
        chatfield.text = "";
        chatfield.Select();
        chatfield.ActivateInputField();
    }

    public override void OnJoinedRoom()//called when the local player joins the room
    {
        notifpanel.SetActive(false);
        roomPanel.SetActive(true); //activate the display for being in a room
        lobbyPanel.SetActive(false); //hide the display for being in a lobby
        roomNameDisplay.text = PhotonNetwork.CurrentRoom.Name; //update room name display

        string namajudulact = "";
        if (PlayerPrefs.GetInt("ActNumber") == 1) namajudulact = "Newcomer";
        if (PlayerPrefs.GetInt("ActNumber") == 2) namajudulact = "My Bike";
        if (PlayerPrefs.GetInt("ActNumber") == 3) namajudulact = "Rescue Pet";
        if (PlayerPrefs.GetInt("ActNumber") == 4) namajudulact = "Dawn War";
        roomactdisplay.text = "Act "+PlayerPrefs.GetInt("ActNumber")+" - "+namajudulact;

        if (PhotonNetwork.IsMasterClient) //if master client then activate the start button
        {
            PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable() { { "campaign", PlayerPrefs.GetInt("ActNumber") } });
            startButton.SetActive(true);
            PhotonNetwork.SetPlayerCustomProperties(new Hashtable() { { "team", "red" } });
            PhotonNetwork.SetPlayerCustomProperties(new Hashtable() { { "loading", true } });
        }
        else
        {
            startButton.SetActive(false);
            dropdownTeam.gameObject.SetActive(false);
            int red = 0;
            int blue = 0;
            int yellow = 0;
            int green = 0;
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                if ((string)player.CustomProperties["team"] == "red") red++;
                if ((string)player.CustomProperties["team"] == "blue") blue++;
                if ((string)player.CustomProperties["team"] == "yellow") yellow++;
                if ((string)player.CustomProperties["team"] == "green") green++;
            }

            PhotonNetwork.SetPlayerCustomProperties(new Hashtable() { { "team", "red" } });
            PhotonNetwork.SetPlayerCustomProperties(new Hashtable() { { "loading", true } });

            Debug.Log(red+"-"+blue);

            if (red < maxPlayerPerTeam)
            {
                //PhotonNetwork.SetPlayerCustomProperties(new Hashtable() { { "team", "red" } });
                dropdownTeam.value = 0;
            }
            else
            {
                lobbyPanel.SetActive(true);
                roomPanel.SetActive(false);
                PhotonNetwork.LeaveRoom();
                PhotonNetwork.LeaveLobby();
                notifpanel.SetActive(true);
                notifpanel.transform.Find("BotNotif").transform.Find("IsiNotif").GetComponent<Text>().text = "Room is full !!";
                StartCoroutine(rejoinLobby());
            }
        }
        //photonPlayers = PhotonNetwork.PlayerList;
        ClearPlayerListings(); //remove all old player listings
        ListPlayers(); //relist all current player listings
    }

    public override void OnPlayerPropertiesUpdate(Player target, ExitGames.Client.Photon.Hashtable changedProps)
    {
        ClearPlayerListings(); //remove all old player listings
        ListPlayers(); //relist all current player listings
    }

    public override void OnPlayerEnteredRoom(Player newPlayer) //called whenever a new player enter the room
    {
        ClearPlayerListings(); //remove all old player listings
        ListPlayers(); //relist all current player listings
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)//called whenever a player leave the room
    {
        ClearPlayerListings();//remove all old player listings
        ListPlayers();//relist all current player listings
        if (PhotonNetwork.IsMasterClient)//if the local player is now the new master client then we activate the start button
        {
            startButton.SetActive(true);
        }
    }

    public void StartGameOnClick() //paired to the start button. will load all players into the multiplayer scene through the master client and AutomaticallySyncScene
    {
        if(PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false; //Comment out if you want player to join after the game has started
            PhotonNetwork.LoadLevel("LoadingScreen");   
        }
    }

    IEnumerator rejoinLobby()
    {
        yield return new WaitForSeconds(1);
        PhotonNetwork.JoinLobby();
    }

    public void BackOnClick() // paired to the back button in the room panel. will return the player to the lobby panel.
    {
        AudioSource audio = GameObject.Find("Clicked").GetComponent<AudioSource>();
        audio.Play();

        notifpanel.SetActive(true);
        lobbyPanel.SetActive(true);
        roomPanel.SetActive(false);
        PhotonNetwork.LeaveRoom();
        //PhotonNetwork.LeaveLobby();
        //StartCoroutine(rejoinLobby());
    }

    public void OnChangeTeam() // change team
    {
        int red = 0;
        int blue = 0;
        int yellow = 0;
        int green = 0;
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if ((string)player.CustomProperties["team"] == "red") red++;
            if ((string)player.CustomProperties["team"] == "blue") blue++;
            if ((string)player.CustomProperties["team"] == "yellow") yellow++;
            if ((string)player.CustomProperties["team"] == "green") green++;
        }

        if (red < maxPlayerPerTeam && dropdownTeam.value == 0)
        {
            PhotonNetwork.SetPlayerCustomProperties(new Hashtable() { {"team", "red" } });
            dropdownTeam.value = 0;
        }
        else if (blue < maxPlayerPerTeam && dropdownTeam.value == 1)
        {
            PhotonNetwork.SetPlayerCustomProperties(new Hashtable() { { "team", "blue" } });
            dropdownTeam.value = 1;
        }
        else if (yellow < maxPlayerPerTeam && dropdownTeam.value == 2)
        {
            PhotonNetwork.SetPlayerCustomProperties(new Hashtable() { { "team", "yellow" } });
            dropdownTeam.value = 2;
        }
        else if (green < maxPlayerPerTeam && dropdownTeam.value == 3)
        {
            PhotonNetwork.SetPlayerCustomProperties(new Hashtable() { { "team", "green" } });
            dropdownTeam.value = 3;
        }
        else
        {
            notifpanel.SetActive(true);
            notifpanel.transform.Find("BotNotif").transform.Find("IsiNotif").GetComponent<Text>().text = "Team is full !!";
            if((string)PhotonNetwork.LocalPlayer.CustomProperties["team"]=="red")dropdownTeam.value = 0;
            else if ((string)PhotonNetwork.LocalPlayer.CustomProperties["team"] == "blue") dropdownTeam.value = 1;
            else if ((string)PhotonNetwork.LocalPlayer.CustomProperties["team"] == "yellow") dropdownTeam.value = 2;
            else if ((string)PhotonNetwork.LocalPlayer.CustomProperties["team"] == "green") dropdownTeam.value = 3;
            /*lobbyPanel.SetActive(true);
            roomPanel.SetActive(false);
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.LeaveLobby();
            StartCoroutine(rejoinLobby());*/
        }
        ClearPlayerListings(); //remove all old player listings
        ListPlayers(); //relist all current player listings
    }

    

}
