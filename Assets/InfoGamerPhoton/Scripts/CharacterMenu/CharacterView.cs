using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterView : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        double worldScreenHeight = Screen.height;
        double worldScreenWidth = Screen.width;

        float scales = (float)(((worldScreenHeight / 1125)));
        //float s = Mathf.Min((float)worldScreenWidth, (float)worldScreenHeight);
        transform.position = Camera.main.ScreenToWorldPoint(new Vector3((float)(((420) * worldScreenWidth / 2436)), ((float)(((462.5f-transform.localScale.y/10) * worldScreenHeight / 1125))), 800));
        //transform.localScale = new Vector3((float)(250 * s), (float)(250 * s), (float)(250 * s));
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(SelectButton.character != "6")
        transform.Rotate(0, 30 * Time.deltaTime, 0);
    }
}
