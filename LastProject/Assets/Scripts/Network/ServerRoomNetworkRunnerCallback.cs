using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ServerRoomNetworkRunnerCallback : AbstractSimulationRunnerCallbacks
{
    public override void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if(RoomManager.Instance.GetRoomByPlayerRef(player) != null)
        {
            RoomInitRpcManager.RPC_AnnounceExitRoom(runner, player);
        }
    }

}
