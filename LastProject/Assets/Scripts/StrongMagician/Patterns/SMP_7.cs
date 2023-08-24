using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SMP_7 : MonoBehaviour
{
    public int damage;
    public float Duration=3f;
    public float unSafeRad = 10f;
    public float safeRad = 5f;

    public NetworkObject NO;

    private void OnEnable()
    {
        if (NO.HasStateAuthority)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, unSafeRad, 1 << 3);
            foreach (Collider col in colliders)
            {
                if (Vector3.Distance(transform.position, col.transform.position) > safeRad)
                {
                    HPHandler hpm = col.gameObject.GetComponentInParent<HPHandler>();
                    if (hpm != null)
                    {
                        hpm.OnTakeDamage(Utils.GetRandomDamage(damage));
                    }
                }
            }
        }

        StartCoroutine(off());

    }
    IEnumerator off()
    {
        yield return new WaitForSeconds(Duration);
        gameObject.SetActive(false);
    }


}
