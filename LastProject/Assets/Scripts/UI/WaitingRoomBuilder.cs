using Fusion;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class WaitingRoomBuilder : MonoBehaviour
{

    public GameObject roomListUnitPrefab;
    public GameObject roomMemberListUnitPrefab;
    public Button roomMemberListUnitCharacterClassChangeButtonPrefab;

    PlayerManager _playerManager;
    CharacterClassManager _characterClassManager;


    void Start()
    {
        _playerManager = PlayerManager.Instance;
        _characterClassManager = CharacterClassManager.Instance;
    }


    public GameObject CreateRoomListUnit(WaitingRoomBehavior parent, RoomInfo roomInfo)
    {

        GameObject panel = Instantiate(roomListUnitPrefab);

        TMP_Text[] texts = panel.GetComponentsInChildren<TMP_Text>();


        texts[0].SetText(roomInfo.RoomId.ToString());
        texts[1].SetText(roomInfo.RoomName);
        texts[2].SetText(string.Format("{0} / {1}", roomInfo.CurrentPlayers.Count, roomInfo.MaxPlayerCnt));

        Button panelBtn = panel.GetComponent<Button>();
        panelBtn.onClick.AddListener(delegate { parent.ClickRoomListUnit(roomInfo); });

        return panel;
    }

    public IEnumerable<GameObject> CreatePlayerRoomListUnit(WaitingRoomBehavior parent, RoomInfo roomInfo)
    {
        List<GameObject> playerRoomList = new List<GameObject>();

        PlayerRef local = _playerManager.GetLocalPlayerInfo().PlayerRef;

        bool isHost = true;

        foreach (PlayerRef pr in roomInfo.CurrentPlayers)
        {
            PlayerInfo pi = PlayerManager.Instance.GetPlayerInfo(pr);

            GameObject panel = Instantiate(roomMemberListUnitPrefab);

            TMP_Text[] texts = panel.GetComponentsInChildren<TMP_Text>();

            CharacterClassInfo cci = _characterClassManager.GetCharacterClassInfo(pi.CharacterClassId);

            texts[0].SetText(cci.CharacterClassName);

            Image characterClassImage = panel.GetComponentInChildren<Image>().gameObject.GetComponentsInChildren<Image>()[1];
            characterClassImage.sprite = cci.CharacterClassImage;

            texts[1].SetText(pi.Nickname);

            if (isHost)
            {
                texts[1].fontStyle = FontStyles.Underline;
                isHost = false;
            }

            if (pr == local)
            {
                texts[1].color = Color.red;
                texts[1].fontWeight = FontWeight.Bold;
                Button ccb = Instantiate(roomMemberListUnitCharacterClassChangeButtonPrefab);
                ccb.onClick.AddListener(delegate { parent.ClickCharacterClassChangeButton(); }) ;
                ccb.transform.SetParent(panel.transform);
            }

            

            playerRoomList.Add(panel);

        }

        return playerRoomList;
    }
}
