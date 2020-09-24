using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioClicked : MonoBehaviour
{
    private static AudioClicked instance = null;
    public static AudioClicked Instance
    {
        get { return instance; }
    }
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
    }
}
