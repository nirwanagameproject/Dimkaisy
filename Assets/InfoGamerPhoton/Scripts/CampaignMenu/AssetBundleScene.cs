

using System.Collections;
using UnityEngine;

public class AssetBundleScene : MonoBehaviour
{
    public string url;

    AssetBundle assetBundle;

    IEnumerator Start()
    {
        using (WWW www = new WWW(url))
        {
            yield return www;
            if (!string.IsNullOrEmpty(www.error))
            {
                Debug.LogError(www.error);
                yield break;
            }
            assetBundle = www.assetBundle;
            string[] scenes = assetBundle.GetAllScenePaths();

            foreach(string sceneName in scenes)
            {
                Debug.Log(sceneName);
            }
        }
    }
}