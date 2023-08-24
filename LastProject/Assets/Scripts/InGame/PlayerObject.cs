using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerObject : NetworkBehaviour
{

    public static PlayerObject Local { get; private set; }

    public int UID = -1;

    public int CharacterClassId = -1;

    public NetworkString<_64> Nickname { get; set; }

    public override void Spawned()
    {
        base.Spawned();

        if (Object.HasStateAuthority)
        {
            InGameManager.AddPlayerObject(Runner, Object.InputAuthority, this);
        }
        if (Object.HasInputAuthority)
        {
            this.Nickname = AccountManager.Nickname;
            this.CharacterClassId = PlayerManager.Instance.GetLocalPlayerInfo().CharacterClassId;
            this.UID = AccountManager.Uid;
            Local = this;
        }
    }

}
