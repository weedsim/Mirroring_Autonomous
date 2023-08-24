using Fusion;
using UnityEngine;

public class ServerPlayerNetworkRunnerCallback : AbstractSimulationRunnerCallbacks
{
    new void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log("Hello " + player);
    }

    new void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        PlayerRpcManager.RPC_AnnouncePlayerExited(runner, player);
    }
}