using Fusion;
using Opsive.UltimateCharacterController.Objects;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class FireBallController : NetworkBehaviour
{

    [SerializeField]
    float speed = 60.0f;

    [Header("Particles")]
    public GameObject hitParticle;

    NetworkObject networkObject;

    Vector3 target;
    Vector3 direction;
    TickTimer tickTimer = TickTimer.None;
    public int playerId= -1;
    public void Fire(Vector3 targetPosition)
    {
        target = targetPosition;
        networkObject = GetComponent<NetworkObject>();
        tickTimer = TickTimer.CreateFromSeconds(Runner, 6);
        direction = target - transform.position;
        direction.Normalize();  
    }


    public override void FixedUpdateNetwork()
    {
        if (tickTimer.Expired(Runner))
        {
            Runner.Despawn(networkObject);
            return;
        }

        transform.position += direction * speed * Runner.DeltaTime;

        
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!Object.HasStateAuthority) return;
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (collision.gameObject.GetComponentInParent<HPHandler>() != null)
            {
                collision.gameObject.GetComponentInParent<HPHandler>().OnTakeDamage(Utils.GetRandomDamage(500),playerId);
            }
            Runner.Despawn(networkObject);
            return;
        }
        if (collision.gameObject.layer == LayerMask.NameToLayer("Environment"))
        {
            Runner.Despawn(networkObject);
            return;
        }
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        GameObject explosionEffect = Instantiate(hitParticle);
        Destroy(explosionEffect, 1);
    }
}
