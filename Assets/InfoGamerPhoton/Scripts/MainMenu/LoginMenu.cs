using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if(MyKoin.Instance!=null)
        MyKoin.Instance.mykoinsemua.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onClickButton()
    {
        AudioSource audio = GameObject.Find("Clicked").GetComponent<AudioSource>();
        audio.Play();
        //Text txt = transform.Find("Text").GetComponent<Text>();
        //txt.text = "Oke";
        SceneManager.LoadScene("Login");
    }
}
