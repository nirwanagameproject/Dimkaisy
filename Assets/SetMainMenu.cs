using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetMainMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        MainMenuController.Instance.gameObject.SetActive(true);
        if(MyKoin.Instance.mykoinsemua!=null)
        MyKoin.Instance.mykoinsemua.SetActive(false);

        
        if (googleSignIn.udahlogin && googleSignIn.loggedout)
        {
            //MainMenuController.Instance.transform.Find("Text (1)").GetComponent<Text>().text = "tai";
            MainMenuController.Instance.transform.Find("Firebase").GetComponent<googleSignIn>().SignOutFromGoogle();
            //MainMenuController.Instance.transform.Find("Text (1)").GetComponent<Text>().text = "masuk";
            googleSignIn.loggedout = false;
        }//Invoke("signin",1f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
