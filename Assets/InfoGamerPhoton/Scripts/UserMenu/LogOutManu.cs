using Firebase.Auth;
using Google;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LogOutManu : MonoBehaviour
{
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
        PlayerPrefs.DeleteKey("Username");
        PlayerPrefs.DeleteKey("Password");
        if (PlayerPrefs.HasKey("Token"))
        {
            PlayerPrefs.DeleteKey("Token");
            googleSignIn.loggedout = true;
            SceneManager.LoadScene("MainMenu");
        }
        else
        SceneManager.LoadScene("MainMenu");
    }

    public void gotoMainmenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
