using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RankingUnitInfo
{
    public long uid;

    public string nickname;

    public int characterClassId;

    public RankingUnitInfo(long uid, string nickname, int characterClassId)
    {
        this.uid = uid;
        this.nickname = nickname;
        this.characterClassId = characterClassId;
    }
}
