using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccountHttpManager : Singleton<AccountHttpManager>
{

    static string path = "/account";

    static string path2 = "/account/id";

    static int port = 8081;

    static string uri = ApiConfig.Host + ":" + port + path;

    static string uri2 = ApiConfig.Host + ":" + port + path2;

    public override void Initialize(){}

    public void PostAccount(AccountInfo accountInfo, Action<AuthResponse> callback, Action<string> errorCallback)
    {
        string json = JsonUtility.ToJson(accountInfo);
        StartCoroutine(Http.Post(uri, AccountManager.PlayerKey, json, (string json) =>
        {
            AuthResponse response = JsonUtility.FromJson<AuthResponse>(json);
            callback(response);
        },
        errorCallback));
    }

    public void IdDuplicationCheck(string id, Action<AuthResponse> callback, Action<string> errorCallback)
    {
        Dictionary<string, string> query = new(){["id"] = id};
        StartCoroutine(Http.Get(uri2, AccountManager.PlayerKey, query, (string json) =>
        {
            AuthResponse response = JsonUtility.FromJson<AuthResponse>(json);
            callback(response);
        },
        errorCallback));
    }
}
