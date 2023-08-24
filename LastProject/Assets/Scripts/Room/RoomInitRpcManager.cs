using Fusion;
using UnityEngine;

public class RoomInitRpcManager : SimulationBehaviour
{

    [Rpc]
    public static void RPC_AnnounceExitRoom(NetworkRunner runner, PlayerRef playerRef)
    {
        RoomManager.Instance.ExitRoom(playerRef);
    }

    [Rpc]
    public static void RPC_RequestRoomInfos(NetworkRunner runner, [RpcTarget] PlayerRef target, PlayerRef player)
    {
        RoomInfoDTOUnit[] pack = RoomManager.Instance.Pack();
        GameLobbyInfoDTOUnit[] pack2 = ((IPackable<GameLobbyInfoDTOUnit[]>)RoomManager.Instance).Pack();
        Debug.Log("SERVER :: ROOM DATA WILL RECEIVE");
        foreach (RoomInfoDTOUnit ridu in pack)
        {
            Debug.Log("SERVER :: ROOM " + ridu.RoomId + " [" + ridu.CurrentPlayers + "] DATA WILL RECEIVE");
        }
        RPC_ResponseRoomInfos(runner, player, pack);
        RPC_ResponseLobbyInfos(runner, player, pack2);
        Debug.Log("SERVER :: ROOM DATA SEND TO " + PlayerManager.Instance.GetPlayerInfo(player).Nickname);
    }

    [Rpc]
    public static void RPC_ResponseRoomInfos(NetworkRunner runner, [RpcTarget] PlayerRef target, RoomInfoDTOUnit[] roomInfoDtoUnits)
    {
        RoomManager.Instance.Unpack(roomInfoDtoUnits);

        Debug.Log("CLIENT :: ROOM DATA RECEIVED");
        foreach(RoomInfoDTOUnit ridu in roomInfoDtoUnits)
        {
            Debug.Log("CLIENT :: ROOM " + ridu.RoomId + " [" + ridu.CurrentPlayers + "] DATA RECEIVED");
        }
    }

    [Rpc]
    public static void RPC_ResponseLobbyInfos(NetworkRunner runner, [RpcTarget] PlayerRef target, GameLobbyInfoDTOUnit[] gameLobbyDtoUnits)
    {
        RoomManager.Instance.Unpack(gameLobbyDtoUnits);

        Debug.Log("CLIENT :: LOBBY DATA RECEIVED");
    }
}
