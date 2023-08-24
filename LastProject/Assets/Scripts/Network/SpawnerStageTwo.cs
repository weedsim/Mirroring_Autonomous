using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;

public class SpawnerStageTwo : MonoBehaviour, INetworkRunnerCallbacks
{


    [Header("Prefabs")]
    public NetworkPlayer playerPrefab;
    //public NetworkObject enemyPrefab;
    
    //public static Dictionary<PlayerRef, NetworkPlayer> players = new Dictionary<PlayerRef, NetworkPlayer>();

    //Other compoents
    CharacterInputHandler characterInputHandler;

    // Start is called before the first frame update
    void Start()
    {
        
    }


    public void InstantiateEnemy()
    {
        //Instantiate(enemyPrefab, Utils.GetRandomSpawnPoint(), Quaternion.identity);
    }


    private IEnumerator SpawnEnemy()
    {
        yield return new WaitForSeconds(3f);
        InstantiateEnemy();
    }


    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {
            Debug.Log("OnPlayerJoined we are server. Spawning player");
            runner.Spawn(playerPrefab, Utils.GetStageThreeTestPlayerSpawnPoint(), Quaternion.identity, player);
        }
        else Debug.Log("OnPlayerJoined");

    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        if (characterInputHandler == null && NetworkPlayer.Local != null)
            characterInputHandler = NetworkPlayer.Local.GetComponent<CharacterInputHandler>();

        if (characterInputHandler != null)
            input.Set(characterInputHandler.GetNetworkInput());

    }

    public void OnConnectedToServer(NetworkRunner runner) { Debug.Log("OnConnectedToServer");
     
    }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) {
        //players.Remove(player);
    }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { Debug.Log("OnShutdown"); }
    public void OnDisconnectedFromServer(NetworkRunner runner) { Debug.Log("OnDisconnectedFromServer"); }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { Debug.Log("OnConnectRequest"); }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { Debug.Log("OnConnectFailed"); }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
    public void OnSceneLoadDone(NetworkRunner runner) { /*StartCoroutine(SpawnEnemy());*/ }
    public void OnSceneLoadStart(NetworkRunner runner) { }
}
