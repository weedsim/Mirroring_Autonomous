using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class MeteorHandler : NetworkBehaviour
{
    NetworkObject networkObject;
    //TickTimer tickTimer = TickTimer.None;

    [Header("Boom Effect")]
    public GameObject Boom;

    [Header("Target Posotion")]
    public Vector3 target;

    [Header("Collision")]
    public Collision _Impact;

    [Header("Damages")]
    public int ImpactDamage = 1000;

    public int playerId = -1;

    public void Summon(Vector3 targetPosition)
    {
        target = targetPosition;
        networkObject = GetComponent<NetworkObject>();
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasInputAuthority) return;
        _Impact = GetComponentInChildren<ImpactHandler>().impact();

        if(_Impact != null)
        {
            Debug.Log("Impact" + _Impact);
            Impact();
        }
    }

    private void Impact()
    {
        Debug.Log("_Impact" + _Impact);
        
        if(_Impact.gameObject.CompareTag("Enemy"))
        {
            if (_Impact.gameObject.GetComponentInParent<HPHandler>() != null)
            {
                _Impact.gameObject.GetComponentInParent<HPHandler>().OnTakeDamage(Utils.GetRandomDamage(ImpactDamage),playerId);
                Debug.Log("Impact");
            }
        }

        Debug.Log("_Impact.gameObject" + _Impact.gameObject);

        if (!_Impact.gameObject.CompareTag("Player") && _Impact != null)
        {
            Debug.Log("Destroy");
            Runner.Despawn(networkObject);
        }
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        Debug.Log("WHY");
        Runner.Spawn(Boom, transform.position - (Vector3.up * (10.0f)), Quaternion.identity, Object.InputAuthority, (runner, spawnedtailwind) =>
        {
            spawnedtailwind.GetComponent<TailwindHandler>();
        });

    }
}
