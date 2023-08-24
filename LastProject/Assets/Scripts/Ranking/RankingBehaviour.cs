using Gravitons.UI.Modal;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RankingBehaviour : MonoBehaviour
{
    public GameObject TopRankingContentList;

    public GameObject OtherRankingContentList;

    public TMP_InputField NicknameInputField;

    public RankingContentUnitBehaviour TopRankingContentUnitPrefab;

    public RankingContentUnitBehaviour OtherRankingContentUnitPrefab;

    public MapChooserBehaviour MapChooserBehavior;

    Modal openedModal;

    public void Search()
    {
        int mapId = MapChooserBehavior.GetChoosedId();
        string nickname = NicknameInputField.text;
        Search(mapId, nickname);
    }

    public void Search(int mapId, string nickname)
    {
        Search(mapId, "", 0, 3, TopRankingContentList, TopRankingContentUnitPrefab);
        Search(mapId, nickname, 0, 30, OtherRankingContentList, OtherRankingContentUnitPrefab);
    }

    private void Clear(GameObject contentList)
    {
        foreach (Transform child in contentList.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    void Search(int mapId, string nickname, int start, int count, GameObject contentList, RankingContentUnitBehaviour rankingContentUnitPrefab)
    {
        Clear(contentList);
        RankingHttpManager.Instance.GetRankingAll(mapId, nickname, start, count, (RankingInfo[] ris) =>
        {
            foreach (RankingInfo rankingInfo in ris)
            {
                RankingContentUnitBehaviour rankingContentUnit = Instantiate(rankingContentUnitPrefab);
                rankingContentUnit.SetRankingContentUnit(rankingInfo);
                rankingContentUnit.transform.SetParent(contentList.transform);
            }
            
        }, 
        (string errorMessage) =>
        {
            if(openedModal  != null)
            {
                openedModal = ModalManager.Show("Error", errorMessage, new ModalButton[] { new() { Text = "Close" } });
            }
        });
    }

}
