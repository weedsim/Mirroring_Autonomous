using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigArrowHandler : NetworkBehaviour
{
    [SerializeField]
    public float speed = 100.0f;

    [Header("Particles")]
    public GameObject hitParticle;
    GameObject _HitParticle;

    NetworkObject networkObject;

    [Header("Target Position")]
    Vector3 target;
    Vector3 direction;

    [Header("Damage")]
    public int BigArrowDamage = 500;

    public void Fire(Vector3 targetPosition)
    {
        target = targetPosition;
        transform.LookAt(target);
        transform.position += transform.right * 2.0f;
        transform.LookAt(target);
        networkObject = GetComponent<NetworkObject>();
        direction = target - transform.position;
        direction.Normalize();
    }


    public override void FixedUpdateNetwork()
    {

        transform.position += direction * speed * Runner.DeltaTime;

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (collision.gameObject.GetComponentInParent<HPHandler>() != null)
            {
                collision.gameObject.GetComponentInParent<HPHandler>().OnTakeDamage(BigArrowDamage);
            }
        }
        if (!collision.gameObject.CompareTag("Player"))
        {
            Runner.Despawn(networkObject);
        }
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if(other == null) { return; }
    //    GameObject _gameObject = other.gameObject.transform.root.gameObject;
    //    if (!_gameObject.CompareTag("Player") && !_gameObject.CompareTag("Skill")) { return; }
    //    Debug.Log(_gameObject);
    //    if (_gameObject.CompareTag("Enemy"))
    //    {
    //        Debug.Log("HIT Enemy1");
    //        if (_gameObject.GetComponentInParent<HPHandler>() != null)
    //        {
    //            Debug.Log("HIT Enemy2");
    //            _gameObject.GetComponentInParent<HPHandler>().OnTakeDamage(BigArrowDamage);
    //        }
    //    }

    //    if (networkObject != null)
    //    {
    //        Runner.Despawn(networkObject);
    //    }
    //}

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        _HitParticle = Instantiate(hitParticle);
        Destroy(_HitParticle, 1.0f);
    }
}
