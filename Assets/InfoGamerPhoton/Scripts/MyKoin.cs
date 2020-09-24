using Photon.Pun;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MyKoin : MonoBehaviour
{
    public GameObject mykoinsemua;
    public GameObject mykointext;
    public GameObject mynickname;
    public Dropdown mydropdown;
    public GameObject notifpanel;
    public GameObject buttonserver;

    private static MyKoin instance = null;
    public static MyKoin Instance
    {
        get { return instance; }
    }
    void Awake()
    {
        mykoinsemua.SetActive(false);
        if (instance != null && instance != this) {
            Destroy(this.gameObject);
            return;
        } else
        {
            instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        StartCoroutine(GetCoin());
    }

    public IEnumerator GetCoin()
    {
        //string LoginUrl = "http://192.168.0.103/dimkaisy/login.php";
        string LoginUrl = MyConnection.Instance.LoginUrl + "dimkaisy/getcoin.php";
        WWWForm Form = new WWWForm();
        Form.AddField("username", PlayerPrefs.GetString("Username"));
        Form.AddField("password", PlayerPrefs.GetString("Password"));

        using (UnityWebRequest www = UnityWebRequest.Post(LoginUrl, Form))
        {
            www.certificateHandler = new AcceptAllCertificatesSignedWithASpecificKeyPublicKey();
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                mykointext.GetComponent<Text>().text = www.downloadHandler.text;
                mynickname.GetComponent<TextMeshProUGUI>().text = "Welcome, "+PlayerPrefs.GetString("Username");
            }
        }


        StopCoroutine(GetCoin());
    }

    public void OnChangeServer() // change team
    {
        if(mydropdown.value == 0)
        {
            notifpanel.SetActive(true);
            PhotonNetwork.Disconnect();
            PhotonNetwork.PhotonServerSettings.AppSettings.Server = "158.140.176.15";
            if (!PhotonNetwork.IsConnectedAndReady)
                PhotonNetwork.ConnectUsingSettings();
        }
        else if (mydropdown.value == 1)
        {
            notifpanel.SetActive(true);
            PhotonNetwork.Disconnect();
            PhotonNetwork.PhotonServerSettings.AppSettings.Server = "165.227.58.40";
            if (!PhotonNetwork.IsConnectedAndReady)
                PhotonNetwork.ConnectUsingSettings();
        }
    }

    public void buttonserverclickingame()
    {
        MyKoin.Instance.notifpanel.SetActive(true);
        MyKoin.Instance.notifpanel.transform.Find("BotNotif").Find("IsiNotif").GetComponent<Text>().text = "Can't change server while in-game";
        Invoke("tutupnotif", 3f);
    }

    public void tutupnotif()
    {
        MyKoin.Instance.notifpanel.SetActive(false);
        MyKoin.Instance.notifpanel.transform.Find("BotNotif").Find("IsiNotif").GetComponent<Text>().text = "Connecting to server...";

    }

}