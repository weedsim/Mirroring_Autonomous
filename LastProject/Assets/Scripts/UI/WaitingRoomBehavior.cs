using Fusion;
using Gravitons.UI.Modal;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WaitingRoomBehavior : MonoBehaviour
{

    public Button CreateButton;
    public Button JoinExitButton;
    public TMP_Text DescText;
    public GameObject RoomList;
    public GameObject RoomPlayerList;
    public WaitingRoomBuilder Builder;
    public NetworkInGameStarterManager NetworkInGameStarterManager;

    private ModalContentBase dummy;

    bool _isJoinBtn;
    bool _isCreateBtn;

    RoomManager _roomManager;
    PlayerManager _playerManager;
    MapManager _mapManager;
    RoomInfo _currentRoomInfo;
    void Start()
    {
        _roomManager = RoomManager.Instance;
        _playerManager = PlayerManager.Instance;
        _mapManager = MapManager.Instance;
        _isJoinBtn = false;
        _isCreateBtn = true;
        _roomManager.SetNetworkInGameStarterManager(NetworkInGameStarterManager);
    }

    // Update is called once per frame
    void Update()
    {
        if ( (_roomManager.UIRefreshNeeded || _playerManager.UIRefreshNeeded) && _roomManager.IsInitialized() && _playerManager.IsInitialized())
        {
            try
            {
                Debug.Log("Refresh Room");
                PlayerInfo pi = _playerManager.GetLocalPlayerInfo();
                RoomInfo ri = _roomManager.GetRoomByPlayerRef(pi.PlayerRef);
                if( ri != null )
                {
                    ClickRoomListUnit(ri);
                }
                RefreshCurrentRoomInfo();
                RefreshRooms();
                RefreshRoomInfo();
                RefreshButton();
            }
            catch (NotAllocatedPlayerByServerException e)
            {
                ModalManager.Show("Error", e.Message, new ModalButton[] { new() {Text="OK", Callback=()=>
                {
                    SceneManager.LoadScene((int)SceneDefs.SERVER);
                }
            }});
            }
            catch (GameLobbyNotFoundException ex)
            {
                Debug.Log("Wait For Joining Lobby");
            }catch (InGameException ex)
            {
                Debug.LogError(ex);
                Debug.Log("Refresh Failed");
            }catch(Exception ex)
            {
                Debug.LogError(ex);
                Debug.Log("Refresh Failed by internal exception");
            }
            finally
            {
                _roomManager.AnnounceUIRefreshed();
                _playerManager.AnnounceUIRefreshed();
            }
            
        }
    }

    void RefreshCurrentRoomInfo()
    {
        if (_currentRoomInfo != null && _roomManager.GetRoomByRoomId(_currentRoomInfo.LobbyId, _currentRoomInfo.RoomId) == null)
        {
            _currentRoomInfo = null;
        }
    }

    void RefreshRooms()
    {
        foreach (Transform child in RoomList.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        string lobbyId = _roomManager.GetGameLobby(_playerManager.GetLocalPlayerInfo().PlayerRef)?.LobbyId;
        if (lobbyId == null) return;
        IEnumerable<RoomInfo> rooms = _roomManager.GetRoomsFromLobbyId(lobbyId);
        if(rooms == null)
        {
            throw new GameLobbyNotFoundException();
        }
        foreach (RoomInfo roomInfo in rooms)
        {
            Debug.Log(roomInfo);
            Builder.CreateRoomListUnit(this, roomInfo).transform.SetParent(RoomList.transform, true);
        }
    }

    void RefreshRoomInfo()
    {
        if(_currentRoomInfo == null || _currentRoomInfo.CurrentPlayers.Count == 0)
        {
            DescText.text = "";
        }
        else
        {
            DescText.SetText(string.Format
                (
                    "Room Name : {0} \n Map Name : {1} \n Room Member Count : {2} / {3}",
                    _currentRoomInfo.RoomName,
                    _mapManager.GetMapInfo(_currentRoomInfo.MapId).MapName ?? "Unknown Map",
                    _currentRoomInfo.CurrentPlayers.Count,
                    _currentRoomInfo.MaxPlayerCnt
                )
            );
        }
        RefreshRoomPlayList();
    }

    void RefreshRoomPlayList()
    {
        foreach (Transform child in RoomPlayerList.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        if (_currentRoomInfo != null)
        {
            foreach (GameObject obj in Builder.CreatePlayerRoomListUnit(this, _currentRoomInfo))
            {
                obj.transform.SetParent(RoomPlayerList.transform);
            }
        }
    }

    void RefreshButton()
    {
        PlayerInfo pi = _playerManager.GetLocalPlayerInfo();
        RoomInfo ri = _roomManager.GetRoomByPlayerRef(pi.PlayerRef);
        if (ri == null)
        {
            CreateButton.enabled = true;
            JoinExitButton.enabled = true;
            CreateButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().SetText("Create");
            JoinExitButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().SetText("Join");
            _isJoinBtn = true;
            _isCreateBtn = true;
        }
        else
        {
            CreateButton.enabled = true;
            JoinExitButton.enabled = true;
            CreateButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().SetText("StartGame");
            JoinExitButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().SetText("Exit");
            _isJoinBtn = false;
            _isCreateBtn = false;
        }
    }

    public void ClickRoomListUnit(RoomInfo roomInfo)
    {
        try
        {
            if (_isJoinBtn)
            {
                _currentRoomInfo = roomInfo;
                RefreshRoomInfo();
                Debug.Log("Click RoomInfoUnit: " + roomInfo.RoomName);
            }
        }
        catch (NotAllocatedPlayerByServerException e)
        {
            ModalManager.Show("Error", e.Message, new ModalButton[] { new() {Text="OK", Callback=()=>
                {
                    SceneManager.LoadScene((int)SceneDefs.SERVER);
                }
            }});
        }
        catch (InGameException e)
        {
            ModalManager.Show("Error", e.Message, new ModalButton[] { new() { Text = "Close" } });
        }
        
    }

    public void ClickCreateButton()
    {
        try
        {
            if (_isCreateBtn)
            {
                ModalManager.Show("RoomCreater", dummy, null);
            }
            else
            {
                _roomManager.RequestStartGame();
            }
        }
        catch (NotAllocatedPlayerByServerException e)
        {
            ModalManager.Show("Error", e.Message, new ModalButton[] { new() {Text="OK", Callback=()=>
                {
                    SceneManager.LoadScene((int)SceneDefs.SERVER);
                }
            }});
        }
        catch (InGameException e)
        {
            ModalManager.Show("Error", e.Message, new ModalButton[] { new() { Text = "Close" } });
        }

        
    }

    public void ClickJoinExitButton()
    {
        try
        {
            if (_isJoinBtn)
            {
                if (_currentRoomInfo == null) { return; }
                try
                {
                    _roomManager.JoinRoom(_currentRoomInfo.RoomId);
                }
                catch (InGameException e)
                {
                    ModalManager.Show("Error", e.Message, new ModalButton[] { new() { Text = "Close" } });
                }
            }
            else
            {
                _currentRoomInfo = null;
                _roomManager.ExitRoom();
            }
        }
        catch (NotAllocatedPlayerByServerException e)
        {
            ModalManager.Show("Error", e.Message, new ModalButton[] { new() {Text="OK", Callback=()=>
                {
                    SceneManager.LoadScene((int)SceneDefs.SERVER);
                }
            }});
        }
        catch (InGameException e)
        {
            ModalManager.Show("Error", e.Message, new ModalButton[] { new() { Text = "Close" } });
        }
        
    }

    public void ClickCharacterClassChangeButton()
    {
        ModalManager.Show("CharacterClassChange", dummy, null);
    }
}
