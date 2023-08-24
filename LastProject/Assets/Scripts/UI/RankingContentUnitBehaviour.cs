using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class RankingContentUnitBehaviour : MonoBehaviour
{
    public TMP_Text RankingText;

    public GameObject RankingMemberUnitList;

    public RankingMemberUnitBehaviour rankingMemberUnitPrefab;

    public CompleteGameBehaviour CompleteGameBehaviour;

    public void SetRankingContentUnit(RankingInfo ri)
    {
        RankingText.SetText(ri.rank.ToString());
        foreach (Transform child in RankingMemberUnitList.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        foreach(RankingUnitInfo rui in ri.rankingUnits)
        {
            RankingMemberUnitBehaviour rankingMemberUnit = Instantiate(rankingMemberUnitPrefab);
            rankingMemberUnit.SetRankingInfo(rui);
            rankingMemberUnit.transform.SetParent(RankingMemberUnitList.transform);
        }
        CompleteGameBehaviour.SetGameCompleteTime(ri.completeTime);

    }

}
