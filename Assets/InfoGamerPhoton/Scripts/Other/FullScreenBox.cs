using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullScreenBox : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
		RectTransform sr = GetComponent<RectTransform>();

		double width = sr.rect.width;
		double height = sr.rect.height;

		double worldScreenHeight = Screen.height;
		double worldScreenWidth = Screen.width;

		sr.localScale = new Vector3((float)(((worldScreenWidth / 2688)+ (worldScreenHeight / 1242))/2), (float)(((worldScreenWidth / 2688) + (worldScreenHeight / 1242)) / 2), 1);

		sr.localPosition = new Vector3((float)(sr.localPosition.x * worldScreenWidth / 2688), ((float)(sr.localPosition.y * worldScreenHeight / 1242)), 0);


	}

	// Update is called once per frame
	void Update()
    {
        
    }
}
