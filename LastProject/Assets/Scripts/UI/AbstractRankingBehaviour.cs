using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractRankingBehaviour : MonoBehaviour
{

    public abstract void Refresh(IEnumerable<RankingInfo> rankingInfos);

    public void Refresh()
    {
        RankingHttpManager.Instance.GetRankingAll(1, "", 0, 10, (RankingInfo[] rankings) =>
        {
            Refresh(rankings);
        });
    }

}
