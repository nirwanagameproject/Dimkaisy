using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquidwardButton : MonoBehaviour
{
    public GameObject selectButton;
    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey("Character"))
        {
            if (PlayerPrefs.GetString("Character") == "3")
            {
                onClickButton();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onClickButton()
    {
        selectButton.SetActive(true);

        AudioSource audio = GameObject.Find("Clicked").GetComponent<AudioSource>();
        audio.Play();

        double worldScreenHeight = Screen.height;
        double worldScreenWidth = Screen.width;

        Destroy(GameObject.Find("Canvas3D").transform.Find("Player").gameObject);
        GameObject playerPrefab = Resources.Load<GameObject>("Models/Squidward/Player");
        GameObject go = Instantiate(playerPrefab, GameObject.Find("Canvas3D").transform);
        //go.transform.parent = GameObject.Find("Canvas3D").transform;
        go.name = "Player";
        go.transform.eulerAngles = new Vector3(0, 180, 0);
        go.transform.localScale = new Vector3(2000, 2000, 2000);
        go.AddComponent<CharacterView>();
        if (CharacterController.Instance.NameChar != null)
            CharacterController.Instance.NameCharRender.text = CharacterController.Instance.NameChar[2];
        SelectButton.character = "3";
    }
}
