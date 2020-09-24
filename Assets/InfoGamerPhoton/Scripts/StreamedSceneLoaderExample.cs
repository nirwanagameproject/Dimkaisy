using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class StreamedSceneLoaderExample : MonoBehaviour
{
    IEnumerator Start()
    {
        // Download compressed Scene. If version 5 of the file named "Streamed-Level1.unity3d" was previously downloaded and cached.
        // Then Unity will completely skip the download and load the decompressed Scene directly from disk.
        var download = UnityWebRequestAssetBundle.GetAssetBundle("http://206.189.93.226/dimkaisy/AssetBundles/act1.unity3d", 1);
        yield return download.SendWebRequest();

        // Handle error
        if (download.isNetworkError || download.isHttpError)
        {
            Debug.LogError(download.error);
            yield break;
        }

        // In order to make the Scene available from LoadLevel, we have to load the asset bundle.
        // The AssetBundle class also lets you force unload all assets and file storage once it is no longer needed.
        var bundle = DownloadHandlerAssetBundle.GetContent(download);

        // Load the level we have just downloaded
        Application.LoadLevel("Act1");
    }
}