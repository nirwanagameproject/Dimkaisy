using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Networking;

static public class AssetBundleManager
{
    // A dictionary to hold the AssetBundle references
    static private Dictionary<string, AssetBundleRef> dictAssetBundleRefs;
    static public float progressdownload;
    static public UnityWebRequest mydownload;
    static AssetBundleManager()
    {
        dictAssetBundleRefs = new Dictionary<string, AssetBundleRef>();
    }


    // Class with the AssetBundle reference, url and version
    private class AssetBundleRef
    {
        public AssetBundle assetBundle = null;
        public int version;
        public string url;
        public AssetBundleRef(string strUrlIn, int intVersionIn)
        {
            url = strUrlIn;
            version = intVersionIn;
        }
    };
    // Get an AssetBundle
    public static AssetBundle getAssetBundle(string url, int version, string namefile)
    {
        string keyName = url + version.ToString();
        string dataFileName = namefile;
        string tempPath = Path.Combine(Application.persistentDataPath, "AssetData");
        tempPath = Path.Combine(tempPath, dataFileName + ".unity3d");

        if(PlayerPrefs.HasKey(namefile + "Version"))
        if (PlayerPrefs.GetInt(namefile + "Version") != version) return null;

        if (File.Exists(tempPath))
        {
            //abRef.assetBundle = AssetBundle.LoadFromFile(tempPath);
            //dictAssetBundleRefs.Add(keyName, abRef);
            //if (dictAssetBundleRefs.TryGetValue(keyName, out abRef))
            if (dictAssetBundleRefs.ContainsKey(keyName))
                return dictAssetBundleRefs[keyName].assetBundle;
            else
            {
                AssetBundleRef abRef = new AssetBundleRef(url, version);
                abRef.assetBundle = AssetBundle.LoadFromFile(tempPath);
                dictAssetBundleRefs.Add(keyName, abRef);
                PlayerPrefs.SetInt(namefile + "Version",version);
                return dictAssetBundleRefs[keyName].assetBundle;
            }
           // else
           //    return null;
        }
        else return null;
        
    }
    // Download an AssetBundle
    public static IEnumerator downloadAssetBundle(string url, int version, string dataFileName)
    {
        mydownload = null;
        string keyName = url + version.ToString();
        Debug.Log("go");

        Debug.Log("masuk");
        while (!Caching.ready)
            yield return null;
        Debug.Log("masuk woi");
        mydownload = UnityWebRequest.Get(url);
        mydownload.certificateHandler = new AcceptAllCertificatesSignedWithASpecificKeyPublicKey();
        DownloadHandler handle = mydownload.downloadHandler;
        yield return mydownload.Send();
        if (!string.IsNullOrEmpty(mydownload.error))
        {
            Debug.Log(mydownload.error);
            yield return null;
        }
        AssetBundleRef abRef = new AssetBundleRef(url, version);



        Debug.Log(mydownload.downloadedBytes);

        string tempPath = Path.Combine(Application.persistentDataPath, "AssetData");
        tempPath = Path.Combine(tempPath, dataFileName + ".unity3d");
        PlayerPrefs.SetInt(dataFileName + "Version", version);
        
        //Save
        save(handle.data, tempPath);

        abRef.assetBundle = AssetBundle.LoadFromFile(tempPath);
        dictAssetBundleRefs.Add(keyName, abRef);
        PlayerPrefs.SetString("ActScene", abRef.assetBundle.GetAllScenePaths()[0]);

    }

    public static void save(byte[] data, string path)
    {
        //Create the Directory if it does not exist
        if (!Directory.Exists(Path.GetDirectoryName(path)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
        }

        try
        {
            File.WriteAllBytes(path, data);
            Debug.Log("Saved Data to: " + path.Replace("/", "\\"));
        }
        catch (Exception e)
        {
            Debug.LogWarning("Failed To Save Data to: " + path.Replace("/", "\\"));
            Debug.LogWarning("Error: " + e.Message);
        }
    }

    // Unload an AssetBundle
    public static void Unload(string url, int version, bool allObjects)
    {
        string keyName = url + version.ToString();
        AssetBundleRef abRef;
        if (dictAssetBundleRefs.TryGetValue(keyName, out abRef))
        {
            abRef.assetBundle.Unload(allObjects);
            abRef.assetBundle = null;
            dictAssetBundleRefs.Remove(keyName);
        }
    }
}