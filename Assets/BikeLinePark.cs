using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BikeLinePark : MonoBehaviour
{
    Collider[] mycolliderVehicle;
    public List<GameObject> mymotor;
    public int jumlahbike;
    // Start is called before the first frame update
    void Start()
    {
        mymotor = new List<GameObject>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        mycolliderVehicle = Physics.OverlapSphere(transform.position, 2f, LayerMask.GetMask("Vehicle"));
        bool adamotor = mycolliderVehicle.Length != 0;
        for(int i=0;i< mycolliderVehicle.Length;i++)
        if (adamotor && mycolliderVehicle[i].name.Contains("bikeMission") && PhotonNetwork.IsMasterClient)
        {
                if (mymotor.Count == 0)
                {
                    mymotor.Add(mycolliderVehicle[i].gameObject);
                    ExitGames.Client.Photon.Hashtable getbike = new ExitGames.Client.Photon.Hashtable();
                    getbike.Add("getbike", 0);
                    PhotonNetwork.CurrentRoom.SetCustomProperties(getbike, null);
                    for (int j=1;j<5;j++)
                        if(GameObject.Find("Obstacles").transform.Find("Plane" + j).GetComponent<BikeLinePark>().mymotor.Count!=0)
                        {
                            getbike = new ExitGames.Client.Photon.Hashtable();
                            getbike.Add("getbike", (int)PhotonNetwork.CurrentRoom.CustomProperties["getbike"]+1);
                            PhotonNetwork.CurrentRoom.SetCustomProperties(getbike, null);
                            GameSetupController.instance.GetComponent<PhotonView>().RPC("updateObjective", RpcTarget.All);
                            if ((int)PhotonNetwork.CurrentRoom.CustomProperties["getbike"] == 4)
                            {
                                GameSetupController.instance.GetComponent<PhotonView>().RPC("winact2", RpcTarget.All);
                            }
                        }
                }
            }
        if (!adamotor && PhotonNetwork.IsMasterClient)
        {
            if (mymotor.Count == 1)
            {
                ExitGames.Client.Photon.Hashtable getbike = new ExitGames.Client.Photon.Hashtable();
                getbike.Add("getbike", (int)PhotonNetwork.CurrentRoom.CustomProperties["getbike"] - 1);
                PhotonNetwork.CurrentRoom.SetCustomProperties(getbike, null);
                GameSetupController.instance.GetComponent<PhotonView>().RPC("updateObjective", RpcTarget.All);
                GameSetupController.instance.MenuObjectiveText.text = "- Find 4 bikes and bring it to park bike line near Monas ("+ (int)PhotonNetwork.CurrentRoom.CustomProperties["getbike"] + "/4)\n- Protect The Monas";
            }
            mymotor.Clear();
        }
    }
}
