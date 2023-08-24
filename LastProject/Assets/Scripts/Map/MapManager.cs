using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : Singleton<MapManager>
{

    public Dictionary<int, MapInfo> MapInfos { get; private set; }

    bool _isInitialized;

    public bool UIRefreshNeeded { get; private set; }

    public bool IsInitialized()
    {
        return _isInitialized;
    }

    public void AnnounceUIRefreshed()
    {
        UIRefreshNeeded = false;
    }

    // Start is called before the first frame update
    public override void Initialize()
    {
        MapInfos = new Dictionary<int, MapInfo>();
        MapInfo mi = new()
        {
            MapId = 1,
            SceneIndex = 3,
            MapName = "Boss Raid"
        };
        MapInfos.Add(mi.MapId, mi);
        UIRefreshNeeded = true;
        _isInitialized = true;
    }

    public MapInfo GetMapInfo(int id)
    {
        return MapInfos[id];
    }

    public IEnumerable<MapInfo> GetAllMapInfo()
    {
        return MapInfos.Values;
    }

}
