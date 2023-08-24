using Fusion;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : Singleton<PlayerManager>, IPackable<PlayerInfoDTOUnit[]>, IUnpackable<PlayerInfoDTOUnit[]>, IConverter<PlayerInfo, PlayerInfoDTOUnit>, IConverter<PlayerInfoDTOUnit, PlayerInfo>
{
    CharacterClassManager _characterClassManagerInstance;
    Dictionary<int, PlayerInfo> _playerInfos;
    PlayerRef _localPlayerRef;
    NetworkRunner _runner;
    bool _initialized = false;

    public bool UIRefreshNeeded { get; private set; }

    public void AnnounceUIRefreshed()
    {
        UIRefreshNeeded = false;
    }

    public void SetNetworkRunner(NetworkRunner runner)
    {
        _runner = runner;
    }

    public override void Initialize()
    {
        Debug.Log("PlayerManager Started");
        _playerInfos = new Dictionary<int, PlayerInfo>();
        _characterClassManagerInstance = CharacterClassManager.Instance;
        _initialized = false;
        UIRefreshNeeded = true;
    }

    public void SetLocalPlayerRef(PlayerRef localPlayerRef)
    {
        Debug.Log("_localPlayerRef Set : " + localPlayerRef);
        _localPlayerRef = localPlayerRef;
    }

    public bool IsInitialized()
    {
        return _initialized;
    }

    public PlayerInfo GetPlayerInfo(PlayerRef playerRef)
    {
        return _playerInfos[playerRef];
    }

    public void RegisterPlayer(PlayerInfo pi)
    {
        _playerInfos[pi.PlayerRef] = pi;
    }

    public PlayerInfo JoinPlayer(PlayerRef playerRef, string playerId)
    {
        return _playerInfos[playerRef] = CreateDefaultPlayerInfo(playerRef, playerId);
    }

    public void ExitPlayer(PlayerRef playerRef)
    {
        _playerInfos.Remove(playerRef);
    }

    public PlayerInfo CreateDefaultPlayerInfo(PlayerRef playerRef, string playerId)
    {
        return new PlayerInfo()
        {
            PlayerRef = playerRef,
            Nickname = playerId,
            CharacterClassId = _characterClassManagerInstance.GetDefaultCharacterClassId(),
        };
    }

    public PlayerInfo GetLocalPlayerInfo()
    {
        if(_localPlayerRef == -1)
        {
            throw new NotAllocatedPlayerByServerException();
        }
        return _playerInfos[_localPlayerRef];
    }

    public void Notify_WhoAmI(PlayerInfo pi)
    {
        if (!_playerInfos.ContainsKey(pi.PlayerRef))
        {
            _playerInfos.Add(pi.PlayerRef, pi);
        }
    }

    public void ChangeCharacterClass(int characterClassId)
    {
        PlayerRpcManager.RPC_RequestChangeCharacgterClass(_runner, PlayerRef.None, _localPlayerRef, characterClassId);
    }

    public void ChangeCharacterClass(PlayerRef player, int characterClassId)
    {
        GetPlayerInfo(player).CharacterClassId = characterClassId;
        UIRefreshNeeded = true;
    }

    public PlayerInfoDTOUnit[] Pack()
    {
        PlayerInfoDTOUnit[] result = new PlayerInfoDTOUnit[_playerInfos.Count];
        int i = 0;
        foreach(PlayerInfo pi in _playerInfos.Values) 
        {
            PlayerInfoDTOUnit pudu = Convert(pi);
            result[i++] = pudu;
        }
        return result;
    }

    public void Unpack(PlayerInfoDTOUnit[] packedInstance)
    {
        _playerInfos.Clear();
        foreach(PlayerInfoDTOUnit pudu in packedInstance)
        {
            PlayerInfo pi = Convert(pudu);
            _playerInfos.Add(pi.PlayerRef, pi);
        }
        _initialized = true;
    }

    public PlayerInfoDTOUnit Convert(PlayerInfo pi)
    {
        return new()
        {
            PlayerRef = pi.PlayerRef,
            CharacterClassId = pi.CharacterClassId,
            PlayerId = pi.Nickname,
        };
    }

    public PlayerInfo Convert(PlayerInfoDTOUnit pudu)
    {
        return new()
        {
            PlayerRef = pudu.PlayerRef,
            Nickname = pudu.PlayerId.Value,
            CharacterClassId = pudu.CharacterClassId,
        };
    }



}
