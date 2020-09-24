using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Register : MonoBehaviour
{
    public static LoginForm logform;
    public InputField username;
    public InputField email;
    public InputField phone;
    public InputField password;
    public GameObject notif;
    public MyPlayer myObject;

    // Start is called before the first frame update
    void Start()
    {
        logform = new LoginForm();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void DisableNotif()
    {
        notif.SetActive(false);
    }

    public void onClickButton()
    {
        AudioSource audio = GameObject.Find("Clicked").GetComponent<AudioSource>();
        audio.Play();

        logform.username = username.text;
        logform.password = password.text;

        StartCoroutine(LoginAccount());
    }

    public IEnumerator LoginAccount()
    {
        //string LoginUrl = "http://192.168.0.103/dimkaisy/register.php";
        string LoginUrl = MyConnection.Instance.LoginUrl + "dimkaisy/register.php";
        WWWForm Form = new WWWForm();
        Form.AddField("username", username.text);
        Form.AddField("phone", phone.text);
        Form.AddField("email", email.text);
        Form.AddField("password", password.text);

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
                Debug.Log("Form upload complete! " + www.downloadHandler.text);
                myObject = JsonUtility.FromJson<MyPlayer>(www.downloadHandler.text);

                if (myObject.message == "success")
                {
                    notif.transform.Find("Text").GetComponent<Text>().text = "Success Register. Check your email";
                    notif.GetComponent<Image>().color = Color.green;
                    notif.SetActive(true);
                    Invoke("DisableNotif", 3);
                }
                else
                {
                    notif.transform.Find("Text").GetComponent<Text>().text = myObject.message;
                    notif.GetComponent<Image>().color = Color.red;
                    notif.SetActive(true);
                    Invoke("DisableNotif", 3);
                }
            }
        }


        StopCoroutine(LoginAccount());
    }
}
