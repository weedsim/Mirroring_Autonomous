using Fusion;
using System.Collections.Generic;
using UnityEngine;

public class ClientPlayerNetworkRunnerCallback : AbstractSimulationRunnerCallbacks
{

    public override void OnConnectedToServer(NetworkRunner runner)
    {
        Debug.Log("Connect to Server");
        PlayerRpcManager.RPC_RequestPlayerInfos(runner, PlayerRef.None, runner.LocalPlayer, AccountManager.Nickname);
    }

    public override void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
        Debug.Log("Login Complete Message From Server");

        AccountManager.Uid = (int)((long)data["uid"]);
        AccountManager.Id = (string)data["id"];
        AccountManager.Nickname = (string)data["nickname"];
        AccountManager.PlayerKey = (string)data["clientKey"];
        
        Debug.Log("You just logined  Nickname : " + AccountManager.Nickname + " with key " + AccountManager.PlayerKey);

    }

    public override void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        Debug.LogWarning($"{nameof(OnShutdown)}: {nameof(shutdownReason)}: {shutdownReason}");
    }

}