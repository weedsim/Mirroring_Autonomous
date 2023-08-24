using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RankingInfo
{

    public int rank;

    public int mapId;

    public long completeTime;

    public RankingUnitInfo[] rankingUnits;

    public RankingInfo(int mapId, long completeTime, RankingUnitInfo[] rankingUnits)
    {
        this.mapId = mapId;
        this.completeTime = completeTime;
        this.rankingUnits = rankingUnits;
    }
}

