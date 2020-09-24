using System;
using System.Collections;
using UnityEngine;

public class MyMusic : MonoBehaviour
{


    private static MyMusic instance = null;
    public static MyMusic Instance
    {
        get { return instance; }
    }
    void Awake()
    {
        if (instance != null && instance != this) {
            Destroy(this.gameObject);
            return;
        } else
        {
            instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
    }

}