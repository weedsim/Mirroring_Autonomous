using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Server or Host Holds DTO to preserve Room Data.
public struct RoomInfoDTOUnit : INetworkStruct
{
    public int RoomId { get; set; }

    public NetworkString<_64> LobbyId { get; set; }

    public NetworkString<_64> RoomName { get; set; }

    public int MapId { get; set; }

    [Networked]
    [Capacity(32)]
    [UnitySerializeField]
    public NetworkLinkedList<PlayerRef> CurrentPlayers { get; }

    public int MaxPlayerCnt { get; set; }
}
