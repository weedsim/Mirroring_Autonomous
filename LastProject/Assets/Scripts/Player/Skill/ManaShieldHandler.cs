using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaShieldHandler : NetworkBehaviour
{
    //TickTimer tickTimer = TickTimer.None;

    NetworkObject networkObject;
    GameObject Caster;

    public void Cast(NetworkObject Caster)
    {
        //tickTimer = TickTimer.CreateFromSeconds(Runner, 3);
        networkObject = GetComponent<NetworkObject>();
        this.Caster = Caster.gameObject;
    }


    public void EndCast()
    {
        if (networkObject != null)
        {
            Runner.Despawn(networkObject);
        }
    }


    public override void FixedUpdateNetwork()
    {
        //if (tickTimer.Expired(Runner))
        //{
        //    EndCast();
        //    return;
        //}

        if (Caster != null)
        {
            Vector3 pos = Caster.transform.position;
            transform.position = pos;
        }
    }
    public override void Despawned(NetworkRunner runner, bool hasState)
    {
    }
}
