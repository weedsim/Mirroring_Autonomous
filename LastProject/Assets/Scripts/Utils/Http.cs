using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using static System.Net.WebRequestMethods;

public class Http
{

    public static IEnumerator Post(string URI, string playerKey, string jsonBody, Action<string> onSuccessCallback, Action<string> onErrorCallback)
    {
        using UnityWebRequest request = UnityWebRequest.Post(URI, jsonBody);
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonBody);
        request.uploadHandler = new UploadHandlerRaw(jsonToSend);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        if (playerKey != null)
        {
            request.SetRequestHeader("Cookie", "SESSION=" + playerKey);
        }
        yield return request.SendWebRequest();

        if (request.error != null)
        {
            onErrorCallback(request.error);
        }
        else
        {
            onSuccessCallback(Encoding.Default.GetString(request.downloadHandler.data));
        }
        request.Dispose();
    }

    public static IEnumerator Get(string URI, string playerKey, Dictionary<string, string> parameters, Action<string> onSuccessCallback, Action<string> onErrorCallback)
    {
        var urlBuilder = new System.Text.StringBuilder(256);
        urlBuilder.Append(URI);
        if(parameters != null)
        {
            urlBuilder.Append("?");

            foreach (string key in parameters.Keys)
            {
                urlBuilder.Append(key);
                urlBuilder.Append("=");
                urlBuilder.Append(parameters[key]);
                urlBuilder.Append("&");
            }
            urlBuilder.Remove(urlBuilder.Length - 1, 1);
        }
        Debug.Log(urlBuilder.ToString());
        using UnityWebRequest request = UnityWebRequest.Get(urlBuilder.ToString());
        request.downloadHandler = new DownloadHandlerBuffer();
        if(playerKey != null)
        {
            request.SetRequestHeader("Cookie", "SESSION=" + playerKey);
        }
        yield return request.SendWebRequest();

        if (request.error != null)
        {
            onErrorCallback(request.error);
        }
        else
        {
            onSuccessCallback(Encoding.Default.GetString(request.downloadHandler.data));
        }
        request.Dispose();
    }





}
