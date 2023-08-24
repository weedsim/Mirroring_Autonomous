using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using UnityEngine;
using UnityEngine.Networking;

public class RankingHttpManager : Singleton<RankingHttpManager>
{

    static string path = "/ranking";

    static int port = 8082;

    static string uri = ApiConfig.Host + ":" + port + path;

    public override void Initialize()
    {

    }

    public void PostRanking(RankingInfo rankingInfo, Action<string> callback, Action<string> errorCallback)
    {
        string json = JsonUtility.ToJson(rankingInfo);
        StartCoroutine(Http.Post(uri, AccountManager.PlayerKey, json, callback, errorCallback));
    }

    public void PostRanking(RankingInfo rankingInfo, Action<string> callback)
    {
        PostRanking(rankingInfo, callback, (string errorText) =>
            {
                Debug.Log(errorText);
            }
        );
    }

    public void GetRankingAll(int mapId, string nickname, int start, int count, Action<RankingInfo[]> callback, Action<string> errorCallback)
    {
        Dictionary<string, string> queryParams = new();
        queryParams["mapId"] = mapId.ToString();
        queryParams["nickname"] = nickname;
        queryParams["start"] = start.ToString();
        queryParams["count"] = count.ToString();
        StartCoroutine(Http.Get(uri, AccountManager.PlayerKey, queryParams, (string json) =>
            {
                HttpResponse<RankingInfo> response = JsonUtility.FromJson<HttpResponse<RankingInfo>>(json);
                callback(response.items);
            },
            errorCallback
        ));
    }

    public void GetRankingAll(int mapId, string nickname, int start, int count, Action<RankingInfo[]> callback)
    {
        GetRankingAll(mapId, nickname, start, count, callback, (string errorText) =>
        {
            Debug.Log(errorText);
        }
        );
    }






}
