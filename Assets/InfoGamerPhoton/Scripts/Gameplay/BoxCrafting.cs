using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class BoxCrafting : MonoBehaviourPunCallbacks
{
    public Dictionary<int, BoxStation> BoxPosition;

    public string team;

    // Start is called before the first frame update
    void Start()
    {
        BoxPosition = new Dictionary<int, BoxStation>();
        string myteam = "red";
        string[] mystasiunsplit = gameObject.name.Split('#');
        int mystasiun = int.Parse(mystasiunsplit[1]);
        int mynourut = 0;

        if (gameObject.name.Contains("Red"))myteam="Red";
        else if (gameObject.name.Contains("Blue")) myteam = "Blue";
        else if (gameObject.name.Contains("Yellow")) myteam = "Yellow";
        else if (gameObject.name.Contains("Green")) myteam = "Green";

        BoxStation bStation = new BoxStation();
        bStation.position = new Vector3(transform.position.x - 0.4f, transform.position.y+0.6f, transform.position.z - 0.375f);
        bStation.isFilled = false;
        bStation.team = myteam;
        bStation.stasiun = mystasiun;
        bStation.nourut = mynourut;
        bStation.name = myteam+"_"+mynourut+"_"+mystasiun;
        mynourut++;

        BoxPosition.Add(1, bStation);

        bStation = new BoxStation();
        bStation.position = new Vector3(transform.position.x + 0.4f, transform.position.y + 0.6f, transform.position.z - 0.375f);
        bStation.isFilled = false;
        bStation.team = myteam;
        bStation.stasiun = mystasiun;
        bStation.nourut = mynourut;
        bStation.name = myteam + "_" + mynourut + "_" + mystasiun;
        mynourut++;

        BoxPosition.Add(2, bStation);

        bStation = new BoxStation();
        bStation.position = new Vector3(transform.position.x - 0.4f, transform.position.y + 0.6f, transform.position.z + 0.375f);
        bStation.isFilled = false;
        bStation.team = myteam;
        bStation.stasiun = mystasiun;
        bStation.nourut = mynourut;
        bStation.name = myteam + "_" + mynourut + "_" + mystasiun;
        mynourut++;

        BoxPosition.Add(3, bStation);

        bStation = new BoxStation();
        bStation.position = new Vector3(transform.position.x + 0.4f, transform.position.y + 0.6f, transform.position.z + 0.375f);
        bStation.isFilled = false;
        bStation.team = myteam;
        bStation.stasiun = mystasiun;
        bStation.nourut = mynourut;
        bStation.name = myteam + "_" + mynourut + "_" + mystasiun;
        mynourut++;

        BoxPosition.Add(4, bStation);

        bStation = new BoxStation();
        bStation.position = new Vector3(transform.position.x - 0.4f, transform.position.y + 1.35f, transform.position.z - 0.375f);
        bStation.isFilled = false;
        bStation.team = myteam;
        bStation.stasiun = mystasiun;
        bStation.nourut = mynourut;
        bStation.name = myteam + "_" + mynourut + "_" + mystasiun;
        mynourut++;

        BoxPosition.Add(5, bStation);

        bStation = new BoxStation();
        bStation.position = new Vector3(transform.position.x + 0.4f, transform.position.y + 1.35f, transform.position.z - 0.375f);
        bStation.isFilled = false;
        bStation.team = myteam;
        bStation.stasiun = mystasiun;
        bStation.nourut = mynourut;
        bStation.name = myteam + "_" + mynourut + "_" + mystasiun;
        mynourut++;

        BoxPosition.Add(6, bStation);

        bStation = new BoxStation();
        bStation.position = new Vector3(transform.position.x - 0.4f, transform.position.y + 1.35f, transform.position.z + 0.375f);
        bStation.isFilled = false;
        bStation.team = myteam;
        bStation.stasiun = mystasiun;
        bStation.nourut = mynourut;
        bStation.name = myteam + "_" + mynourut + "_" + mystasiun;
        mynourut++;

        BoxPosition.Add(7, bStation);

        bStation = new BoxStation();
        bStation.position = new Vector3(transform.position.x + 0.4f, transform.position.y + 1.35f, transform.position.z + 0.375f);
        bStation.isFilled = false;
        bStation.team = myteam;
        bStation.stasiun = mystasiun;
        bStation.nourut = mynourut;
        bStation.name = myteam + "_" + mynourut + "_" + mystasiun;
        mynourut++;

        BoxPosition.Add(8, bStation);


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(PhotonNetwork.IsMasterClient)
        if (collision.collider.tag.Equals("Obstacle")){
            Debug.Log("Tabrakan Craft");
            int iBoxTerdekat = 0;
            float distanceTerdekat = 10f;
            bool dapat = false;

            for (int i=0;i<BoxPosition.Count;i++) {
                if (Vector3.Distance(collision.collider.GetComponent<Rigidbody>().position, BoxPosition[i+1].position) <= distanceTerdekat && !BoxPosition[i+1].isFilled) {
                    if (i + 1 >= 5)
                    {
                        if(i+1 == 5 && BoxPosition[1].isFilled)
                        {
                            iBoxTerdekat = i + 1;
                            distanceTerdekat = Vector3.Distance(collision.collider.GetComponent<Rigidbody>().position, BoxPosition[i + 1].position);
                            dapat = true;
                        }
                        if (i + 1 == 6 && BoxPosition[2].isFilled)
                        {
                            iBoxTerdekat = i + 1;
                            distanceTerdekat = Vector3.Distance(collision.collider.GetComponent<Rigidbody>().position, BoxPosition[i + 1].position);
                            dapat = true;
                        }
                        if (i + 1 == 7 && BoxPosition[3].isFilled)
                        {
                            iBoxTerdekat = i + 1;
                            distanceTerdekat = Vector3.Distance(collision.collider.GetComponent<Rigidbody>().position, BoxPosition[i + 1].position);
                            dapat = true;
                        }
                        if (i + 1 == 8 && BoxPosition[4].isFilled)
                        {
                            iBoxTerdekat = i + 1;
                            distanceTerdekat = Vector3.Distance(collision.collider.GetComponent<Rigidbody>().position, BoxPosition[i + 1].position);
                            dapat = true;
                        }
                    }
                    else
                    {
                        iBoxTerdekat = i + 1;
                        distanceTerdekat = Vector3.Distance(collision.collider.GetComponent<Rigidbody>().position, BoxPosition[i + 1].position);
                        dapat = true;
                    }
                }
            }

            if (dapat)
            {
                GetComponent<PhotonView>().RPC("dapatBoxRPC", RpcTarget.All, iBoxTerdekat, collision.collider.name);
                
            }
        }
    }

    [PunRPC]
    void dapatBoxRPC(int iBoxTerdekat, string colname)
    {
        GameObject col = GameObject.Find(colname);
        BoxPosition[iBoxTerdekat].isFilled = true;
        BoxPosition[iBoxTerdekat].nameCube = col.name;
        col.transform.eulerAngles = new Vector3(0, 0, 0);
        col.GetComponent<Rigidbody>().position = BoxPosition[iBoxTerdekat].position;
        col.transform.position = BoxPosition[iBoxTerdekat].position;
        col.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        col.GetComponent<Rigidbody>().angularVelocity = new Vector3(0, 0, 0);
        col.GetComponent<Rigidbody>().isKinematic = true;
        col.GetComponent<PhotonView>().RPC("setStasiun", RpcTarget.All, name, iBoxTerdekat);

    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.tag.Equals("Obstacle"))
        {
        }
    }

    public void crackInStation()
    {
        int countRed = 0;
        int countBlue = 0;
        int countYellow = 0;
        int countGreen = 0;
        for (int i = 0; i < BoxPosition.Count; i++)
        {
            if (BoxPosition[i + 1].nameCube.Contains("Red")) countRed++;
            else if (BoxPosition[i + 1].nameCube.Contains("Blue")) countBlue++;
            else if (BoxPosition[i + 1].nameCube.Contains("Green")) countGreen++;
            else if (BoxPosition[i + 1].nameCube.Contains("Yellow")) countYellow++;
            if(BoxPosition[i + 1].nameCube!="")
            PhotonNetwork.Destroy(PhotonView.Find(GameObject.Find(BoxPosition[i + 1].nameCube).GetComponent<PhotonView>().ViewID));
            BoxPosition[i + 1].isFilled = false;
            BoxPosition[i + 1].nameCube = "";
        }

        if (countRed == 1 && countBlue == 0)
        {
            GameObject go = PhotonNetwork.InstantiateSceneObject(Path.Combine("Models/Weapon", "Baseball"), new Vector3(transform.position.x,transform.position.y+0.5f,transform.position.z), Quaternion.identity);
            go.name = "Baseball (" + go.GetPhotonView().ViewID + ")";
            go.GetComponent<PhotonView>().RPC("weaponrpc", RpcTarget.All, go.name, "Weapon Street Spawn");
        }else if (countRed == 0 && countBlue == 1)
        {
            GameObject go = PhotonNetwork.InstantiateSceneObject(Path.Combine("Models/Weapon", "BaseballU"), new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z), Quaternion.identity);
            go.name = "BaseballU (" + go.GetPhotonView().ViewID + ")";
            go.GetComponent<PhotonView>().RPC("weaponrpc", RpcTarget.All, go.name, "Weapon Street Spawn");
        }
        else if (countRed == 1 && countBlue == 1)
        {
            GameObject go = PhotonNetwork.InstantiateSceneObject(Path.Combine("Models/Weapon", "GunMolotov"), new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z), Quaternion.identity);
            go.name = "GunMolotov (" + go.GetPhotonView().ViewID + ")";
            go.GetComponent<PhotonView>().RPC("weaponrpc", RpcTarget.All, go.name, "Weapon Street Spawn");
        }
        else if(countRed == 2 && countBlue == 0)
        {
            GameObject go = PhotonNetwork.InstantiateSceneObject(Path.Combine("Models/Weapon", "Gun"), new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z), Quaternion.identity);
            go.name = "Gun (" + go.GetPhotonView().ViewID + ")";
            go.GetComponent<PhotonView>().RPC("weaponrpc", RpcTarget.All, go.name, "Weapon Street Spawn");
        }
        else if (countRed == 0 && countBlue == 2)
        {
            GameObject go = PhotonNetwork.InstantiateSceneObject(Path.Combine("Models/Weapon", "GunU"), new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z), Quaternion.identity);
            go.name = "GunU (" + go.GetPhotonView().ViewID + ")";
            go.GetComponent<PhotonView>().RPC("weaponrpc", RpcTarget.All, go.name, "Weapon Street Spawn");
        }
        else if (countRed == 3 && countBlue == 0)
        {
            GameObject go = PhotonNetwork.InstantiateSceneObject(Path.Combine("Models/Vehicle", "bike"), new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z), Quaternion.identity);
            go.name = "bike (" + go.GetPhotonView().ViewID + ")";
            go.GetComponent<PhotonView>().RPC("vehiclerpc", RpcTarget.All, go.name, "Weapon Street Spawn");
        }
        else if (countRed == 2 && countBlue ==1)
        {
            GameObject go = PhotonNetwork.InstantiateSceneObject(Path.Combine("Models/Vehicle", "bikebajaj"), new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z), Quaternion.identity);
            go.name = "bikebajaj (" + go.GetPhotonView().ViewID + ")";
            go.GetComponent<PhotonView>().RPC("vehiclerpc", RpcTarget.All, go.name, "Weapon Street Spawn");
        }
        else if (countRed == 3 && countBlue == 1)
        {
            GameObject go = PhotonNetwork.InstantiateSceneObject(Path.Combine("Models/Weapon", "GunFlame"), new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z), Quaternion.identity);
            go.name = "GunFlame (" + go.GetPhotonView().ViewID + ")";
            go.GetComponent<PhotonView>().RPC("weaponrpc", RpcTarget.All, go.name, "Weapon Street Spawn");
        }
        else if (countRed == 4 && countBlue == 0)
        {
            GameObject go = PhotonNetwork.InstantiateSceneObject(Path.Combine("Models/Weapon", "GunRocket"), new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z), Quaternion.identity);
            go.name = "GunRocket (" + go.GetPhotonView().ViewID + ")";
            go.GetComponent<PhotonView>().RPC("weaponrpc", RpcTarget.All, go.name, "Weapon Street Spawn");
        }
        else if (countRed == 4 && countBlue == 1)
        {
            GameObject go = PhotonNetwork.InstantiateSceneObject(Path.Combine("Models/AI", "Robot"), new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z), Quaternion.identity);
            go.name = "Robot (" + go.GetPhotonView().ViewID + ")";
            go.GetComponent<PhotonView>().RPC("robotrpc", RpcTarget.All, go.name, "Weapon Street Spawn", team);
        }
        else if (countRed == 5 && countBlue == 0)
        {
            GameObject go = PhotonNetwork.InstantiateSceneObject(Path.Combine("Models/Vehicle", "tank"), new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z), Quaternion.identity);
            go.name = "tank (" + go.GetPhotonView().ViewID + ")";
            go.GetComponent<PhotonView>().RPC("vehiclerpc", RpcTarget.All, go.name, "Weapon Street Spawn");
        }
        

    }

    public void getJumlahBoxBiru()
    {
        //BoxPosition[nourut + 1].isFilled = false;
        //BoxPosition[nourut + 1].nameCube = "";
    }

    [PunRPC]
    void setRemoveCube(int iBoxTerdekat, string colname)
    {
        
        if (iBoxTerdekat < 5 && iBoxTerdekat >= 1)
        {
            GameObject col = GameObject.Find(colname);
            BoxPosition[iBoxTerdekat].isFilled = false;
            BoxPosition[iBoxTerdekat].nameCube = "";
            col.GetComponent<PhotonView>().RPC("setStasiun", RpcTarget.All, "", -1);
            if (BoxPosition[iBoxTerdekat + 4].nameCube != "")
            {
                col = GameObject.Find(BoxPosition[iBoxTerdekat + 4].nameCube);
                BoxPosition[iBoxTerdekat + 4].isFilled = false;
                BoxPosition[iBoxTerdekat + 4].nameCube = "";
                col.GetComponent<PhotonView>().RPC("setStasiun", RpcTarget.All, "", -1);
                col.GetComponent<Rigidbody>().isKinematic = false;
            }
        }
        else if (iBoxTerdekat > 4)
        {
            GameObject col = GameObject.Find(colname);
            BoxPosition[iBoxTerdekat].isFilled = false;
            BoxPosition[iBoxTerdekat].nameCube = "";
            col.GetComponent<PhotonView>().RPC("setStasiun", RpcTarget.All, "", -1);
        }
        
    }

}
