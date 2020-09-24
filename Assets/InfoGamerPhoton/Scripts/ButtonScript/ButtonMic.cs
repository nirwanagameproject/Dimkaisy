using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonMic : MonoBehaviour
{
    private Controller mycontroller;
    private AudioSource audio;
    private string mydeviceMic;
    // Start is called before the first frame update
    void Start()
    {

        mydeviceMic = Microphone.devices[0].ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if(mycontroller!=null) audio.gameObject.transform.position = GameObject.Find("Main Camera").transform.position;
    }

    public void OnClickButton()
    {
        mycontroller = Controller.Instance;
        audio = mycontroller.transform.Find("MyMic").GetComponent<AudioSource>();
        
        audio.clip = Microphone.Start(mydeviceMic, true, 999, 44100);

        StartCoroutine(RecordVoice());
    }

    IEnumerator RecordVoice()
    {
        yield return new WaitForSeconds(3);
        Microphone.End(mydeviceMic);
        audio.loop = true;
        Debug.Log("start playing... position is " + Microphone.GetPosition(null));
        audio.Play();
        
    }
}
