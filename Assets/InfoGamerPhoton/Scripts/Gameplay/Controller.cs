using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using ExitGames.Client.Photon;
using Photon.Realtime;

public class Controller : MonoBehaviour
{
    public static Controller Instance { get; private set; }

    //Input
    protected Player1 Player;

    //Parameters
    protected const float RotationSpeed = 10;

    //Camera Controll
    public Vector3 CameraPivot;
    public float CameraDistance;
    protected float InputRotationX;
    protected float InputRotationY;
    public LayerMask grounds;

    protected Vector3 CharacterPivot;
    protected Vector3 LookDirection;

    protected Vector3 CameraVelocity;

    public float smooth = 0.3f;
    public float height;
    public Joystick joystick;
    public Joystick joystickAttack;

    Collider[] mycolliderVehicle;

    [SerializeField]
    public GameObject ButtonCrack;
    [SerializeField]
    public GameObject ButtonThrowWeapon;
    [SerializeField]
    public GameObject ButtonJump;
    [SerializeField]
    public GameObject ButtonPickUpBox;

    private Vector3 velocity = Vector3.zero;

    // Use this for initialization
    void Start()
    {
        ButtonCrack = GameObject.Find("ButtonCrack");
        ButtonThrowWeapon = GameObject.Find("ButtonThrowWeapon");
        ButtonPickUpBox = GameObject.Find("ButtonPickupBox");
        ButtonJump = GameObject.Find("ButtonJump");

        Player = GetComponent<Player1>();
        Instance = this;
        joystick = GameObject.Find("Canvas").transform.Find("Fixed Joystick").GetComponent<Joystick>();
        joystickAttack = GameObject.Find("Canvas").transform.Find("Fixed Joystick Attack").GetComponent<Joystick>();
    }


        // Update is called once per frame
     void FixedUpdate()
     {


        //left and forward
        var characterForward = Quaternion.AngleAxis(InputRotationX, Vector3.up) * Vector3.forward;
        var characterLeft = Quaternion.AngleAxis(InputRotationX + 90, Vector3.up) * Vector3.forward;

        //look and run direction
        LookDirection = Quaternion.AngleAxis(InputRotationY, characterLeft) * characterForward;

            

        //set player values
        Player.Inputs.RunX = Input.GetAxisRaw("Horizontal");
        Player.Inputs.LookX = Input.GetAxisRaw("Horizontal");
        Player.Inputs.RunZ = Input.GetAxisRaw("Vertical");
        Player.Inputs.LookZ = Input.GetAxisRaw("Vertical");
        Player.Inputs.Jump = Input.GetKeyDown("space");
        if(Input.GetButtonDown("joystickButtonA"))
        Player.Inputs.Jump = Input.GetButtonDown("joystickButtonA");
            
        Player.Inputs.JoystickX = joystick.Horizontal;
        Player.Inputs.JoystickZ = joystick.Vertical;

        if (joystickAttack.Horizontal == 0 || joystickAttack.Vertical == 0)
        {
            Player.Inputs.JoystickXAttack = Input.GetAxis("HoriJoystick");
            Player.Inputs.JoystickZAttack = Input.GetAxis("VertJoystick");
        }
        else {
            Player.Inputs.JoystickXAttack = joystickAttack.Horizontal;
            Player.Inputs.JoystickZAttack = joystickAttack.Vertical;
        }

        mycolliderVehicle = Physics.OverlapSphere(transform.position, 1f, LayerMask.GetMask("Vehicle"));
        bool EnterVehicle = mycolliderVehicle.Length != 0;

        if (EnterVehicle && !mycolliderVehicle[0].name.Contains("Robot") && !Player.Inputs.angkatbox && mycolliderVehicle[0].GetComponent<Vehicle>().myHP!=0 && ((string)PhotonNetwork.LocalPlayer.CustomProperties["vehicle"]== mycolliderVehicle[0].name || mycolliderVehicle[0].GetComponent<Vehicle>().TempatDuduk.Count < mycolliderVehicle[0].GetComponent<Vehicle>().maxTempatDuduk))
        {
            
            if ((string)PhotonNetwork.LocalPlayer.CustomProperties["vehicle"] == mycolliderVehicle[0].name || mycolliderVehicle[0].GetComponent<Vehicle>().TempatDuduk.Count < mycolliderVehicle[0].GetComponent<Vehicle>().maxTempatDuduk)
            {
                GameObject.Find("ButtonCrack").transform.Find("GameObject").GetComponent<Image>().overrideSprite = Resources.Load<Sprite>("[UIFabrica]TrollNest_Free_v01/02.PNG/EnterVehicle");
                Player.Inputs.EnterVehicle = true;
            }
        }
        else if (!Player.Inputs.angkatbox)
        {
            GameObject.Find("ButtonCrack").transform.Find("GameObject").GetComponent<Image>().overrideSprite = Resources.Load<Sprite>("[UIFabrica]TrollNest_Free_v01/02.PNG/trollnest_checkbox-press");
            Player.Inputs.EnterVehicle = false;
        }
    }

