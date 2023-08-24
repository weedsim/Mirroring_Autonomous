using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SMP_12 : MonoBehaviour
{
    public int damage;
    public float hitDuration=1f;
    public NetworkObject NO;

    private void OnEnable()
    {

        if (NO.HasStateAuthority)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, 50f, 1 << 3);
            foreach (Collider collider in colliders)
            {
                HPHandler hpm = collider.gameObject.GetComponentInParent<HPHandler>();
                if (hpm != null)
                {
                    hpm.OnTakeDamage(Utils.GetRandomDamage(damage));
                }
            }
        }

        StartCoroutine(off());
    }
    IEnumerator off()
    {
        yield return new WaitForSeconds(hitDuration);
        gameObject.SetActive(false);
    }
}
