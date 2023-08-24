using Fusion;
using Fusion.Photon.Realtime;
using Fusion.Sockets;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;
#if !UNITY_SERVER
using UnityEngine.SceneManagement;
#endif

public class NetworkServerManager : Singleton<NetworkServerManager>
{
#if !UNITY_SERVER
    public override void Initialize()
    {
        SceneManager.LoadScene((int)SceneDefs.LOGIN, LoadSceneMode.Single);
    }
# else

    [SerializeField] private NetworkRunner _runnerPrefab;

    public async override void Initialize()
    {
        // Continue with start the Dedicated Server
        Application.targetFrameRate = 30;
        // Start a new Runner instance
        NetworkRunner runner = Instantiate(_runnerPrefab);

        // Start the Server
        var result = await StartSimulation(runner, SessionConfig.GetDefaultLobbyConfig());

        // Check if all went fine
        if (result.Ok)
        {
            Log.Debug($"Runner Start DONE");
            RoomManager.Instance.SetNetworkRunner(runner);
        }
        else
        {
            // Quit the application if startup fails
            Log.Debug($"Error while starting Server: {result.ShutdownReason}");

            // it can be used any error code that can be read by an external application
            // using 0 means all went fine
            Application.Quit(1);
        }
    }

    private Task<StartGameResult> StartSimulation(
      NetworkRunner runner,
      SessionConfig config
    )
    {

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


        // Start Runner
        return runner.StartGame(new StartGameArgs()
        {
            SessionName = config.SessionName,
            GameMode = GameMode.Server,
            SceneManager = runner.gameObject.AddComponent<NetworkSceneManagerDefault>(),
            Scene = (int)SceneDefs.LOBBY,
            Address = NetAddress.Any(config.Port),
            CustomPublicAddress = externalAddr,
            CustomLobbyName = config.Lobby,
            CustomPhotonAppSettings = photonSettings,
        });
    }
#endif
}
