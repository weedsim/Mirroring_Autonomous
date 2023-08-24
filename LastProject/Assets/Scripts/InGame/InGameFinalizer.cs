using Fusion;
using Gravitons.UI.Modal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameFinalizer : MonoBehaviour
{

    public void ReturnLobby()
    {
        //Modal returnModal = ModalManager.Show("Please Wait", "Try to return Lobby", new ModalButton[0]);
        NetworkRunner runner = RoomManager.Instance.GetNetworkInGameStarterManager().GetNetworkRunner();
        Debug.Log(runner.SessionInfo.Name);
        RoomManager.Instance.GetNetworkInGameStarterManager().CleanNetworkRunner();
        /**
        //NetworkRunner.GetRunnerForScene();

        NetworkClientManager clientManager = GetComponent<NetworkClientManager>();
        if(clientManager != null)
        {
            clientManager = gameObject.AddComponent<NetworkClientManager>();
        }

        StartGameResult gameResult = await clientManager.JoinLobby(clientManager.GetNetworkRunner(), SessionConfig.GetDefaultLobbyConfig(), AccountManager.authenticationValues);
        if (!gameResult.Ok)
        {
            Debug.Log("Lobby Rejoining Failed");
        }
        else
        {
            Debug.Log("Lobby Rejoining Complete");
            RoomManager.Instance.SetNetworkRunner(clientManager.GetNetworkRunner());
            PlayerManager.Instance.SetNetworkRunner(clientManager.GetNetworkRunner());
        }

        **/

        //returnModal.Close();
        Spawner.PlayerObjects.Clear();
        SceneManager.LoadScene((int)SceneDefs.LOBBY);
    }
}
