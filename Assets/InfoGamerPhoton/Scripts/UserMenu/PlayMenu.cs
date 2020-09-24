using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayMenu : MonoBehaviour
{
    public GameObject notifpanel;
    public GameObject notiftext;
    // Start is called before the first frame update
    void Start()
    {
        MyKoin.Instance.mykoinsemua.SetActive(true);
        MyKoin.Instance.buttonserver.SetActive(false);
        if(MainMenuController.Instance!=null)
        MainMenuController.Instance.gameObject.SetActive(false);
        MyKoin.Instance.StartCoroutine(MyKoin.Instance.GetCoin());
        if (!MyMusic.Instance.GetComponent<AudioSource>().isPlaying)
        MyMusic.Instance.GetComponent<AudioSource>().Play();

        if (PlayerPrefs.HasKey("Win")) {
            notifpanel.SetActive(true);
            PlayerPrefs.DeleteKey("Win");
            if(PlayerPrefs.GetInt("ActNumber") == 1)
            notiftext.GetComponent<Text>().text = "Congratulations!!\nYou get 15";
            else if (PlayerPrefs.GetInt("ActNumber") == 2)
                notiftext.GetComponent<Text>().text = "Congratulations!!\nYou get 20";
        }

        if (PlayerPrefs.HasKey("ActNumber"))
        {
            PlayerPrefs.DeleteKey("ActNumber");
            PlayerPrefs.DeleteKey("ActScene");
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onClickButton()
    {
        AudioSource audio = GameObject.Find("Clicked").GetComponent<AudioSource>();
        audio.Play();
        SceneManager.LoadScene("PlayMenu");
    }

    public void onClickOKRewards()
    {
        AudioSource audio = GameObject.Find("Clicked").GetComponent<AudioSource>();
        audio.Play();
        notifpanel.SetActive(false);
    }
}
