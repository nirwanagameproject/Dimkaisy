using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class SelectButton : MonoBehaviour
{
    public static string character;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onClickButton()
    {
        AudioSource audio = GameObject.Find("Clicked").GetComponent<AudioSource>();
        audio.Play();
        this.StartCoroutine(SetCharacter());
    }

    public IEnumerator SetCharacter()
    {
        //string LoginUrl = "http://192.168.0.103/dimkaisy/login.php";
        string LoginUrl = MyConnection.Instance.LoginUrl + "dimkaisy/setcharacter.php";
        WWWForm Form = new WWWForm();
        Form.AddField("username", PlayerPrefs.GetString("Username"));
        Form.AddField("password", PlayerPrefs.GetString("Password"));
        Form.AddField("charset", character);

        using (UnityWebRequest www = UnityWebRequest.Post(LoginUrl, Form))
        {
            www.certificateHandler = new AcceptAllCertificatesSignedWithASpecificKeyPublicKey();
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
            {
                
            }
            else
            {
                if (www.downloadHandler.text == "success")
                {
                    PlayerPrefs.SetString("Character", character);
                    ExitGames.Client.Photon.Hashtable setChar = new ExitGames.Client.Photon.Hashtable();
                    setChar.Add("character", character);
                    PhotonNetwork.LocalPlayer.SetCustomProperties(setChar, null);
                    SceneManager.LoadScene("UserMenu");
                }
                else
                {

                }
            }
        }


        StopCoroutine(SetCharacter());
    }
}
