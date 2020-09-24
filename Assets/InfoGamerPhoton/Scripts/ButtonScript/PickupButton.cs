using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupButton : MonoBehaviour
{
    private Controller mycontroller;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnClickButton()
    {
        mycontroller = Controller.Instance;
        mycontroller.MyPickUp();
    }
}
