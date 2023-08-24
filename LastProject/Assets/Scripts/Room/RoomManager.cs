using Fusion;
using Gravitons.UI.Modal;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : Singleton<RoomManager>, IPackable<RoomInfoDTOUnit[]>, IUnpackable<RoomInfoDTOUnit[]>, IPackable<GameLobbyInfoDTOUnit[]>, IUnpackable<GameLobbyInfoDTOUnit[]>
{
    NetworkInGameStarterManager networkInGameStarterManager;

    PlayerManager _playerManagerInstance;
    GameLobbyManager _gameLobbyManagerInstance;
    Dictionary<PlayerRef, RoomInfo> _playerToRoom;
    Dictionary<PlayerRef, GameLobbyInfo> _playerToLobby;
    Dictionary<string, Dictionary<int, RoomInfo>> _lobbyIdToRoomList;
    NetworkRunner _runner;
    int _roomIdSeq;

    bool _initializedRoom;
    bool _initializedLobby;

    protected int _roomServerSeq;

    public int RoomServerSeq { get { _roomServerSeq++; return _roomServerSeq; } }

    public bool UIRefreshNeeded { get; private set; }

    public override void Initialize()
    {
        Debug.Log("RoomManager Started");
        _playerManagerInstance = PlayerManager.Instance;
        _gameLobbyManagerInstance = GameLobbyManager.Instance;
        _playerToLobby = new Dictionary<PlayerRef, GameLobbyInfo>();
        _playerToRoom = new Dictionary<PlayerRef, RoomInfo>();
        _lobbyIdToRoomList = new Dictionary<string, Dictionary<int, RoomInfo>>();
        _roomIdSeq = 0;
        UIRefreshNeeded = true;
        _initializedRoom = false;
        _initializedLobby = false;
    }
    public NetworkInGameStarterManager GetNetworkInGameStarterManager()
    {
        return this.networkInGameStarterManager;
    }

    public void SetNetworkInGameStarterManager(NetworkInGameStarterManager networkInGameStarterManager)
    {
        this.networkInGameStarterManager = networkInGameStarterManager;
    }

    public void SetNetworkRunner(NetworkRunner runner)
    {
        _runner = runner;
    }

    public void AnnounceUIRefreshed()
    {
        UIRefreshNeeded = false;
    }

    public bool IsInitialized()
    {
        return _initializedRoom && _initializedLobby;
    }

    public GameLobbyInfo GetGameLobby(PlayerRef pr)
    {
        return _playerToLobby.GetValueOrDefault(pr);
    }

    public Dictionary<int, RoomInfo> GetRoomList(string lobbyId)
    {
        return _lobbyIdToRoomList.GetValueOrDefault(lobbyId);
    }

    public RoomInfo GetRoomByPlayerRef(PlayerRef playerRef)
    {
        return _playerToRoom.GetValueOrDefault(playerRef);
    }

    public RoomInfo GetRoomByRoomId(string lobbyId, int roomId)
    {
        return GetRoomList(lobbyId).GetValueOrDefault(roomId);
    }

    public IEnumerable<RoomInfo> GetRoomsFromLobbyId(string lobbyId)
    {
        return _lobbyIdToRoomList.GetValueOrDefault(lobbyId)?.Values;
    }

    public void JoinLobby(string lobbyId)
    {
        PlayerInfo self = _playerManagerInstance.GetLocalPlayerInfo();
        RoomRpcManager.RPC_RequestJoinLobby(_runner, PlayerRef.None, lobbyId, self.PlayerRef);
    }

    public void JoinLobby(string lobbyId, PlayerRef pr)
    {
        PlayerInfo pi = _playerManagerInstance.GetPlayerInfo(pr);
        _playerToLobby[pr] = _gameLobbyManagerInstance.GetLobbyInfo(lobbyId);

        if (!_lobbyIdToRoomList.ContainsKey(lobbyId))
        {
            _lobbyIdToRoomList[lobbyId] = new Dictionary<int, RoomInfo>();
        }
        Debug.Log("Method : JoinLobby" + "::" + "Player : " + pi.Nickname);
        UIRefreshNeeded = true;
    }

    public int CreateRoomId(string lobbyId)
    {
        int seq = GetRoomList(lobbyId).Count;
        RoomRpcManager.RPC_RequestCreateRoomId(_runner, PlayerRef.None, lobbyId, seq);
        return seq;
    }

    public int CreateRoomId(string lobbyId, int roomIdSeq)
    {
        this._roomIdSeq = roomIdSeq;
        return this._roomIdSeq;
    }

    public void CreateRoom(string roomName, int mapId)
    {
        PlayerInfo self = _playerManagerInstance.GetLocalPlayerInfo();
        RoomRpcManager.RPC_RequestCreateRoom(_runner, PlayerRef.None, self.PlayerRef, roomName, mapId);
    }

    public void CreateRoom(PlayerRef pr, string roomName, int mapId)
    {
        PlayerInfo pi = _playerManagerInstance.GetPlayerInfo(pr);
        
        
        GameLobbyInfo gli = _playerToLobby.GetValueOrDefault(pr) ?? throw new GameLobbyNotFoundException();

        if (_playerToRoom.ContainsKey(pr) && _playerToRoom[pr].RoomId != -1)
        {
            ExitRoom(pr);
        }
        string lobbyId = gli.LobbyId;

        int roomId = CreateRoomId(lobbyId);
        RoomInfo room = new RoomInfo()
        {
            LobbyId = lobbyId,
            MapId = mapId,
            MaxPlayerCnt = 4,
            CurrentPlayers = new HashSet<PlayerRef>(),
            RoomId = roomId,
            RoomName = roomName,
        };
        room.CurrentPlayers.Add(pi.PlayerRef);

        _playerToRoom[pi.PlayerRef] = room;
        GetRoomList(lobbyId).Add(room.RoomId, room);
        Debug.Log("Method : CreateRoom" + "::" + "Player : " + pi.Nickname);
        
        UIRefreshNeeded = true;
    }

    public void JoinRoom(int roomId)
    {
        PlayerInfo self = _playerManagerInstance.GetLocalPlayerInfo();
        RoomRpcManager.RPC_AnnounceJoinRoom(_runner, roomId, self.PlayerRef);
    }

    public void JoinRoom(int roomId, PlayerRef pr)
    {
        PlayerInfo pi = _playerManagerInstance.GetPlayerInfo(pr);
        GameLobbyInfo gli = GetGameLobby(pr);
        RoomInfo ri = GetRoomByRoomId(gli.LobbyId,roomId);
        if(ri != null)
        {
            if(ri.CurrentPlayers.Count == ri.MaxPlayerCnt)
            {
                throw new RoomIsAlreadyFullException();
            }
            else
            {
                ri.CurrentPlayers.Add(pi.PlayerRef);
                _playerToRoom[pi.PlayerRef] = ri;
            }
        }
        else
        {
            throw new RoomNotFoundException();
        }
        Debug.Log("Method : JoinRoom" + "::" + "Player : " + pi.Nickname);
        UIRefreshNeeded = true;
    }

    public void ExitRoom()
    {
        PlayerInfo self = _playerManagerInstance.GetLocalPlayerInfo();
        RoomRpcManager.RPC_AnnounceExitRoom(_runner, self.PlayerRef);
    }

    public void ExitRoom(PlayerRef pr)
    {
        RoomInfo ri;
        if(_playerToRoom.TryGetValue(pr,out ri))
        {
            if(ri.CurrentPlayers.Count == 1)
            {
                ri.CurrentPlayers.Clear();
                GetRoomList(ri.LobbyId).Remove(ri.RoomId);
            }
            else
            {
                ri.CurrentPlayers.Remove(pr);
            }
            _playerToRoom.Remove(pr);
        }
        else
        {
            throw new RoomNotFoundException();
        }
        Debug.Log("Method : ExitRoom" + "::" + "Player : " + pr);
        UIRefreshNeeded = true;
    }

    public void RequestStartGame()
    {
        PlayerInfo self = _playerManagerInstance.GetLocalPlayerInfo();
        RoomRpcManager.RPC_RequestStartGame(_runner, PlayerRef.None, self.PlayerRef);
    }

    public void StartGame(int mapId, int sessionSeq, int playerCount, bool isHost)
    {
        if(networkInGameStarterManager == null)
        {
            ModalManager.Show("Error", "GameStarter is not initialized", new ModalButton[] { new() { Text="Close"} });
        }
        else
        {
            networkInGameStarterManager.StartGame(mapId, sessionSeq, playerCount, isHost);
        }
    }

    public RoomInfoDTOUnit[] Pack()
    {
        int size = 0;
        int i = 0;
        foreach (var rooms in _lobbyIdToRoomList.Values)
        {
            size += rooms.Count;
        }
        RoomInfoDTOUnit[] result = new RoomInfoDTOUnit[size];
        foreach (var rooms in _lobbyIdToRoomList.Values)
        {
            foreach(RoomInfo ri in rooms.Values)
            {
                RoomInfoDTOUnit unit = new RoomInfoDTOUnit();
                unit.LobbyId = ri.LobbyId;
                unit.RoomId = ri.RoomId;
                unit.RoomName = ri.RoomName;
                unit.MapId = ri.MapId;
                unit.MaxPlayerCnt = ri.MaxPlayerCnt;
                foreach (PlayerRef pr in ri.CurrentPlayers)
                {
                    unit.CurrentPlayers.Add(pr);
                }
                result[i++] = unit;
            }
        }
        return result;
    }

    public void Unpack(RoomInfoDTOUnit[] packedInstance)
    {
        
        _playerToRoom.Clear();
        _lobbyIdToRoomList.Clear();

        foreach(RoomInfoDTOUnit unit in packedInstance)
        {
            RoomInfo ri = new()
            {
                LobbyId = unit.LobbyId.Value,
                RoomId = unit.RoomId,
                RoomName = unit.RoomName.Value,
                MapId = unit.MapId,
                MaxPlayerCnt = unit.MaxPlayerCnt,
                CurrentPlayers = new HashSet<PlayerRef>()
            };
            foreach (PlayerRef pr in unit.CurrentPlayers)
            {
                ri.CurrentPlayers.Add(pr);
                _playerToRoom.Add(pr, ri);
            }
            if (!_lobbyIdToRoomList.ContainsKey(ri.LobbyId))
            {
                _lobbyIdToRoomList.Add(ri.LobbyId, new());
            }
            _lobbyIdToRoomList[ri.LobbyId].Add(ri.RoomId, ri);
        }
        _initializedRoom = true;
    }

    GameLobbyInfoDTOUnit[] IPackable<GameLobbyInfoDTOUnit[]>.Pack()
    {
        GameLobbyInfoDTOUnit[] result = new GameLobbyInfoDTOUnit[_playerToLobby.Count];
        int i = 0;
        foreach(KeyValuePair<PlayerRef,GameLobbyInfo> pgli in _playerToLobby)
        {
            GameLobbyInfoDTOUnit gliu = new GameLobbyInfoDTOUnit();
            gliu.LobbyId = pgli.Value.LobbyId;
            gliu.PlayerRef = pgli.Key;
            result[i++] = gliu;
        }
        return result;

    }

    public void Unpack(GameLobbyInfoDTOUnit[] packedInstance)
    {
        _playerToLobby.Clear();
        foreach(GameLobbyInfoDTOUnit gliu in packedInstance)
        {
            _playerToLobby[gliu.PlayerRef] = _gameLobbyManagerInstance.GetLobbyInfo(gliu.LobbyId.Value);
        }
        _initializedLobby = true;
    }
}