     private void LateUpdate()
     {
        //set camera values
        
        if (IsTouchOverGameObject())
        {
            return;
        }

        if (IsPointerOverUIObject())
        {
            return;
        }

        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                ClickMouseTouch(touch.position);
            }
        }

        if (Input.GetMouseButtonDown(0))
            {
                ClickMouseTouch(Input.mousePosition);

            }

        if (Input.GetKeyDown(KeyCode.Q) || Input.GetButtonDown("joystickButtonB"))
        {
            MyPickUp();
        }

        if (Input.GetKeyDown(KeyCode.E) || Input.GetButtonDown("joystickButtonY"))
        {
            Cracking();
        }

        if (Input.GetKeyDown(KeyCode.G) || Input.GetButtonDown("joystickButtonX"))
        {
            ThrowWeapon();
        }





    }

    public void Cracking()
    {
        if(!(bool)PhotonNetwork.LocalPlayer.CustomProperties["dead"])
        if (Player.Inputs.angkatbox)
        {
            ThrowUp();
        }else
        if (Player.Inputs.EnterVehicle)
        {
            EnterVehicle();
        }
        else
        CrackUp();
    }

    public void EnterVehicle()
    {
        if ((string)PhotonNetwork.LocalPlayer.CustomProperties["vehicle"] == "" && mycolliderVehicle[0].GetComponent<Vehicle>().TempatDuduk.Count< mycolliderVehicle[0].GetComponent<Vehicle>().maxTempatDuduk)
        {
            string myVehicleName = mycolliderVehicle[0].name;
            ExitGames.Client.Photon.Hashtable setRace = new ExitGames.Client.Photon.Hashtable();
            setRace.Add("weaponTemp", PhotonNetwork.LocalPlayer.CustomProperties["weapon"]);
            setRace.Add("weapontipeTemp", PhotonNetwork.LocalPlayer.CustomProperties["weapontipe"]);
            setRace.Add("needammoTemp", PhotonNetwork.LocalPlayer.CustomProperties["needammo"]);
            setRace.Add("aspdTemp", PhotonNetwork.LocalPlayer.CustomProperties["aspd"]);

            if (myVehicleName.Contains("tank"))
            {
                setRace.Add("weapon", "GunTank");
                setRace.Add("weapontipe", "range");
                setRace.Add("needammo", 0);
                setRace.Add("aspd", 20f);
            }
            setRace.Add("vehicle", myVehicleName);
            PhotonNetwork.LocalPlayer.SetCustomProperties(setRace, null);

            transform.Find("Player").GetComponent<Animator>().SetInteger("condition", 0);

            if (mycolliderVehicle[0].transform.Find("Honk")!=null && mycolliderVehicle[0].GetComponent<Vehicle>().TempatDuduk.Count==0)
            ButtonThrowWeapon.transform.Find("GameObject").GetComponent<Image>().overrideSprite = Resources.Load<Sprite>("[UIFabrica]TrollNest_Free_v01/02.PNG/Honk");
            else ButtonThrowWeapon.SetActive(false);

            ButtonJump.SetActive(false);
            ButtonPickUpBox.SetActive(false);

            GameObject.Find(myVehicleName).GetComponent<PhotonView>().RPC("vehicleOwned", RpcTarget.All, name);
            GetComponent<PhotonView>().RPC("EnterVehicleRPC", RpcTarget.All, name, myVehicleName);
        }
        else
        {
            string myVehicleName = (string)PhotonNetwork.LocalPlayer.CustomProperties["vehicle"];
            ExitGames.Client.Photon.Hashtable setRace = new ExitGames.Client.Photon.Hashtable();
            setRace.Add("weapon", PhotonNetwork.LocalPlayer.CustomProperties["weaponTemp"]);
            setRace.Add("weapontipe", PhotonNetwork.LocalPlayer.CustomProperties["weapontipeTemp"]);
            setRace.Add("needammo", PhotonNetwork.LocalPlayer.CustomProperties["needammoTemp"]);
            setRace.Add("aspd", PhotonNetwork.LocalPlayer.CustomProperties["aspdTemp"]);
            setRace.Add("vehicle", "");
            PhotonNetwork.LocalPlayer.SetCustomProperties(setRace, null);
            GetComponent<PhotonView>().RPC("OutVehicleRPC", RpcTarget.All, name, myVehicleName);
            GameObject.Find(myVehicleName).GetComponent<PhotonView>().RPC("vehicleRemove", RpcTarget.All, name);
            ButtonThrowWeapon.transform.Find("GameObject").GetComponent<Image>().overrideSprite = Resources.Load<Sprite>("[UIFabrica]TrollNest_Free_v01/02.PNG/ThrowWeapon");
            ButtonThrowWeapon.SetActive(true);
            Player.Inputs.EnterVehicle = false;
        }
        
    }

    public void ThrowUp()
    {
        GameObject.Find("ButtonCrack").transform.Find("GameObject").GetComponent<Image>().overrideSprite = Resources.Load<Sprite>("[UIFabrica]TrollNest_Free_v01/02.PNG/trollnest_checkbox-press");
        string myboxname = (string)PhotonNetwork.LocalPlayer.CustomProperties["mybox"];
        GetComponent<PhotonView>().RPC("ThrowBoxRPC", RpcTarget.All, myboxname, name);
        ExitGames.Client.Photon.Hashtable setRace = new ExitGames.Client.Photon.Hashtable();
        setRace.Add("mybox", "");
        PhotonNetwork.LocalPlayer.SetCustomProperties(setRace, null);
    }

    public void ThrowWeapon()
    {
        int slot = (int)PhotonNetwork.LocalPlayer.CustomProperties["weaponslotnumber"];
        string myweapon = (string)PhotonNetwork.LocalPlayer.CustomProperties["weaponslot" + slot];
        if (myweapon != "hand" && !Player.Inputs.angkatbox && !Player.Inputs.EnterVehicle && !(bool)PhotonNetwork.LocalPlayer.CustomProperties["dead"])
        {
            float angleku = Mathf.Round(transform.eulerAngles.y * 1000f) / 1000f;
            float tambahX = Mathf.Round(Mathf.Sin(angleku * Mathf.Deg2Rad) * 100000) / 100000;
            float tambahZ = Mathf.Round(Mathf.Cos(angleku * Mathf.Deg2Rad) * 100000) / 100000;

            GetComponent<PhotonView>().RPC("ThrowWeaponRPC", RpcTarget.All, myweapon, tambahX, tambahZ);

            
            ExitGames.Client.Photon.Hashtable setRace = new ExitGames.Client.Photon.Hashtable();
            setRace.Add("weaponslot"+ slot, "hand");
            setRace.Add("weaponslot" + slot+"weapontipe", "melee");
            setRace.Add("weaponslot" + slot + "aspd", 20f);
            setRace.Add("weaponslot" + slot+"damage", 0);
            setRace.Add("weaponslot" + slot+"needammo", 0);

            if (slot == 1)
            {
                GameSetupController.instance.SlotWeapon1.transform.Find("GameObject").GetComponent<Image>().color = Color.gray;
                GameSetupController.instance.SlotWeapon1.transform.Find("GameObject").GetComponent<Image>().overrideSprite = null;
            }
            else if (slot == 2)
            {
                GameSetupController.instance.SlotWeapon2.transform.Find("GameObject").GetComponent<Image>().color = Color.gray;
                GameSetupController.instance.SlotWeapon2.transform.Find("GameObject").GetComponent<Image>().overrideSprite = null;
            }
            else if (slot == 3)
            {
                GameSetupController.instance.SlotWeapon3.transform.Find("GameObject").GetComponent<Image>().color = Color.gray;
                GameSetupController.instance.SlotWeapon3.transform.Find("GameObject").GetComponent<Image>().overrideSprite = null;
            }
            

            PhotonNetwork.LocalPlayer.SetCustomProperties(setRace, null);
            GameSetupController.instance.setWeapon(slot);

        }else if(!Player.Inputs.angkatbox && Player.Inputs.movingWithVehicle)
        {
            GetComponent<PhotonView>().RPC("VehicleHonk", RpcTarget.All, name);
        }
    }

    public void PickUp()
    {
        if (!Player.Inputs.Angkat)
        {
            GameObject[] gos = GameObject.FindGameObjectsWithTag("Obstacle");
            GameObject closest;
            var distance = 2f;
            var tempangle = 46f;
            var position = transform.position;

            // Iterate through them and find the closest one
            foreach (GameObject go in gos)
            {
                if (go.transform != transform)
                {
                    var diff = (go.transform.position - position);
                    var curDistance = diff.sqrMagnitude;
                    float angle = Vector3.Angle(transform.forward, diff);
                    if (curDistance < distance && Mathf.Abs(angle) < tempangle)
                    {
                        closest = go;
                        distance = curDistance;

                        var count = 0;
                        foreach (Player player in PhotonNetwork.PlayerList) //loop through each player and create a player listing
                        {
                            if ((string)player.CustomProperties["mybox"] == closest.name) count++;
                        }

                        if (count == 0)
                        {
                            GetComponent<PhotonView>().RPC("myBoxRPC", RpcTarget.All, closest.name, false, true,false);
                            Player.Inputs.boxname = closest.name;
                            ExitGames.Client.Photon.Hashtable setRace = new ExitGames.Client.Photon.Hashtable();
                            setRace.Add("mybox", Player.Inputs.boxname);
                            PhotonNetwork.LocalPlayer.SetCustomProperties(setRace, null);
                            Player.Inputs.angkatbox = true;
                            Player.Inputs.Angkat = true;
                            GameObject.Find("ButtonCrack").transform.Find("GameObject").GetComponent<Image>().overrideSprite = Resources.Load<Sprite>("[UIFabrica]TrollNest_Free_v01/02.PNG/ThrowBox");

                            int nourut = GameObject.Find(Player.Inputs.boxname).GetComponent<Cube>().getNoUrut();
                            if (nourut <= 8 && nourut >= 1)
                            {
                                GameObject.Find(GameObject.Find(Player.Inputs.boxname).GetComponent<Cube>().getBoxInStasiun()).GetComponent<PhotonView>().RPC("setRemoveCube", RpcTarget.All, nourut, Player.Inputs.boxname);
                            }

                        }
                        else
                        {
                            Debug.Log("GA BISA AMBIL PNYA ORG");
                        }

                    }
                }
            }
        }
        


    }

    public void CrackUp()
    {
        GameObject[] gos = GameObject.FindGameObjectsWithTag("Obstacle");
        GameObject closest;
        var distance = 2f;
        var tempangle = 46f;
        var position = transform.position;

        // Iterate through them and find the closest one
        foreach (GameObject go in gos)
        {
            if (go.transform != transform)
            {
                var diff = (go.transform.position - position);
                var curDistance = diff.sqrMagnitude;
                float angle = Vector3.Angle(transform.forward, diff);
                if (curDistance < distance && Mathf.Abs(angle) < tempangle)
                {
                    closest = go;
                    distance = curDistance;

                    var count = 0;
                    foreach (Player player in PhotonNetwork.PlayerList) //loop through each player and create a player listing
                    {
                        if ((string)player.CustomProperties["mybox"] == closest.name) count++;
                    }

                    if (count == 0 && !Player.Inputs.angkatbox)
                    {
                        Vector3 posisi = GameObject.Find(closest.name).transform.position;
                        GetComponent<PhotonView>().RPC("crackBoxRPC", RpcTarget.All, closest.name, posisi);
                        Debug.Log("CRACKING BOX");
                        Player.Inputs.Crack = false;
                    }
                    else
                    {
                        Debug.Log("GA BISA CRACK PNYA ORG");
                    }

                }
            }
        }
        



    }

    public void RealJump()
    {
        Player.Inputs.Jump = true;
    }

    public void MyPickUp()
    {
        if (!Player.Inputs.angkatbox)
        {
            PickUp();
        }
        else
        {
            ExitGames.Client.Photon.Hashtable setRace = new ExitGames.Client.Photon.Hashtable();
            setRace.Add("mybox", "");
            PhotonNetwork.LocalPlayer.SetCustomProperties(setRace, null);
            GetComponent<PhotonView>().RPC("myBoxRPC", RpcTarget.All, Player.Inputs.boxname, true, false,true);

            Player.Inputs.Angkat = false;
            Player.Inputs.angkatbox = false;
            GameObject.Find("ButtonCrack").transform.Find("GameObject").GetComponent<Image>().overrideSprite = Resources.Load<Sprite>("[UIFabrica]TrollNest_Free_v01/02.PNG/trollnest_checkbox-press");
        }
    }

    public void ClickMouseTouch(Vector3 clikPos)
    {
        Plane playerPlane = new Plane(Vector3.up, transform.position);
        Ray ray = Camera.main.ScreenPointToRay(clikPos);
        float hitDistance = 0.00f;

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f, grounds))
        {
            Vector3 targetPoint = hit.point;
            Player.Inputs.pmrPos = new Vector3(targetPoint.x, targetPoint.y, targetPoint.z);
            GameObject.Find("PlayerMovePoint").transform.LookAt(new Vector3(Player.Inputs.pmrPos.x,transform.position.y,Player.Inputs.pmrPos.z));
            Player.Inputs.pmrRot = GameObject.Find("PlayerMovePoint").transform.eulerAngles;
            Player.Inputs.moving = true;

        }


        Vector3 camRay = Camera.main.ScreenToWorldPoint(clikPos);
        Vector2 ms = new Vector2(camRay.x, camRay.y);
        RaycastHit2D hits = Physics2D.Raycast(ms, Vector2.zero);
        if (hits.collider != null)
        {
            Debug.Log(hits.transform.gameObject.name);
        }

        if (playerPlane.Raycast(ray, out hitDistance))
        {
            /*Vector3 targetPoint = ray.GetPoint(hitDistance);
            moving = true;

            pmr.transform.position = new Vector3(targetPoint.x, targetPoint.y, targetPoint.z);
            */
        }


    }

    private bool IsTouchOverGameObject()
    {
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
                eventDataCurrentPosition.position = new Vector2(touch.position.x, touch.position.y);
                List<RaycastResult> results = new List<RaycastResult>();
                EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
                if (results.Count > 0)
                {
                    if (results[0].gameObject.layer == 10) return false;
                    else
                        return results.Count > 0;
                }
            }
        }
        return false;
    }

    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

        if (results.Count > 0)
        {
            if (results[0].gameObject.layer == 10) return false;
            else
                return results.Count > 0;
        }
        return false;
    }

    

}
