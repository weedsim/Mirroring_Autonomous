using Fusion;
using Gravitons.UI.Modal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameCompleteManager : MonoBehaviour
{
    public InGameFinalizer finalizer;

    public void RequestCompleteGame(NetworkRunner runner, CompleteGameModalContent modalContent)
    {

        int mapId = GetMapId(runner);

        List<RankingUnitInfo> ruis = new();
        foreach (PlayerRef pr in runner.ActivePlayers)
        {
            long uid = GetPlayerUid(runner, pr);
            string nickname = GetPlayerNickname(runner, pr);
            int characterClassId = GetPlayerCharacterClassId(runner, pr);

            RankingUnitInfo rui = new RankingUnitInfo(uid, nickname, characterClassId);
            ruis.Add(rui);
        }

        RankingInfo rankingInfo = new(mapId, modalContent.CompleteTime, ruis.ToArray());

        RankingHttpManager.Instance.PostRanking(rankingInfo, (string result) =>
        {
            Debug.Log("Get : " + result);
            PopupCompleteModal(runner, modalContent);
        }, (string errorText) =>
        {
            ModalManager.Show("Server Error", errorText, new ModalButton[] { new() { Text = "Close" } });
        });

        

    }

    public void RequestCompleteGame(NetworkRunner runner, long completeTime)
    {

        CompleteGameModalContent modalContent = new() { CompleteTime = completeTime };

        int mapId = GetMapId(runner);

        List<RankingUnitInfo> ruis = new();
        foreach (PlayerRef pr in runner.ActivePlayers)
        {
            long uid = GetPlayerUid(runner, pr);
            string nickname = GetPlayerNickname(runner, pr);
            int characterClassId = GetPlayerCharacterClassId(runner, pr);

            RankingUnitInfo rui = new RankingUnitInfo(uid, nickname, characterClassId);
            ruis.Add(rui);
        }

        RankingInfo rankingInfo = new(mapId, completeTime, ruis.ToArray());

        RankingHttpManager.Instance.PostRanking(rankingInfo, (string result) =>
        {
            Debug.Log("Get : " + result);
            PopupCompleteModal(runner, modalContent);
        }, (string errorText) =>
        {
            ModalManager.Show("Server Error", errorText, new ModalButton[] { new() { Text = "Close" } });
        });

        

    }

    public void PopupCompleteModal(NetworkRunner runner, CompleteGameModalContent modalContent)
    {
        ModalManager.Show("CompleteGame", modalContent, new ModalButton[]{ new() { Text="Exit",
            Callback= ()=>
            {
                finalizer.ReturnLobby();
            }
        }});
    }

    

    public int GetMapId(NetworkRunner runner)
    {
        return runner.SessionInfo.Properties["mapId"];
    }

    public int GetPlayerCharacterClassId(NetworkRunner runner, PlayerRef pr)
    {
        return Spawner.PlayerObjects[pr].CharacterClassId;
    }

    public string GetPlayerNickname(NetworkRunner runner, PlayerRef pr)
    {
        return Spawner.PlayerObjects[pr].Nickname.Value;
    }

    public int GetPlayerUid(NetworkRunner runner, PlayerRef pr)
    {
        return Spawner.PlayerObjects[pr].UID;
    }
}
