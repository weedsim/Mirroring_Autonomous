using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct GameLobbyInfoDTOUnit : INetworkStruct
{
    public PlayerRef PlayerRef { get; set; }

    public NetworkString<_64> LobbyId { get; set; }

}
