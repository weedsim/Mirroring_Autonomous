using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PlayerInfoDTOUnit : INetworkStruct
{
    public PlayerRef PlayerRef { get; set; }

    public NetworkString<_64> PlayerId { get; set; }

    public int CharacterClassId { get; set; }
}
