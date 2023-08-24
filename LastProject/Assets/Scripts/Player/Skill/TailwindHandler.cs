using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TailwindHandler : NetworkBehaviour
{
    NetworkObject networkObject;

    public int TickDamage = 5;

    Collider _TailwindC = null;

    public float time = 0.0f;

    private void Start()
    {
        networkObject = GetComponentInParent<NetworkObject>();
    }

    private void Update()
    {
        time += Runner.DeltaTime;

        if(time >= 2.0f)
        {
            Runner.Despawn(networkObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.gameObject.CompareTag("Skill"))
            _TailwindC = other;

        if (_TailwindC != null)
        {
            Debug.Log(_TailwindC.gameObject.transform.root.gameObject);
            if (_TailwindC.gameObject.transform.parent.gameObject.CompareTag("Enemy"))
            {
                if (_TailwindC.gameObject.transform.parent.gameObject.GetComponentInParent<HPHandler>() != null)
                {
                    _TailwindC.gameObject.transform.parent.gameObject.GetComponentInParent<HPHandler>().OnTakeDamage(TickDamage);
                    Debug.Log("Tailwind Tick Damage");
                }
            }
        }
    }

}
