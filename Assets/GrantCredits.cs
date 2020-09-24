using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GrantCredits : MonoBehaviour
{
    public int userCredits;
    public GameObject notiftext;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GrantCreditsku(int credits)
    {
        userCredits = userCredits + credits;
        notiftext.GetComponent<Text>().text = ""+Purchaser.Instance.IsInitialized();
        Debug.Log("You received " +credits +" Credits!");
    }
}
