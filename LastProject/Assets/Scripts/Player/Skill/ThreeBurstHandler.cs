using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreeBurstHandler : NetworkBehaviour
{
    [SerializeField]
    float speed = 100.0f;

    [Header("Particles")]
    public GameObject hitParticle;

    NetworkObject networkObject;

    [Header("Target Position")]
    Vector3 target;
    Vector3 direction;

    TickTimer tickTimer = TickTimer.None;

    public void Fire(Vector3 targetPosition)
    {
        target = targetPosition;
        networkObject = GetComponent<NetworkObject>();
        tickTimer = TickTimer.CreateFromSeconds(Runner, 4);
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


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (collision.gameObject.GetComponentInParent<HPHandler>() != null)
            {
                collision.gameObject.GetComponentInParent<HPHandler>().OnTakeDamage(500);
            }
        }
        if (!collision.gameObject.CompareTag("Player"))
        {
            Runner.Despawn(networkObject);
        }
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
    }
}
