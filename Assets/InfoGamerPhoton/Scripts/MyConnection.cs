using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MyConnection : MonoBehaviourPunCallbacks
{
    public string LoginUrl= "http://206.189.93.226/";
    //public static string LoginUrl= "http://192.168.0.103/";
    List<float> startTime;
    int pos;
    List<int> myping;
    public bool ingame;

    private static MyConnection instance = null;
    public static MyConnection Instance
    {
        get { return instance; }
    }
    void Awake()
    {
        //mykoinsemua.SetActive(false);
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        PlayerPrefs.DeleteKey("myserver");
        pos = 0;
        myping = new List<int>();
        if (PlayerPrefs.HasKey("myserver"))
        {
            pos = 2;
            if (PlayerPrefs.GetString("myserver") == "asia")
            {
                MyKoin.Instance.notifpanel.SetActive(true);
                PhotonNetwork.Disconnect();
                //PhotonNetwork.PhotonServerSettings.AppSettings.Server = "158.140.176.15";
                if (!PhotonNetwork.IsConnectedAndReady)
                    PhotonNetwork.ConnectUsingSettings();
            }
            else if (PlayerPrefs.GetString("myserver") == "us")
            {
                MyKoin.Instance.notifpanel.SetActive(true);
                MyKoin.Instance.mydropdown.value = 1;
            }
        }
        else
        {
            //PhotonNetwork.PhotonServerSettings.AppSettings.Server = "158.140.176.15";
            if (!PhotonNetwork.IsConnectedAndReady)
                PhotonNetwork.ConnectUsingSettings(); //Connects to Photon master servers
            MyKoin.Instance.notifpanel.SetActive(true);
        }

        
    }

    public override void OnConnectedToMaster()
    {
        MyKoin.Instance.notifpanel.SetActive(false);
        myping.Add(PhotonNetwork.GetPing());
        Debug.Log(PhotonNetwork.GetPing());
        Debug.Log(PhotonNetwork.PhotonServerSettings.AppSettings.Server);
        //if (PhotonNetwork.PhotonServerSettings.AppSettings.Server == "158.140.176.15") { PlayerPrefs.SetString("myserver", "asia"); MyConnection.Instance.LoginUrl = "https://digixkoin.com/"; }
        //else if (PhotonNetwork.PhotonServerSettings.AppSettings.Server == "165.227.58.40") {PlayerPrefs.SetString("myserver", "us"); MyConnection.Instance.LoginUrl = "https://165.227.58.40/"; }
        if (pos == 1)
        {
            if (myping[0] < myping[1])
            {
                MyKoin.Instance.notifpanel.SetActive(true);
                PhotonNetwork.Disconnect();
                //PhotonNetwork.PhotonServerSettings.AppSettings.Server = "158.140.176.15";
                if (!PhotonNetwork.IsConnectedAndReady)
                    PhotonNetwork.ConnectUsingSettings();
                pos++;
                
            }
            else
            {
                MyKoin.Instance.mydropdown.value = 1;
                pos++;
            }
            
        }

        if (pos == 0)
        {
            PhotonNetwork.Disconnect();
            //PhotonNetwork.PhotonServerSettings.AppSettings.Server = "165.227.58.40";
            if (!PhotonNetwork.IsConnectedAndReady)
                PhotonNetwork.ConnectUsingSettings();
            pos++;
        }

        if (pos == 2)
        {
            LoadingMenu.Instance.loadingawal();
            pos++;
        }

        Debug.Log("MY PREFS "+PlayerPrefs.GetString("myserver"));
        
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log(cause.ToString());
        if (cause.ToString().Equals("ClientTimeout") && !ingame)
        {
            MyKoin.Instance.notifpanel.transform.Find("BotNotif").Find("IsiNotif").GetComponent<Text>().text = "Server maintenance";
            Invoke("notifsetactive", 3f);
        }else if (cause.ToString().Equals("ClientTimeout") && ingame)
        {
            MyKoin.Instance.notifpanel.SetActive(true);
            MyKoin.Instance.notifpanel.transform.Find("BotNotif").Find("IsiNotif").GetComponent<Text>().text = "You are disconnected";
            Invoke("quitgame", 3f);
        }
    }

    public void notifsetactive()
    {
        
        if (MyKoin.Instance.mydropdown.value==1) MyKoin.Instance.mydropdown.value = 0;
        else if (MyKoin.Instance.mydropdown.value==0) MyKoin.Instance.mydropdown.value = 1;
        
        MyKoin.Instance.notifpanel.SetActive(false);
        MyKoin.Instance.notifpanel.transform.Find("BotNotif").Find("IsiNotif").GetComponent<Text>().text = "Connecting to server...";

    }

    public void quitgame()
    {

        Application.Quit();

    }

}