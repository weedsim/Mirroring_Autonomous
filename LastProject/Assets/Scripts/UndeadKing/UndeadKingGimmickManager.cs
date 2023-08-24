using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UndeadKingGimmickManager : NetworkBehaviour
{
    public int damage = 200;
    public float duration=5f;
    private HashSet<Collider> attackedList = new HashSet<Collider>();
    public ParticleSystemManager[] psms;
    private void OnEnable()
    {
        foreach (ParticleSystemManager psm in psms)
        {
            psm.PlayPS();
        }
        Destroy(gameObject,duration);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!Object.HasStateAuthority) return;
        if (other.CompareTag("Player")&&!attackedList.Contains(other))
        {
            attackedList.Add(other);
            HPHandler hpm = other.gameObject.GetComponentInParent<HPHandler>();
            if (hpm != null)
            {
                hpm.OnTakeDamage(Utils.GetRandomDamage(damage));
            }
        }
    }

}
