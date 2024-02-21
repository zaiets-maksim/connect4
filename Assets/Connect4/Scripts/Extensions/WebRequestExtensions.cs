using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public static class WebRequestExtensions
{
    public static IEnumerator GetData<T> (this MonoBehaviour monoBehaviour, string url, Action<T> callback)
    {
        var www = new UnityWebRequest(url)
        {
            downloadHandler = new DownloadHandlerBuffer()
        };

        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.LogError($"Something went wrong {www.error}");
            yield break;
        }

        callback?.Invoke(JsonUtility.FromJson<T>(www.downloadHandler.text));
    }
}
