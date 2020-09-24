using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class LoadingScreen : MonoBehaviourPunCallbacks
{

    public Text loadAmountText;
    public Image progressBar;
    public Text teksnotif;
    public int completedPlayer;

    void Start()
    {
        if (PlayerPrefs.HasKey("ActScene"))
            PhotonNetwork.AutomaticallySyncScene = false;
        completedPlayer = 0;
        StartCoroutine("LoadLevelAsync");
    }

    void Update()
    {
        loadAmountText.text = ((int)(PhotonNetwork.LevelLoadingProgress * 100) + " %");
        progressBar.fillAmount = PhotonNetwork.LevelLoadingProgress;  
    }

    IEnumerator LoadLevelAsync()
    {
        if (PlayerPrefs.HasKey("ActScene")) PhotonNetwork.LoadLevel(PlayerPrefs.GetString("ActScene"));
        //if (PlayerPrefs.HasKey("ActScene")) PhotonNetwork.LoadLevel("Act3");
        else PhotonNetwork.LoadLevel(1);
        while (PhotonNetwork.LevelLoadingProgress < 1)
        {
            yield return new WaitForEndOfFrame();
        }

    }
}
