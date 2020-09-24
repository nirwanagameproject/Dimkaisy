using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GoogleLoginScript : MonoBehaviour
{
    public GameObject NotifLoading;
    public GameObject NotifIsi;
    public GameObject notifTaken;

    public InputField inputField;

    public MyNewPlayer myObject;

    // Start is called before the first frame update
    void Start()
    {
        myObject = new MyNewPlayer();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void buttonOk()
    {
        AudioSource audio = GameObject.Find("Clicked").GetComponent<AudioSource>();
        audio.Play();

        string mytoken = PlayerPrefs.GetString("Token");
        string myusername = inputField.text;
        StartCoroutine(RegisterAccount(myusername, mytoken));
        NotifLoading.SetActive(true);
    }

    public IEnumerator RegisterAccount(string username, string tokenid)
    {
        //string LoginUrl = "http://192.168.0.103/dimkaisy/register.php";
        string LoginUrl = MyConnection.Instance.LoginUrl +"dimkaisy/registergoogle.php";
        WWWForm Form = new WWWForm();
        Form.AddField("username", username);
        Form.AddField("token", tokenid);

        using (UnityWebRequest www = UnityWebRequest.Post(LoginUrl, Form))
        {
            www.certificateHandler = new AcceptAllCertificatesSignedWithASpecificKeyPublicKey();
            yield return www.SendWebRequest();
            myObject = JsonUtility.FromJson<MyNewPlayer>(www.downloadHandler.text);
            

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                notifTaken.transform.Find("IsiNotif").GetComponent<Text>().text = "No internet connection";
                notifTaken.SetActive(true);
            }
            else
            {
                Debug.Log("Form upload complete! " + www.downloadHandler.text);

                if (myObject.message == "success")
                {
                    PlayerPrefs.SetString("Username", myObject.username);
                    PlayerPrefs.SetString("Password", myObject.password);
                    PlayerPrefs.SetString("Character", "" + myObject.character);

                    ExitGames.Client.Photon.Hashtable setChar = new ExitGames.Client.Photon.Hashtable();
                    setChar.Add("character", myObject.character);
                    PhotonNetwork.LocalPlayer.SetCustomProperties(setChar, null);

                    SceneManager.LoadScene("UserMenu");

                }
                if(myObject.message == "fail")
                {
                    notifTaken.transform.Find("IsiNotif").GetComponent<Text>().text = "Username taken. Please pick another";
                    notifTaken.SetActive(true);
                    NotifLoading.SetActive(false);
                }
                else
                {
                    notifTaken.transform.Find("IsiNotif").GetComponent<Text>().text = myObject.message;
                    notifTaken.SetActive(true);
                    NotifLoading.SetActive(false);
                }
            }
        }


        StopCoroutine(RegisterAccount(username, tokenid));
    }

    public void UsernameTaken()
    {
        notifTaken.transform.Find("IsiNotif").GetComponent<Text>().text = "Username taken. Please pick another";
        notifTaken.SetActive(false);
    }
}