using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;

public class Spawner : SimulationBehaviour, INetworkRunnerCallbacks
{

    public static Dictionary<int, PlayerObject> PlayerObjects = new();

    CharacterInputHandler characterInputHandler;

    public void OnSceneLoadStart(NetworkRunner runner) 
    {
        
    }


    [Rpc]
    public static void RPC_Connect(NetworkRunner runner, [RpcTarget] PlayerRef target, PlayerRef player, int uid, int cid, string nickname)
    {
        Debug.Log("Hello Player : " + nickname);
        PlayerObjects[player] = new PlayerObject { CharacterClassId = cid, UID = uid, Nickname = nickname };

        PlayerObject playerobject = PlayerObjects[player];
        Debug.Log("OnPlayerJoined we are server. Spawning " + playerobject.Nickname.Value);
        NetworkPlayer playerPrefab = CharacterClassManager.Instance.GetCharacterClassInfo(playerobject.CharacterClassId).playerPrefab;

        if (playerPrefab != null)
        {
            runner.Spawn(playerPrefab, Utils.GetStageOnePlayerSpawnPoint(), Quaternion.identity, player);
        }
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {
            if (runner.LocalPlayer == player)
            {
                OnConnectedToServer(runner);
            }
        }

    }


    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        if (characterInputHandler == null && NetworkPlayer.Local != null)
            characterInputHandler = NetworkPlayer.Local.GetComponent<CharacterInputHandler>();

        if (characterInputHandler != null)
            input.Set(characterInputHandler.GetNetworkInput());

    }

    public void OnConnectedToServer(NetworkRunner runner) { 
        PlayerInfo playerinfo = PlayerManager.Instance.GetLocalPlayerInfo();
        RPC_Connect(runner, PlayerRef.None, runner.LocalPlayer, AccountManager.Uid, playerinfo.CharacterClassId, AccountManager.Nickname);
        Debug.Log("OnConnectedToServer");
     
    }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) {
        PlayerObjects.Remove(player);
    }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { Debug.Log(shutdownReason.ToString()); }
    public void OnDisconnectedFromServer(NetworkRunner runner) { Debug.Log("OnDisconnectedFromServer"); }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { Debug.Log("OnConnectRequest"); }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { Debug.Log("OnConnectFailed"); }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
    public void OnSceneLoadDone(NetworkRunner runner) {

    }
    
}
