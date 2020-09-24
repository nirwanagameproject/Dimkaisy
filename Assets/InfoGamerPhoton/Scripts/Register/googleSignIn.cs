using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using Google;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class googleSignIn : MonoBehaviour
{
    public Text infoText;
    public string webClientId;

    static public googleSignIn instance;
    static public bool udahlogin;
    static public bool loggedout;

    public FirebaseAuth auth;
    private GoogleSignInConfiguration configuration;

    public MyNewPlayer myObject;

    public GameObject notif;
    public GameObject notifloading;

    public string idtoken;

    private void Awake()
    {
    }

    void Start()
    {
        configuration = new GoogleSignInConfiguration { WebClientId = webClientId, RequestEmail = true, RequestIdToken = true };
        CheckFirebaseDependencies();
        myObject = new MyNewPlayer();
        if (PlayerPrefs.HasKey("Token")) notif.SetActive(true);
    }

    private void CheckFirebaseDependencies()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            
            if (task.IsCompleted)
            {
                if (task.Result == DependencyStatus.Available)
                {
                    auth = FirebaseAuth.DefaultInstance;
                    udahlogin = true;
                    if (PlayerPrefs.HasKey("Token")) Invoke("SignInWithGoogle", 0f);
                    Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
                    Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;
                }
                else
                    AddToInformation("Could not resolve all Firebase dependencies: " + task.Result.ToString());
                
            }
            else
            {
                AddToInformation("Dependency check was not completed. Error : " + task.Exception.Message);
                notifloading.SetActive(false);
            }
        });
    }

    public void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token)
    {
        UnityEngine.Debug.Log("Received Registration Token: " + token.Token);
    }

    public void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e)
    {
        UnityEngine.Debug.Log("Received a new message from: " + e.Message.From);
    }

    public void SignInWithGoogle() { if (!udahlogin) OnSignIn(); else OnSignIn(); notif.SetActive(true);
        AudioSource audio = GameObject.Find("Clicked").GetComponent<AudioSource>();
        audio.Play();
    }
    public void SignOutFromGoogle() { OnSignOut(); }

    private void OnSignIn()
    {
        AddToInformation("Calling SignIn");
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = false;
        GoogleSignIn.Configuration.RequestIdToken = true;
        

        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(OnAuthenticationFinished);
    }

    private void OnSignOut()
    {
        AddToInformation("Calling SignOut");
        auth.SignOut();
        GoogleSignIn.DefaultInstance.SignOut();
        FirebaseAuth.DefaultInstance.SignOut();
    }

    public void OnDisconnect()
    {
        AddToInformation("Calling Disconnect");
        GoogleSignIn.DefaultInstance.Disconnect();
    }

    internal void OnAuthenticationFinished(Task<GoogleSignInUser> task)
    {
        if (task.IsFaulted)
        {
            using (IEnumerator<Exception> enumerator = task.Exception.InnerExceptions.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    GoogleSignIn.SignInException error = (GoogleSignIn.SignInException)enumerator.Current;
                    AddToInformation("Got Error: " + error.Status + " " + error.Message);
                }
                else
                {
                    AddToInformation("Got Unexpected Exception?!?" + task.Exception);
                }
            }
            DisableNotif();
        }
        else if (task.IsCanceled)
        {
            AddToInformation("Canceled");
            DisableNotif();
        }
        else
        {
            AddToInformation("Welcome: " + task.Result.DisplayName + "!");
            AddToInformation("Email = " + task.Result.Email);
            //AddToInformation("Google ID Token = " + task.Result.IdToken);
            //StartCoroutine(LoginAccount(task.Result.UserId, task.Result.IdToken));
            SignInWithGoogleOnFirebase(task.Result.IdToken);
            
        }
        
    }

    private void SignInWithGoogleOnFirebase(string idToken)
    {
        Credential credential = GoogleAuthProvider.GetCredential(idToken, null);

        auth.SignInWithCredentialAsync(credential).ContinueWith(task =>
        {
            AggregateException ex = task.Exception;
            if (ex != null)
            {
                if (ex.InnerExceptions[0] is FirebaseException inner && (inner.ErrorCode != 0))
                    AddToInformation("\nError code = " + inner.ErrorCode + " Message = " + inner.Message);
                DisableNotif();
            }
            else
            {
                AddToInformation("PhoneNo = " + auth.CurrentUser.PhoneNumber);
                AddToInformation("photo = " + auth.CurrentUser.PhotoUrl);
                AddToInformation("Sign In Successful.");

                idtoken = idToken;
                Invoke("CekToken",1f);
            }
        });
    }

    private void CekToken()
    {
        Firebase.Auth.FirebaseUser user = auth.CurrentUser;
        user.TokenAsync(true).ContinueWith(task => {
            if (task.IsCanceled)
            {
                DisableNotif();
                Debug.LogError("TokenAsync was canceled.");
                return;
            }

            if (task.IsFaulted)
            {
                DisableNotif();
                Debug.LogError("TokenAsync encountered an error: " + task.Exception);
                return;
            }

            string mytoken = task.Result;

            StartCoroutine(LoginAccount(idtoken, mytoken));
        });
    }

    public void OnSignInSilently()
    {
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = false;
        GoogleSignIn.Configuration.RequestIdToken = true;
        AddToInformation("Calling SignIn Silently");

        GoogleSignIn.DefaultInstance.SignInSilently().ContinueWith(OnAuthenticationFinished);
    }

    public void OnGamesSignIn()
    {
        AddToInformation("Calling Games SignIn");
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = true;
        GoogleSignIn.Configuration.RequestIdToken = false;

        

        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(OnAuthenticationFinished);
    }

    public IEnumerator LoginAccount(string clientid, string tokenid)
    {
        //string LoginUrl = "http://192.168.0.103/dimkaisy/register.php";
        string LoginUrl = MyConnection.Instance.LoginUrl + "dimkaisy/logingoogle.php";
        WWWForm Form = new WWWForm();
        Form.AddField("client_id", clientid);
        Form.AddField("token", tokenid);

        using (UnityWebRequest www = UnityWebRequest.Post(LoginUrl, Form))
        {
            www.certificateHandler = new AcceptAllCertificatesSignedWithASpecificKeyPublicKey();
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                notifloading.SetActive(false);
            }
            else
            {
                DisableNotif();
                Debug.Log("Form upload complete! " + www.downloadHandler.text);
                infoText.text = www.downloadHandler.text;
                myObject = JsonUtility.FromJson<MyNewPlayer>(www.downloadHandler.text);
                //auth.SignOut();
                //SignOutFromGoogle();
                infoText.text = myObject.message;
                if (myObject.message == "success")
                {
                    PlayerPrefs.SetString("Username", myObject.username);
                    PlayerPrefs.SetString("Password", myObject.password);
                    PlayerPrefs.SetString("Character", "" + myObject.character);
                    PlayerPrefs.SetString("Token", myObject.token);

                    ExitGames.Client.Photon.Hashtable setChar = new ExitGames.Client.Photon.Hashtable();
                    setChar.Add("character", myObject.character);
                    PhotonNetwork.LocalPlayer.SetCustomProperties(setChar, null);

                    SceneManager.LoadScene("UserMenu");

                }else
                if (myObject.message == "success2")
                {
                    PlayerPrefs.SetString("Token", myObject.token);

                    SceneManager.LoadScene("GoogleLogin");
                }
                else
                {
                    notifloading.SetActive(false);
                }
                
            }
        }
        StopCoroutine(LoginAccount(clientid, tokenid));

    }

    public void DisableNotif()
    {
        notif.SetActive(false);
        notifloading.SetActive(true);
    }

    private void AddToInformation(string str) { infoText.text += "\n" + str; }
}


[Serializable]
public class MyNewPlayer
{
    public String username;
    public String password;
    public String token;
    public String character;
    public String message;
}
