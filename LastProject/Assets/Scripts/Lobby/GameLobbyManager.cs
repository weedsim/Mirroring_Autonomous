using Fusion;
using System.Collections;
using System.Collections.Generic;

public class GameLobbyManager : Singleton<GameLobbyManager>
{

    public Dictionary<string, GameLobbyInfo> GameLobbyInfos { get; private set; }



    public override void Initialize()
    {
        GameLobbyInfos = new Dictionary<string, GameLobbyInfo>();

        GameLobbyInfo gli = new()
        {
            LobbyId = "default",
            MapId = 0
        };
        GameLobbyInfos.Add(gli.LobbyId, gli);
    }

    public GameLobbyInfo GetLobbyInfo(string lobbyId)
    {
        return GameLobbyInfos[lobbyId];
    }

}
