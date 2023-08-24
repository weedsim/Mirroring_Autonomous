using Fusion;
using Gravitons.UI.Modal;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NetworkInGameStarterManager : MonoBehaviour
{
    NetworkRunner networkRunner;
    public NetworkRunner networkRunnerPrefab;

    public void CleanNetworkRunner()
    {
        if(networkRunner != null && !networkRunner.IsShutdown)
        {
            networkRunner.Shutdown();
        }
        this.networkRunner = null;
    }


    public NetworkRunner GetNetworkRunner()
    {
        //if (networkRunner == null || !networkRunner.IsRunning)
        //{
        //    networkRunner = gameObject.AddComponent<NetworkRunner>();
        //    networkRunner.name = "NetworkRunner";
        //}
        //return networkRunner;

        if(networkRunner == null || networkRunner.IsShutdown)
        {
            networkRunner = Instantiate(networkRunnerPrefab);
            networkRunner.name = "NetworkRunner";
        }
        return networkRunner;
    }

    public async void StartGame(int mapId, int sessionSeq, int playerCount, bool isHost)
    {
        CleanNetworkRunner();
        NetworkRunner runner = GetNetworkRunner();
        
        var sceneManager = runner.GetComponents(typeof(MonoBehaviour)).OfType<INetworkSceneManager>().FirstOrDefault();

        if (sceneManager == null)
        {
            //Handle networked objects that already exits in the scene
            sceneManager = runner.gameObject.AddComponent<NetworkSceneManagerDefault>();
        }
        Debug.Log("PlayerCount : " + playerCount);
        Dictionary<string, SessionProperty> properties = new()
        {
            ["mapId"] = mapId,
            ["playerCount"] = playerCount
        };

        Modal startGameModal = ModalManager.Show("Start Game", "Game will be started. Wait!!", new ModalButton[] { });
        StartGameResult result = await networkRunner.StartGame(new StartGameArgs()
        {
            SessionName = CreateSessionName(mapId, sessionSeq),
            GameMode = isHost?GameMode.Host:GameMode.Client,
            SceneManager = sceneManager,
            Scene = MapManager.Instance.GetMapInfo(mapId).SceneIndex,
            SessionProperties = properties
        });
        if (result.Ok)
        {

            Debug.Log("[START GAME]");
            return;
        }
        else
        {
            startGameModal.Close();
            Debug.Log(result.ErrorMessage);
            ModalManager.Show("Start Failed", result.ErrorMessage, new ModalButton[] { new ModalButton() { Text="Close" } });
            throw new InGameException();
            
        }
    }
    public static string CreateSessionName(int mapId, int sessionSeq)
    {
        return "Game" + mapId + ":" + sessionSeq;
    }
}
