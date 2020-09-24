using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Security.Cryptography.X509Certificates;

public class LoadingMenu : MonoBehaviour
{
    private string username;
    private string password;
    private string maintenance;

    public GameObject notif;

    public AudioMixer mixer;

    private static LoadingMenu instance = null;
    public static LoadingMenu Instance
    {
        get { return instance; }
    }
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    public void loadingawal()
    {
        maintenance = "no";
        if (PlayerPrefs.HasKey("Music"))
            mixer.SetFloat("MusicVol", Mathf.Log10(PlayerPrefs.GetFloat("Music")) * 20);
        PlayerPrefs.DeleteKey("ActScene");
        PlayerPrefs.DeleteKey("ActNumber");

        if (!PlayerPrefs.HasKey("Character"))
        {
            PlayerPrefs.SetString("Character", "1");
        }
        if (PlayerPrefs.HasKey("Username") && PlayerPrefs.HasKey("Password"))
        {
            if (PlayerPrefs.GetString("Username") != "")
            {
                username = PlayerPrefs.GetString("Username");
                password = PlayerPrefs.GetString("Password");

                StartCoroutine("LoginAccount");
            }
        }
        else
        {
            username = "cek";
            password = "cek";
            StartCoroutine("LoginAccount");
            //SceneManager.LoadScene("MainMenu");
        }
    }

    IEnumerator LoginAccount()
    {
        //string LoginUrl = "http://192.168.0.103/dimkaisy/login.php";
        string LoginUrl = MyConnection.Instance.LoginUrl +"dimkaisy/login.php";
        WWWForm Form = new WWWForm();
        Form.AddField("username", username);
        Form.AddField("password", password);
        Form.AddField("appversion", Application.version);
        Debug.Log(LoginUrl);
        using (UnityWebRequest www = UnityWebRequest.Post(LoginUrl, Form))
        {
            www.certificateHandler = new AcceptAllCertificatesSignedWithASpecificKeyPublicKey();
            yield return www.SendWebRequest();

            MyPlayer myObject = JsonUtility.FromJson<MyPlayer>(www.downloadHandler.text);
            if (www.isNetworkError)
            {
                notif.transform.Find("BotNotif").Find("IsiNotif").GetComponent<Text>().text = "No internet connection";
                notif.SetActive(true);
                maintenance = "yes";
                Debug.Log(www.error);
            }
            else
            if (www.isHttpError)
            {
                notif.transform.Find("BotNotif").Find("IsiNotif").GetComponent<Text>().text = "Server is under maintenance";
                notif.SetActive(true);
                maintenance = "yes";
                Debug.Log(www.error);
            }
            else
            {
                if (myObject.message == "success")
                {
                    PlayerPrefs.SetString("Character", "" + myObject.character);
                    PhotonNetwork.LocalPlayer.CustomProperties["character"] = myObject.character;

                    SceneManager.LoadScene("UserMenu");
                }
                else
                {
                    if (myObject.message == "Game is outdated, need update first")
                        notif.SetActive(true);
                    else if (myObject.message == "Server is under maintenance")
                    {
                        notif.transform.Find("BotNotif").Find("IsiNotif").GetComponent<Text>().text = "Server is under maintenance";
                        notif.SetActive(true);
                        maintenance = "yes";
                    }

                    else SceneManager.LoadScene("MainMenu");
                }
            }
        }
        StopCoroutine("LoginAccount");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void QuitGame()
    {
        if (Application.platform == RuntimePlatform.Android && maintenance=="no")
            Application.OpenURL("market://details?id=com.konoha.dimkaisy");
        Application.Quit();
    }
}

class AcceptAllCertificatesSignedWithASpecificKeyPublicKey : CertificateHandler
{

    // Encoded RSAPublicKey
    private static string PUB_KEY = "3082010A0282010100DD757BCE39A4F75DD29FDF45C7CAB60BB14704D0272B303E268052DA6DF338DF27A70988E92F23F454798BA6F5EA1A5B66489002022E44E783DDB950E115B7B17F6878AEF4E58E3DF4688DFB62B7C51D71ADE8491E71130D07BFD0AEF1204B1ABD6E25A415BD74A4EDE4E9D8269E237E937C240014A916152C9594262E00EBF9CB3550AD43C675F092DB088B255F087681BC348F9D1C3EE553B52754A61BEDFC82D53CC9E83E09E72A4BFD6CB2CAFA0243B928A0DAD937B5DAF9B344310F25C33A2245E5CE9A5CFAE5912C208E5BC433AB3310B3AF2490603F6BF45EE6B721D7441AAEB1FEB16E66C9EA178FD6CD682F98DEC75C27CF661C10B625858746491B0203010001";
    protected override bool ValidateCertificate(byte[] certificateData)
    {
        X509Certificate2 certificate = new X509Certificate2(certificateData);
        string pk = certificate.GetPublicKeyString();
        if (pk.ToLower().Equals(PUB_KEY.ToLower()))
            return true;
        return false;
    }
}