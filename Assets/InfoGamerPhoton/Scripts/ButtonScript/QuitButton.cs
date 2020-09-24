using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuitButton : MonoBehaviour
{
    private Controller mycontroller;
    // Start is called before the first frame update
    void Start()
    {
        mycontroller = Controller.Instance;
        mycontroller.GetComponent<PhotonView>().RPC("losing", RpcTarget.All);
        mycontroller.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnClickButton()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LeaveLobby();
        SceneManager.LoadScene("Endgame");
    }
}
