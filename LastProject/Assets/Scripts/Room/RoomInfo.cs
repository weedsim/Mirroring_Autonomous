using Fusion;
using System;
using System.Collections.Generic;

[Serializable]
public class RoomInfo
{
    public int RoomId;

    public string LobbyId;

    public string RoomName;

    public int MapId;

    public ISet<PlayerRef> CurrentPlayers;

    public int MaxPlayerCnt;
    
}
