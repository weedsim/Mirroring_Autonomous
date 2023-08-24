using Fusion;

public class ClientRoomNetworkRunnerCallback : AbstractSimulationRunnerCallbacks
{
    public override void OnConnectedToServer(NetworkRunner runner)
    {
        RoomInitRpcManager.RPC_RequestRoomInfos(runner, PlayerRef.None, runner.LocalPlayer);
    }

}
