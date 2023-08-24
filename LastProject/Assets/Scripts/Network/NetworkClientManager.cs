using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System.Threading.Tasks;
using Fusion.Photon.Realtime;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using Gravitons.UI.Modal;

public class NetworkClientManager : MonoBehaviour
{
    public NetworkRunner networkRunnerPrefab;

    NetworkRunner networkRunner;

    public NetworkRunner GetNetworkRunner()
    {
        if (networkRunner == null || networkRunner.IsShutdown)
        {
            if(networkRunner != null)
            {
                Destroy(networkRunner.gameObject);
            }
            networkRunner = Instantiate(networkRunnerPrefab);
            networkRunner.name = "NetworkRunner";
        }
        return networkRunner;
    }

    public async void Login(string username, string password)
    {

        

        NetworkRunner runner = GetNetworkRunner();

        // Create a new AuthenticationValues
        AuthenticationValues authentication = new AuthenticationValues();

        // Setup
        authentication.AuthType = CustomAuthenticationType.Custom;

        Dictionary<string, object> postData = new Dictionary<string, object>();
        postData["username"] = username;
        postData["password"] = password;
        authentication.SetAuthPostData(postData);



        
        StartGameResult result = await JoinLobby(runner, SessionConfig.GetDefaultLobbyConfig(), authentication);
        

        if (!result.Ok)
        {
            Debug.Log("Login Failed");
            ModalManager.Show("Login Failed", result.ErrorMessage, new ModalButton[1] { new ModalButton { Text = "Close" } } );

        }
        else
        {
            Debug.Log("Login Complete");
            RoomManager.Instance.SetNetworkRunner(runner);
            PlayerManager.Instance.SetNetworkRunner(runner);
        }
        
    }

    public virtual async Task<StartGameResult> JoinLobby(
      NetworkRunner runner,
      SessionConfig config, 
      AuthenticationValues authentication
    )
    {

        var sceneManager = runner.GetComponents(typeof(MonoBehaviour)).OfType<INetworkSceneManager>().FirstOrDefault();

        if (sceneManager == null)
        {
            //Handle networked objects that already exits in the scene
            sceneManager = runner.gameObject.AddComponent<NetworkSceneManagerDefault>();
        }

        // Build Custom Photon Config
        var photonSettings = PhotonAppSettings.Instance.AppSettings.GetCopy();

        if (string.IsNullOrEmpty(config.Region) == false)
        {
            photonSettings.FixedRegion = config.Region.ToLower();
        }

        // Build Custom External Addr
        NetAddress? externalAddr = null;

        if (string.IsNullOrEmpty(config.PublicIP) == false && config.PublicPort > 0)
        {
            if (IPAddress.TryParse(config.PublicIP, out var _))
            {
                externalAddr = NetAddress.CreateFromIpPort(config.PublicIP, config.PublicPort);
            }
            else
            {
                Log.Warn("Unable to parse 'Custom Public IP'");
            }
        }

        Modal authWaitModal = ModalManager.Show("Please Wait", "Connect to Auth Server...", new ModalButton[] { });
        StartGameResult result = await runner.JoinSessionLobby(SessionLobby.ClientServer, config.Lobby, authentication);
        authWaitModal.Close();
        if (result.Ok)
        {
            Modal dedicateWaitModal = ModalManager.Show("Please Wait", "Connect to Dedicate Server...", new ModalButton[] { });
            // Start Runner
            StartGameResult gameResult = await runner.StartGame(new StartGameArgs()
            {
                SessionName = config.SessionName,
                GameMode = GameMode.Client,
                SceneManager = sceneManager,
                Scene = (int)SceneDefs.LOBBY,
                Address = NetAddress.Any(config.Port),
                CustomPublicAddress = externalAddr,
                CustomLobbyName = config.Lobby,
                CustomPhotonAppSettings = photonSettings,
                AuthValues = authentication
            });
            dedicateWaitModal.Close();
            return gameResult;
        }
        else
        {
            return result;
        }
    }
}