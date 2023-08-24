using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.Types;

public class RoomRpcManager : SimulationBehaviour
{

    [Rpc]
    public static void RPC_RequestJoinLobby(NetworkRunner runner, [RpcTarget] PlayerRef target, string lobbyId, PlayerRef playerRef)
    {
        Debug.Log(playerRef + " Joined Lobby Scanning");
        RPC_AnnounceJoinLobby(runner, lobbyId, playerRef);
    }

    [Rpc]
    public static void RPC_AnnounceJoinLobby(NetworkRunner runner, string lobbyId, PlayerRef playerRef)
    {
        Debug.Log(playerRef + " Joined Lobby");
        RoomManager.Instance.JoinLobby(lobbyId, playerRef);
    }

    [Rpc]
    public static void RPC_RequestCreateRoomId(NetworkRunner runner, [RpcTarget] PlayerRef target, string lobbyId, int seq)
    {
        RoomManager.Instance.CreateRoomId(lobbyId, seq);
        RPC_AnnounceCreateRoomId(runner, lobbyId, seq);
    }

    [Rpc]
    public static void RPC_AnnounceCreateRoomId(NetworkRunner runner, string lobbyId, int seq)
    {
        RoomManager.Instance.CreateRoomId(lobbyId, seq);
    }

    [Rpc]
    public static void RPC_RequestCreateRoom(NetworkRunner runner, [RpcTarget] PlayerRef target, PlayerRef playerRef, string roomName, int mapId)
    {
        Debug.Log("RPC_RequestCreateRoom : " + playerRef);
        RoomManager.Instance.CreateRoom(playerRef, roomName, mapId);
        RPC_AnnounceCreateRoom(runner, playerRef, roomName, mapId);
    }

    [Rpc]
    public static void RPC_AnnounceCreateRoom(NetworkRunner runner, PlayerRef playerRef, string roomName, int mapId)
    {
        Debug.Log("RPC_AnnounceCreateRoom : " + playerRef);
        RoomManager.Instance.CreateRoom(playerRef, roomName, mapId);
    }

    [Rpc]
    public static void RPC_AnnounceJoinRoom(NetworkRunner runner, int roomId, PlayerRef playerRef)
    {
        RoomManager.Instance.JoinRoom(roomId, playerRef);
    }

    [Rpc]
    public static void RPC_AnnounceExitRoom(NetworkRunner runner, PlayerRef playerRef)
    {
        RoomManager.Instance.ExitRoom(playerRef);
    }

    /*
    [Rpc]
    public static void RPC_RequestRoomInfos(NetworkRunner runner, [RpcTarget] PlayerRef target, PlayerRef player)
    {
        RoomRpcManager.RPC_ResponseRoomInfos(runner, player, RoomManager.Instance.Pack());
        Debug.Log("SERVER :: ROOM DATA SEND TO " + PlayerManager.Instance.GetPlayerInfo(player).Nickname);
    }

    [Rpc]
    public static void RPC_ResponseRoomInfos(NetworkRunner runner, [RpcTarget] PlayerRef target, RoomInfoDTOUnit[] roomInfoDtoUnits)
    {
        RoomManager.Instance.Unpack(roomInfoDtoUnits);
        Debug.Log("CLIENT :: ROOM DATA RECEIVED");
    }
    */

    [Rpc]
    public static void RPC_RequestStartGame(NetworkRunner runner, [RpcTarget] PlayerRef target, PlayerRef who)
    {
        RoomInfo ri = RoomManager.Instance.GetRoomByPlayerRef(who);
        if(ri == null)
        {
            RPC_AnnounceInGameException(runner, who);
            return;
        }



        if(ri.CurrentPlayers.Count < ri.MaxPlayerCnt)
        {
            Debug.Log("SERVER :: PlayerCount is not fulfilled. but accept to start a game for the test");
        }
        List<PlayerRef> lists = new List<PlayerRef>(ri.CurrentPlayers);

        int newSessionSeq = RoomManager.Instance.RoomServerSeq;

        int playerCount = lists.Count;
        PlayerRef hostPlayer = lists[0];
        RPC_AnnounceExitRoom(runner, hostPlayer);
        RPC_AnnounceStartGame(runner, hostPlayer, ri.MapId, newSessionSeq, playerCount, true);

        foreach (PlayerRef pr in lists.GetRange(1, lists.Count - 1)) 
        {
            RPC_AnnounceExitRoom(runner, pr);
            RPC_AnnounceStartGame(runner, pr, ri.MapId, newSessionSeq, playerCount, false);
        }
        Debug.Log("SERVER :: START GAME");
    }

    [Rpc]
    public static void RPC_AnnounceStartGame(NetworkRunner runner, [RpcTarget] PlayerRef target, int mapId, int sessionSeq, int playerCount, bool isHost)
    {
        RoomManager.Instance.StartGame(mapId, sessionSeq, playerCount, isHost);
        Debug.Log("CLIENT :: START GAME AS A " + (isHost?"HOST":"CLIENT(NOT HOST)") );
    }

    [Rpc]
    public static void RPC_AnnounceInGameException(NetworkRunner runner, [RpcTarget] PlayerRef target)
    {
        throw new InGameException();
    }

}
