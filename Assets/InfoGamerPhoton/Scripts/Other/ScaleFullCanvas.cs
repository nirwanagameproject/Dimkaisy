using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleFullCanvas : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        RectTransform sr = GetComponent<RectTransform>();

        double width = sr.rect.width;
        double height = sr.rect.height;

        double worldScreenHeight = Screen.height;
        double worldScreenWidth = Screen.width;

        sr.localScale = new Vector3((float)(((sr.localScale.x * worldScreenWidth / 1024) + (sr.localScale.y * worldScreenHeight / 768)) / 2), (float)(((sr.localScale.x * worldScreenWidth / 1024) + (sr.localScale.y * worldScreenHeight / 768)) / 2), 1);

        sr.localPosition = new Vector3((float)(((sr.localPosition.x - GameObject.Find("Canvas").GetComponent<RectTransform>().rect.x) * worldScreenWidth / 1024) + GameObject.Find("Canvas").GetComponent<RectTransform>().rect.x), ((float)(((sr.localPosition.y + GameObject.Find("Canvas").GetComponent<RectTransform>().rect.y) * worldScreenHeight / 768) - GameObject.Find("Canvas").GetComponent<RectTransform>().rect.y)), 0);

    }

    // Update is called once per frame
    void Update()
    {

    }
}
