using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class spawnRobot : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if(name.Contains("SpawnRobot1"))
        InvokeRepeating("spawnRobotRandom", 90f,90f);
        else if (name.Contains("SpawnRobot2"))
            InvokeRepeating("spawnRobotRandom", 75f, 75f); 
        else InvokeRepeating("spawnRobotRandom", 60f, 60f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void spawnRobotRandom()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            int myrandom = Random.Range(1,5);
            string tambahan = "";
            if (myrandom == 1) tambahan = "Flame";
            else if (myrandom == 2) return;
            else if (myrandom == 3) return;
            else if (myrandom == 4) return;
            GameObject go = PhotonNetwork.InstantiateSceneObject(Path.Combine("Models/AI", "Robot"+tambahan), new Vector3(transform.position.x, transform.position.y + 5f, transform.position.z), Quaternion.identity);
            go.name = "Robot"+tambahan+"HitBase (" + go.GetPhotonView().ViewID + ")";
            go.GetComponent<PhotonView>().RPC("robotrpc", RpcTarget.All, go.name, "Weapon Street Spawn", "");
        }
    }
}
