using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Endgame : MonoBehaviour
{
    public GameObject myloading;
    public bool selesai;
    // Start is called before the first frame update
    void Start()
    {
        
        if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
            SceneManager.LoadScene("UserMenu");
        else if (Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor)
            SceneManager.LoadScene("UserMenu");
        else if (Application.platform == RuntimePlatform.LinuxPlayer || Application.platform == RuntimePlatform.LinuxEditor)
            SceneManager.LoadScene("UserMenu");
    }

    // Update is called once per frame
    void Update()
    {
        if(GameObject.Find("Canvas").GetComponent<AdManager>().rewardBasedVideo!=null)
        if (!selesai && GameObject.Find("Canvas").GetComponent<AdManager>().rewardBasedVideo.IsLoaded())
        {
            GameObject.Find("Canvas").GetComponent<AdManager>().ShowRewardedAd();
            selesai = true;
        }
    }

    public void ClickAdsButton()
    {
        AudioSource audio = GameObject.Find("Clicked").GetComponent<AudioSource>();
        audio.Play();

        myloading.SetActive(true);
        GameObject.Find("Canvas").GetComponent<AdManager>().RequestRewardBasedVideo();
        gameObject.SetActive(false);
    }
}
