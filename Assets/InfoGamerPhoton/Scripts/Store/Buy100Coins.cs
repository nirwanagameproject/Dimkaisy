using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Buy100Coins : MonoBehaviour
{
    //private string defaultText;

    // Start is called before the first frame update
    void Start()
    {
        //defaultText = transform.Find("Text").GetComponent<Text>().text;
        //StartCoroutine(LoadPrice());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onClickButton()
    {
        AudioSource audio = GameObject.Find("Clicked").GetComponent<AudioSource>();
        audio.Play();

        Purchaser.Instance.Buy100Coin();
    }

    /*private IEnumerator LoadPrice()
    {
        while (Purchaser.Instance.IsInitialized())
            yield return null;

        transform.Find("Text").GetComponent<Text>().text = defaultText + " " +Purchaser.Instance.getPrice(Purchaser.Instance.product_100Coin);
    }*/
}
