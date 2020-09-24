using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class LoadingScreenComplete : MonoBehaviourPunCallbacks
{

    public GameObject canvasloading;
    public int completedPlayer;

    int playercount;
    Player[] players;

    void Start()
    {
        completedPlayer = 0;

        Hashtable setRace = new Hashtable();
        setRace.Add("loading", false);
        PhotonNetwork.LocalPlayer.SetCustomProperties(setRace, null);

        playercount = PhotonNetwork.CurrentRoom.PlayerCount;
        players = PhotonNetwork.PlayerList;
    }

    [PunRPC]
    void destroyLoadingComplete()
    {
        canvasloading.SetActive(false);
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
            if (completedPlayer < playercount)
            {
                completedPlayer = 0;
                foreach (Player player in players) //loop through each player and create a player listing
                {
                    if (!(bool)player.CustomProperties["loading"])
                    {
                        completedPlayer++;
                    }
                }
                Debug.Log("PLayer sukkses " + completedPlayer);
            
                if (completedPlayer == playercount)
                {
                    completedPlayer++;
                    GetComponent<PhotonView>().RPC("destroyLoadingComplete", RpcTarget.All);
                }
                        
            }

    }

}
