using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterController : MonoBehaviour
{
    public GameObject locked1;
    public GameObject locked2;
    public GameObject locked3;
    public GameObject locked4;
    public GameObject locked5;
    public GameObject locked6;
    public GameObject locked7;
    public GameObject locked8;

    public GameObject unlocked;

    public GameObject NotifPanel;
    public GameObject buttonSelect;
    public string[] NameChar;
    public Text NameCharRender;

    public bool buttonexit;

    public int charpilihan;

    private static CharacterController instance = null;
    public static CharacterController Instance
    {
        get { return instance; }
    }
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        NotifPanel.SetActive(true);
    }

    void Start()
    {
        StartCoroutine(GetCharacter());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator GetCharacter()
    {
        //string LoginUrl = "http://192.168.0.103/dimkaisy/login.php";
        string LoginUrl = MyConnection.Instance.LoginUrl +"dimkaisy/getcharacter.php";
        WWWForm Form = new WWWForm();
        Form.AddField("username", PlayerPrefs.GetString("Username"));
        Form.AddField("password", PlayerPrefs.GetString("Password"));

        using (UnityWebRequest www = UnityWebRequest.Post(LoginUrl, Form))
        {
            www.certificateHandler = new AcceptAllCertificatesSignedWithASpecificKeyPublicKey();
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                NotifPanel.transform.Find("BotNotif").Find("NotifTunggu").gameObject.SetActive(false);
                NotifPanel.transform.Find("BotNotif").Find("IsiNotif").GetComponent<Text>().text = "Connection error";
                NotifPanel.transform.Find("BotNotif").Find("IsiNotif").gameObject.SetActive(true);
                NotifPanel.transform.Find("BotNotif").Find("ButtonOK").gameObject.SetActive(true);

                buttonexit = true;
            }
            else
            {
                NotifPanel.transform.Find("BotNotif").Find("NotifTunggu").gameObject.SetActive(true);
                NotifPanel.transform.Find("BotNotif").Find("IsiNotif").gameObject.SetActive(false);
                NotifPanel.transform.Find("BotNotif").Find("ButtonYes").gameObject.SetActive(false);
                NotifPanel.transform.Find("BotNotif").Find("ButtonNo").gameObject.SetActive(false);
                NotifPanel.transform.Find("BotNotif").Find("ButtonOK").gameObject.SetActive(false);

                NotifPanel.SetActive(false);

                var jku = JSON.Parse(www.downloadHandler.text);
                
                for(var i = 0; i < jku.Count; i++)
                {
                    if (jku[i]["character"] == 1 && jku[i]["unlocked"] == 1)
                    {
                        locked1.SetActive(false);
                        locked1.transform.parent.gameObject.GetComponent<Button>().enabled = true;
                    }
                    else if (jku[i]["character"] == 2 && jku[i]["unlocked"] == 1)
                    {
                        locked2.SetActive(false);
                        locked2.transform.parent.gameObject.GetComponent<Button>().enabled = true;
                    }
                    else if (jku[i]["character"] == 3 && jku[i]["unlocked"] == 1)
                    {
                        locked3.SetActive(false);
                        locked3.transform.parent.gameObject.GetComponent<Button>().enabled = true;
                    }
                    else if (jku[i]["character"] == 4 && jku[i]["unlocked"] == 1)
                    {
                        locked4.SetActive(false);
                        locked4.transform.parent.gameObject.GetComponent<Button>().enabled = true;
                    }
                    else if (jku[i]["character"] == 5 && jku[i]["unlocked"] == 1)
                    {
                        locked5.SetActive(false);
                        locked5.transform.parent.gameObject.GetComponent<Button>().enabled = true;
                    }
                    else if (jku[i]["character"] == 6 && jku[i]["unlocked"] == 1)
                    {
                        locked6.SetActive(false);
                        locked6.transform.parent.gameObject.GetComponent<Button>().enabled = true;
                    }
                    else if (jku[i]["character"] == 7 && jku[i]["unlocked"] == 1)
                    {
                        locked7.SetActive(false);
                        locked7.transform.parent.gameObject.GetComponent<Button>().enabled = true;
                    }
                    else if (jku[i]["character"] == 8 && jku[i]["unlocked"] == 1)
                    {
                        locked8.SetActive(false);
                        locked8.transform.parent.gameObject.GetComponent<Button>().enabled = true;
                    }
                    NameChar[i] = jku[i]["name"];

                }
                NameCharRender.text = jku[Int32.Parse(PlayerPrefs.GetString("Character"))-1]["name"];
            }
        }


        StopCoroutine(GetCharacter());
    }

    public void OnClickLockedChar(int charnum)
    {
        NotifPanel.SetActive(true);
        buttonSelect.SetActive(false);

        double worldScreenHeight = Screen.height;
        double worldScreenWidth = Screen.width;

        Destroy(GameObject.Find("Canvas3D").transform.Find("Player").gameObject);
        string model = "";
        if (charnum == 4) model = "Models/Sandy/Player";
        if (charnum == 5) model = "Models/Mr Krabs/Player";
        if (charnum == 6) model = "Models/Orang/Player";
        GameObject playerPrefab = Resources.Load<GameObject>(model);
        GameObject go = Instantiate(playerPrefab, GameObject.Find("Canvas3D").transform);
        //go.transform.parent = GameObject.Find("Canvas3D").transform;
        go.name = "Player";
        go.transform.eulerAngles = new Vector3(0, 180, 0);
        go.transform.localScale = new Vector3(2000, 2000, 2000);
        go.AddComponent<CharacterView>();
        SelectButton.character = ""+charnum;
        NameCharRender.text = NameChar[charnum-1];

        StartCoroutine(GetCharStats(charnum));
    }

    public IEnumerator GetCharStats(int charnum)
    {
        //string LoginUrl = "http://192.168.0.103/dimkaisy/login.php";
        string LoginUrl = MyConnection.Instance.LoginUrl + "dimkaisy/getcharstats.php";
        WWWForm Form = new WWWForm();
        Form.AddField("mychar", charnum);

        using (UnityWebRequest www = UnityWebRequest.Post(LoginUrl, Form))
        {
            www.certificateHandler = new AcceptAllCertificatesSignedWithASpecificKeyPublicKey();
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                NotifPanel.transform.Find("BotNotif").Find("NotifTunggu").gameObject.SetActive(false);
                NotifPanel.transform.Find("BotNotif").Find("IsiNotif").GetComponent<Text>().text = "Connection error";
                NotifPanel.transform.Find("BotNotif").Find("IsiNotif").gameObject.SetActive(true);
                NotifPanel.transform.Find("BotNotif").Find("ButtonOK").gameObject.SetActive(true);
            }
            else
            {
                NotifPanel.transform.Find("BotNotif").Find("NotifTunggu").gameObject.SetActive(false);
                NotifPanel.transform.Find("BotNotif").Find("IsiNotif").GetComponent<Text>().text = "Unlock this character costs "+ www.downloadHandler.text+" Coins";
                NotifPanel.transform.Find("BotNotif").Find("IsiNotif").gameObject.SetActive(true);
                NotifPanel.transform.Find("BotNotif").Find("ButtonYes").gameObject.SetActive(true);
                NotifPanel.transform.Find("BotNotif").Find("ButtonNo").gameObject.SetActive(true);

                charpilihan = charnum;
            }
        }


        StopCoroutine(GetCharacter());
    }

    public void OnClickYes()
    {
        NotifPanel.transform.Find("BotNotif").Find("NotifTunggu").gameObject.SetActive(true);
        NotifPanel.transform.Find("BotNotif").Find("IsiNotif").gameObject.SetActive(false);
        NotifPanel.transform.Find("BotNotif").Find("ButtonYes").gameObject.SetActive(false);
        NotifPanel.transform.Find("BotNotif").Find("ButtonNo").gameObject.SetActive(false);

        StartCoroutine(BuyCharacter(charpilihan));
    }

    public void OnClickNo()
    {
        NotifPanel.transform.Find("BotNotif").Find("NotifTunggu").gameObject.SetActive(true);
        NotifPanel.transform.Find("BotNotif").Find("IsiNotif").gameObject.SetActive(false);
        NotifPanel.transform.Find("BotNotif").Find("ButtonYes").gameObject.SetActive(false);
        NotifPanel.transform.Find("BotNotif").Find("ButtonNo").gameObject.SetActive(false);
        NotifPanel.transform.Find("BotNotif").Find("ButtonOK").gameObject.SetActive(false);

        NotifPanel.SetActive(false);

        if(buttonexit) SceneManager.LoadScene("UserMenu");
    }

    public IEnumerator BuyCharacter(int charnum)
    {
        //string LoginUrl = "http://192.168.0.103/dimkaisy/login.php";
        string LoginUrl = MyConnection.Instance.LoginUrl +"dimkaisy/buycharacter.php";
        WWWForm Form = new WWWForm();
        Form.AddField("username", PlayerPrefs.GetString("Username"));
        Form.AddField("password", PlayerPrefs.GetString("Password"));
        Form.AddField("mychar", charnum);

        using (UnityWebRequest www = UnityWebRequest.Post(LoginUrl, Form))
        {
            www.certificateHandler = new AcceptAllCertificatesSignedWithASpecificKeyPublicKey();
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                NotifPanel.transform.Find("BotNotif").Find("NotifTunggu").gameObject.SetActive(false);
                NotifPanel.transform.Find("BotNotif").Find("IsiNotif").GetComponent<Text>().text = "Connection error";
                NotifPanel.transform.Find("BotNotif").Find("IsiNotif").gameObject.SetActive(true);
                NotifPanel.transform.Find("BotNotif").Find("ButtonOK").gameObject.SetActive(true);
            }
            else
            {
                if (www.downloadHandler.text == "success")
                {
                    NotifPanel.transform.Find("BotNotif").Find("NotifTunggu").gameObject.SetActive(false);
                    NotifPanel.transform.Find("BotNotif").Find("IsiNotif").GetComponent<Text>().text = "Congratulations!!\nYou unlocked this character!!";
                    NotifPanel.transform.Find("BotNotif").Find("IsiNotif").gameObject.SetActive(true);
                    NotifPanel.transform.Find("BotNotif").Find("ButtonOK").gameObject.SetActive(true);
                    buttonSelect.SetActive(true);

                    if (charpilihan == 4)
                    {
                        locked4.SetActive(false);
                        locked4.transform.parent.gameObject.GetComponent<Button>().enabled = true;
                    }else if (charpilihan == 5)
                    {
                        locked5.SetActive(false);
                        locked5.transform.parent.gameObject.GetComponent<Button>().enabled = true;
                    }
                    else if (charpilihan == 6)
                    {
                        locked6.SetActive(false);
                        locked6.transform.parent.gameObject.GetComponent<Button>().enabled = true;
                    }

                    MyKoin.Instance.StartCoroutine(MyKoin.Instance.GetCoin());
                }
                else
                {
                    NotifPanel.transform.Find("BotNotif").Find("NotifTunggu").gameObject.SetActive(false);
                    NotifPanel.transform.Find("BotNotif").Find("IsiNotif").GetComponent<Text>().text = "You don't have enough coin";
                    NotifPanel.transform.Find("BotNotif").Find("IsiNotif").gameObject.SetActive(true);
                    NotifPanel.transform.Find("BotNotif").Find("ButtonOK").gameObject.SetActive(true);
                }
            }
        }


        StopCoroutine(GetCharacter());
    }
}