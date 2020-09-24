using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Projectile : MonoBehaviourPunCallbacks
{
    Player1 player1;
    [SerializeField]
    public int damage=0;
    [SerializeField]
    public string owner="";
    [SerializeField]
    public string target="";
    [SerializeField]
    public bool moving = false;
    [SerializeField]
    public bool destroyed = false;
    // Start is called before the first frame update

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        StartCoroutine(kadaluarsa());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        if (moving && GameObject.Find(target) != null)
        {
            Vector3 targetPos = new Vector3(GameObject.Find(target).transform.position.x, GameObject.Find(target).transform.position.y + 0.5f, GameObject.Find(target).transform.position.z);

            GameObject.Find("PMRbullet").transform.LookAt(targetPos);
            Vector3 targetRot = GameObject.Find("PMRbullet").transform.eulerAngles;
            Quaternion lookRot = Quaternion.Euler(targetRot);

            transform.LookAt(targetPos);
            transform.position = Vector3.MoveTowards(transform.position, targetPos, 4 * Time.deltaTime);

            //Quaternion lookRot = Quaternion.Euler(targetRot);
            //transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, 12 * Time.deltaTime);
        }
        else if(moving)GetComponent<Rigidbody>().useGravity = true;
    }

    [PunRPC]
    void moveToTarget(string NamaTarget)
    {
        moving = true;
        target = NamaTarget;
    }

    [PunRPC]
    void itemrpc(string nama, string namaParent, Vector3 posisi, string nickname)
    {
        owner = nickname;
        name = nama;
        transform.parent = GameObject.Find(namaParent).transform;
        //transform.LookAt(new Vector3(posisi.x, posisi.y + 0.5f, posisi.z));
        Random rnd = new Random();
        if (name.Contains("GunU"))
        {
            int randomsound = Random.Range(1, 4);
            AudioSource audio = GameObject.Find("GunUpgrade" + randomsound).GetComponent<AudioSource>();
            GameObject.Find("GunUpgrade" + randomsound).transform.position = transform.position;
            audio.Play();
            GetComponent<Rigidbody>().AddForce(GetComponent<Rigidbody>().transform.forward * 50, ForceMode.VelocityChange);
            damage = 10;
        }else if (name.Contains("GunTank"))
        {
            AudioSource audio = GameObject.Find("tankshot").GetComponent<AudioSource>();
            GameObject.Find("tankshot").transform.position = transform.position;
            audio.Play();
            GetComponent<Rigidbody>().AddForce(GetComponent<Rigidbody>().transform.forward * 20, ForceMode.VelocityChange);
            damage = 15;
        }else if (name.Contains("GunTurret"))
        {
            AudioSource audio = GameObject.Find("tankshot").GetComponent<AudioSource>();
            GameObject.Find("tankshot").transform.position = transform.position;
            audio.Play();
            GetComponent<Rigidbody>().useGravity = false;
            //GetComponent<Rigidbody>().AddForce(GetComponent<Rigidbody>().transform.forward * 20, ForceMode.VelocityChange);
            damage = 10;
        }
        else if (name.Contains("GunRocket"))
        {
            AudioSource audio = GameObject.Find("tankshot").GetComponent<AudioSource>();
            GameObject.Find("tankshot").transform.position = transform.position;
            audio.Play();
            GetComponent<Rigidbody>().AddForce(GetComponent<Rigidbody>().transform.forward * 20, ForceMode.VelocityChange);
            damage = 15;
        }
        else if (name.Contains("GunFlame"))
        {
            AudioSource audio = GameObject.Find("flameshot").GetComponent<AudioSource>();
            GameObject.Find("flameshot").transform.position = transform.position;
            if(!audio.isPlaying)
            audio.Play();
            GetComponent<Rigidbody>().AddForce(GetComponent<Rigidbody>().transform.forward * 5, ForceMode.VelocityChange);
            damage = 2;
        }
        else if (name.Contains("GunMolotov") && !name.Contains("Fire"))
        {
            AudioSource audio = GameObject.Find("Swosh").GetComponent<AudioSource>();
            GameObject.Find("Swosh").transform.position = transform.position;
            if (!audio.isPlaying)
                audio.Play();
            GetComponent<Rigidbody>().AddForce(GetComponent<Rigidbody>().transform.up * 7, ForceMode.VelocityChange);
            GetComponent<Rigidbody>().AddForce(GetComponent<Rigidbody>().transform.forward * 5, ForceMode.VelocityChange);
            damage = 2;
        }
        else if(!name.Contains("Fire"))
        {
            int randomsound = Random.Range(1, 3);
            AudioSource audio = GameObject.Find("Shot" + randomsound).GetComponent<AudioSource>();
            GameObject.Find("Shot" + randomsound).transform.position = transform.position;
            audio.Play();
            GetComponent<Rigidbody>().AddForce(GetComponent<Rigidbody>().transform.forward * 20, ForceMode.VelocityChange);
            damage = 7;
        }
    }

    [PunRPC]
    void DestroyItem()
    {
        destroyed = true;
        PhotonNetwork.Destroy(gameObject);
    }

    [PunRPC]
    void createExplosion()
    {
        AudioSource explosion = GameObject.Find("tankexplosion").GetComponent<AudioSource>();
        GameObject.Find("tankexplosion").transform.position = transform.position;
        explosion.Play();

        if (PhotonNetwork.IsMasterClient)
        {
            Vector3 explosionPos = transform.position;
            Collider[] colliders = Physics.OverlapSphere(explosionPos, 2f);
            foreach (Collider hit in colliders)
            {
                Rigidbody rb = hit.GetComponent<Rigidbody>();

                if (rb != null && !rb.isKinematic)
                {
                    if(!hit.name.Contains("Fire"))
                    rb.AddExplosionForce(rb.mass * 100, explosionPos, 2f, 3.0F);
                    if (hit.tag == "Player")
                    {
                        string myNickName = hit.GetComponent<Player1>().getMyNickName(hit.name);
                        foreach (Player player in PhotonNetwork.PlayerList) //loop through each player and create a player listing
                        {
                            if (player.NickName == myNickName)
                            {
                                int randomsound = Random.Range(1, 3);
                                AudioSource audio = GameObject.Find("Punch" + randomsound).GetComponent<AudioSource>();
                                GameObject.Find("Punch" + randomsound).transform.position = hit.transform.position;
                                audio.Play();

                                int damage = 5;
                                int myhp = (int)player.CustomProperties["health"];
                                int mymaxhp = (int)player.CustomProperties["maxhealth"];
                                if (myhp - damage >= 0) myhp -= damage;
                                else myhp = 0;

                                ExitGames.Client.Photon.Hashtable setHP = new ExitGames.Client.Photon.Hashtable();
                                setHP.Add("health", myhp);
                                player.SetCustomProperties(setHP, null);
                                if (!(bool)player.CustomProperties["dead"])
                                    hit.GetComponent<PhotonView>().RPC("attackingPlayer", RpcTarget.All, hit.name, myhp, mymaxhp, player);
                                if (myhp == 0)
                                {
                                    ExitGames.Client.Photon.Hashtable setDead = new ExitGames.Client.Photon.Hashtable();
                                    setDead.Add("dead", true);
                                    player.SetCustomProperties(setDead, null);
                                }
                                break;
                            }
                        }
                    }

                }

            }
        }
        
    }

    public IEnumerator kadaluarsa()
    {
        yield return new WaitForSeconds(5);
        if (name.Contains("BulletGunTank") || name.Contains("Turret") || name.Contains("Rocket"))
            GetComponent<PhotonView>().RPC("createExplosion", RpcTarget.All);
        if (PhotonNetwork.IsMasterClient)
        {
            if (name.Contains("BulletGunTank") || name.Contains("Turret") || name.Contains("Rocket"))
            {
                GameObject go = PhotonNetwork.InstantiateSceneObject(Path.Combine("Models/Fire", "fx_fire_ball_b"), new Vector3(transform.position.x, transform.position.y, transform.position.z), transform.rotation);
                go.name = "FireExplode (" + go.GetPhotonView().ViewID + ")";
                go.GetComponent<PhotonView>().RPC("itemrpc", RpcTarget.All, go.name, "Projectile Spawn");
            } 
            PhotonNetwork.Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (PhotonNetwork.IsMasterClient)
            if (collision.collider.name != owner && !name.Contains("Fire") && (name.Contains("BulletGunTank") || name.Contains("Turret") || name.Contains("Rocket") || name.Contains("GunMolotov")))
            {
                GameObject go = PhotonNetwork.InstantiateSceneObject(Path.Combine("Models/Fire", "fx_fire_ball_b"), new Vector3(transform.position.x, transform.position.y, transform.position.z), transform.rotation);
                go.name = "FireExplode (" + go.GetPhotonView().ViewID + ")";
                go.GetComponent<PhotonView>().RPC("itemrpc", RpcTarget.All, go.name, "Projectile Spawn");
                if(!destroyed && !name.Contains("Molotov"))
                GetComponent<PhotonView>().RPC("createExplosion", RpcTarget.All);

                if (name.Contains("Molotov"))
                {
                    for(int i = 0; i < 5;i++)
                    {
                       
                        GameObject fire = PhotonNetwork.InstantiateSceneObject(Path.Combine("Models", "bullet_GunMolotovFire"), new Vector3(transform.position.x + Random.Range(-2f, 2f), transform.position.y + Random.Range(0.5f, 3f), transform.position.z + Random.Range(-2f, 2f)), Quaternion.identity);
                        fire.name = "BulletGunMolotovFire (" + go.GetPhotonView().ViewID + ")";
                        fire.GetComponent<PhotonView>().RPC("itemrpc", RpcTarget.All, fire.name, "Projectile Spawn", transform.position, owner);
                    }
                }

                if (collision.collider.tag.Equals("Untagged")) PhotonNetwork.Destroy(gameObject);
            }

        if (PhotonNetwork.IsMasterClient)
            if ((collision.collider.name!=owner || name.Contains("MolotovFire")) && (collision.relativeVelocity.magnitude > 2 || name.Contains("Turret") || name.Contains("MolotovFire")))
            {
                foreach (Player player in PhotonNetwork.PlayerList) //loop through each player and create a player listing
                {
                    if ("Player ("+player.NickName+")" == collision.collider.name)
                    {
                        ExitGames.Client.Photon.Hashtable setHP = new ExitGames.Client.Photon.Hashtable();
                        int myhp = (int)player.CustomProperties["health"] - damage;
                        int mymaxhp = (int)player.CustomProperties["maxhealth"];
                        if (myhp <= 0)
                        {
                            myhp = 0;
                        }
                        setHP.Add("health", myhp);
                        player.SetCustomProperties(setHP, null);

                        if (!(bool)player.CustomProperties["dead"])
                            collision.collider.GetComponent<PhotonView>().RPC("attackingPlayer", RpcTarget.All, collision.collider.name, myhp, mymaxhp, player);
                        if (myhp == 0)
                        {
                            ExitGames.Client.Photon.Hashtable setDead = new ExitGames.Client.Photon.Hashtable();
                            setDead.Add("dead", true);
                            player.SetCustomProperties(setDead, null);
                        }

                        float myhpfloat = (float)myhp / (int)player.CustomProperties["maxhealth"];
                        collision.collider.GetComponent<PhotonView>().RPC("refreshBar", player, myhpfloat, myhp);
                        collision.collider.GetComponent<PhotonView>().RPC("soundpunch2", RpcTarget.All, "Player (" + player.NickName + ")");

                        PhotonNetwork.Destroy(gameObject);
                        break;
                    }
                }

                

            }

        

    }

}
