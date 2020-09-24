using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Back : MonoBehaviour
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

        if (PlayerPrefs.HasKey("Username") && PlayerPrefs.HasKey("Password"))
		{
			SceneManager.LoadScene("UserMenu");
		}
		else
		{
			SceneManager.LoadScene("MainMenu");
		}
	}
}
