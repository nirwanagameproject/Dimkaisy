using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CampaignController : MonoBehaviour
{
    public string url;
    AssetBundle bundle;
    public GameObject BGStory;
    public GameObject Go;
    public GameObject canceldownload;
    public int choseact;

    public Text textmapdownload;
    public GameObject notifpanel;
    public GameObject MyBarDownloadProgress;
    public float downloadprogress;

    public GameObject notifnotreadypanel;
    public GameObject actcontainer;

    public int actversion;

    bool downloading;
    // Start is called before the first frame update
    void Start()
    {
        Go.SetActive(false);
        choseact = 1;
        if(Application.platform == RuntimePlatform.Android) url = MyConnection.Instance.LoginUrl + "dimkaisy/AssetBundlesAPK/act1.dlc";
        if (Application.platform == RuntimePlatform.WindowsEditor) url = MyConnection.Instance.LoginUrl + "dimkaisy/AssetBundles/act1.dlc";
        if (Application.platform == RuntimePlatform.IPhonePlayer) url = MyConnection.Instance.LoginUrl + "dimkaisy/AssetBundlesIOS/act1.dlc";

        StartCoroutine(CekVersionAct());
        
        //StartCoroutine(InstantiateObject());

    }

    IEnumerator DownloadAB(string namefile)
    {
        yield return StartCoroutine(AssetBundleManager.downloadAssetBundle(url, actversion, namefile));
        //bundle = AssetBundleManager.getAssetBundle(url, 0, "act" + choseact);
    }

    public IEnumerator CekFileSize(string namefile)
    {
        string myurl = "";
        if (Application.platform == RuntimePlatform.Android) myurl = MyConnection.Instance.LoginUrl + "dimkaisy/AssetBundlesAPK/sizefile.php";
        if (Application.platform == RuntimePlatform.WindowsEditor) myurl = MyConnection.Instance.LoginUrl + "dimkaisy/AssetBundles/sizefile.php";
        if (Application.platform == RuntimePlatform.IPhonePlayer) myurl = MyConnection.Instance.LoginUrl + "dimkaisy/AssetBundlesIOS/sizefile.php";
        myurl += "?file="+namefile;

        using (UnityWebRequest www = UnityWebRequest.Get(myurl))
        {
            www.certificateHandler = new AcceptAllCertificatesSignedWithASpecificKeyPublicKey();
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
               textmapdownload.text = "Download Map ("+www.downloadHandler.text+")";
            }
        }


        StopCoroutine(CekFileSize(namefile));
    }

    public IEnumerator CekVersionAct()
    {
        string myurl = MyConnection.Instance.LoginUrl +"dimkaisy/actversion.php";
        myurl += "?act=act" +choseact;

        using (UnityWebRequest www = UnityWebRequest.Get(myurl))
        {
            www.certificateHandler = new AcceptAllCertificatesSignedWithASpecificKeyPublicKey();
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                actversion = Int32.Parse(www.downloadHandler.text);
                Debug.Log("my version act: "+Int32.Parse(www.downloadHandler.text));

                bundle = AssetBundleManager.getAssetBundle(url, actversion, "act" + choseact);

                if (!bundle)
                {
                    BGStory.transform.Find("Story_Act"+ choseact).Find("TextCerita").gameObject.SetActive(false);
                    BGStory.transform.Find("Story_Act"+ choseact).Find("GambarCerita").gameObject.SetActive(false);
                    BGStory.transform.Find("Story_Act"+ choseact).Find("DownloadButton").gameObject.SetActive(true);
                    BGStory.transform.Find("Story_Act"+ choseact).Find("Text").gameObject.SetActive(true);
                    StartCoroutine(CekFileSize("act" + choseact + ".dlc"));
                    Go.SetActive(false);
                }
                else
                {
                    Go.SetActive(true);
                    PlayerPrefs.SetInt("ActNumber", choseact);
                    PlayerPrefs.SetString("ActScene", bundle.GetAllScenePaths()[0]);
                }
            }
        }


        StopCoroutine(CekVersionAct());
    }

    // Update is called once per frame
    void Update()
    {
        if (downloading)
        {
            MyBarDownloadProgress.transform.Find("GreenBar").GetComponent<Image>().fillAmount = AssetBundleManager.mydownload.downloadProgress;
            MyBarDownloadProgress.transform.Find("Text").GetComponent<Text>().text = (int)(AssetBundleManager.mydownload.downloadProgress * 100)+" %";

            if (MyBarDownloadProgress.transform.Find("GreenBar").GetComponent<Image>().fillAmount == 1)
            {
                MyBarDownloadProgress.transform.Find("GreenBar").GetComponent<Image>().fillAmount = 0;
                MyBarDownloadProgress.transform.Find("Text").GetComponent<Text>().text = "0 %";
                downloading = false;
                notifpanel.SetActive(false);
                BGStory.transform.Find("Story_Act" + choseact).Find("TextCerita").gameObject.SetActive(true);
                BGStory.transform.Find("Story_Act" + choseact).Find("GambarCerita").gameObject.SetActive(true);
                BGStory.transform.Find("Story_Act" + choseact).Find("DownloadButton").gameObject.SetActive(false);
                BGStory.transform.Find("Story_Act" + choseact).Find("Text").gameObject.SetActive(false);
                Go.SetActive(true);
                bundle = AssetBundleManager.getAssetBundle(url, actversion, "act" + choseact);
                PlayerPrefs.SetInt("ActNumber", choseact);
                
                //Debug.Log(AssetBundleManager.mydownload.assetBundle.name);
            }
        }
    }

    public void downloadmap(string namefile)
    {
        if (Application.platform == RuntimePlatform.Android) url = MyConnection.Instance.LoginUrl + "dimkaisy/AssetBundlesAPK/" + namefile+".dlc";
        if (Application.platform == RuntimePlatform.IPhonePlayer) url = MyConnection.Instance.LoginUrl + "dimkaisy/AssetBundlesIOS/" + namefile+".dlc";
        if (Application.platform == RuntimePlatform.OSXEditor) url = MyConnection.Instance.LoginUrl + "dimkaisy/AssetBundles/" + namefile+".dlc";
        //StartCoroutine(AssetBundleManager.downloadAsset(url));

        //bundle = AssetBundleManager.getAssetBundle(url, 0);
        //if (!bundle)
        //{
        StartCoroutine(DownloadAB(namefile));
            downloading = true;
            notifpanel.SetActive(true);
        //}
        
    }


    public void GoBack()
    {
        AudioSource audio = GameObject.Find("Clicked").GetComponent<AudioSource>();
        audio.Play();
        SceneManager.LoadScene("PlayMenu");
    }

    public void GoToNext()
    {
        AudioSource audio = GameObject.Find("Clicked").GetComponent<AudioSource>();
        audio.Play();
        SceneManager.LoadScene("LobbyCampaign");
        //SceneManager.LoadScene(PlayerPrefs.GetString("ActScene"));
    }

    public void NotAvailable()
    {
        notifnotreadypanel.SetActive(true);
    }

    public void NotAvailableOK()
    {
        notifnotreadypanel.SetActive(false);
    }

    public void ClickAct(int myact)
    {
        AudioSource audio = GameObject.Find("Clicked").GetComponent<AudioSource>();
        audio.Play();

        Go.SetActive(false);
        BGStory.transform.Find("Story_Act" + choseact).gameObject.SetActive(false);
        actcontainer.transform.Find("Act" + choseact).GetComponent<Image>().color = Color.white;
        choseact = myact;
        actcontainer.transform.Find("Act" + choseact).GetComponent<Image>().color = new Color32(134, 255, 148,255);
        BGStory.transform.Find("Story_Act" + choseact).gameObject.SetActive(true);
        if (Application.platform == RuntimePlatform.Android) url = MyConnection.Instance.LoginUrl + "dimkaisy/AssetBundlesAPK/act"+ choseact + ".dlc";
        if (Application.platform == RuntimePlatform.WindowsEditor) url = MyConnection.Instance.LoginUrl + "dimkaisy/AssetBundles/act"+ choseact + ".dlc";
        if (Application.platform == RuntimePlatform.IPhonePlayer) url = MyConnection.Instance.LoginUrl + "dimkaisy/AssetBundlesIOS/act"+ choseact + ".dlc";
        textmapdownload = BGStory.transform.Find("Story_Act" + choseact).Find("DownloadButton").Find("Text").GetComponent<Text>();
        StartCoroutine(CekVersionAct());
    }
}
