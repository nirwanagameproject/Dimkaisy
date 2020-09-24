using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayMenuController : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject tutorial;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ClickBack()
    {
        AudioSource audio = GameObject.Find("Clicked").GetComponent<AudioSource>();
        audio.Play();

        if (PlayerPrefs.HasKey("Username") && PlayerPrefs.HasKey("Password"))
        {
            SceneManager.LoadScene("UserMenu");
        }
        else
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    public void ClickCampaign()
    {
        AudioSource audio = GameObject.Find("Clicked").GetComponent<AudioSource>();
        audio.Play();

        if (PlayerPrefs.HasKey("Username") && PlayerPrefs.HasKey("Password"))
        {
            SceneManager.LoadScene("Campaign");
        }
        else
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    public void ClickTutorial()
    {
        AudioSource audio = GameObject.Find("Clicked").GetComponent<AudioSource>();
        audio.Play();

        if (PlayerPrefs.HasKey("Username") && PlayerPrefs.HasKey("Password"))
        {
            tutorial.SetActive(true);
        }
        else
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    public void ClickPublicMatch()
    {
        AudioSource audio = GameObject.Find("Clicked").GetComponent<AudioSource>();
        audio.Play();

        if (PlayerPrefs.HasKey("Username") && PlayerPrefs.HasKey("Password"))
        {
            SceneManager.LoadScene("Lobby");
        }
        else
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    public void ClickOKTutorial()
    {
        AudioSource audio = GameObject.Find("Clicked").GetComponent<AudioSource>();
        audio.Play();

        tutorial.SetActive(false);
    }
}
