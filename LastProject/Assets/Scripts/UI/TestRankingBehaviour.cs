using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Text;

public class TestRankingBehaviour : AbstractRankingBehaviour
{

    public TMP_InputField completeTime;
    public TMP_InputField mapId;

    public TMP_Text textField;

    public void Submit()
    {
        long completeTimeValue = long.Parse(completeTime.text);
        int mapIdValue = int.Parse(mapId.text);


        RankingUnitInfo[] ruis = new RankingUnitInfo[]{
            new RankingUnitInfo(1, "TestNickname1", 1),
            new RankingUnitInfo(2, "TestNickname2", 2),
            new RankingUnitInfo(3, "TestNickname3", 3),
            new RankingUnitInfo(4, "TestNickname4", 4)
        };


        RankingInfo rankingInfo = new RankingInfo(mapIdValue, completeTimeValue, ruis);


        RankingHttpManager.Instance.PostRanking(rankingInfo, (string result) =>
            {
                Debug.Log(result);
            }
        );
    }

    public override void Refresh(IEnumerable<RankingInfo> rankingInfos)
    {
        StringBuilder textBuilder = new();
        foreach(RankingInfo info in rankingInfos)
        {
            textBuilder.Append("[CompleteTime : " + info.completeTime + ", MapId : " + info.mapId + "]\n");
            foreach(RankingUnitInfo rui in info.rankingUnits)
            {
                textBuilder.Append("    " + rui.nickname + ":"  + CharacterClassManager.Instance.GetCharacterClassInfo(rui.characterClassId).CharacterClassName +"\n");
                
            }
        }
        textField.SetText(textBuilder.ToString());
    }
}
