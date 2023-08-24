using Fusion;
using Fusion.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameManager : NetworkBehaviour, INetworkRunnerCallbacks
{

    public static InGameManager Instance { get; private set; }

    [Networked, Capacity(4)]
    NetworkDictionary<PlayerRef, PlayerObject> PlayerObjects { get; }

    public override void Spawned()
    {
        base.Spawned();
        //Instance = this;
        //Runner.AddCallbacks(this);
    }


    private void Start()
    {
        Instance = this;
        Runner.AddCallbacks(this);
    }


    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        base.Despawned(runner, hasState);
        Instance = null;
        Runner.RemoveCallbacks(this);
    }

    public static void AddPlayerObject(NetworkRunner runner, PlayerRef playerRef, PlayerObject playerObject)
    {
        Instance.PlayerObjects.Add(playerRef, playerObject);
    }

    public static void RemovePlayerObject(NetworkRunner runner, PlayerRef playerRef)
    {
        Instance.PlayerObjects.Remove(playerRef);
    }

    public static bool HasPlayer(PlayerRef pRef)
    {
        return Instance.PlayerObjects.ContainsKey(pRef);
    }

    public static PlayerObject GetPlayer(PlayerRef pRef)
    {
        if (HasPlayer(pRef))
            return Instance.PlayerObjects.Get(pRef);
        return null;
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
        AccountManager.Nickname = (string)data["nickname"];
        AccountManager.Uid = (int)data["uid"];
    }

    public void OnDisconnectedFromServer(NetworkRunner runner)
    {
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
    {
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
    }

    
}
