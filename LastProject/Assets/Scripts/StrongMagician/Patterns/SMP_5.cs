using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SMP_5 : MonoBehaviour
{
    public int damage;
    public GameObject eye;
    public float duration=2f;
    public NetworkObject NO;


    private void OnEnable()
    {
        if (NO.HasStateAuthority)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, 100f, 1 << 3);
            foreach(Collider col in colliders)
            {
                if (Vector3.Dot(col.transform.forward, (transform.position - col.transform.position)) > 0)
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
        yield return new WaitForSeconds(duration);
        eye.SetActive(false);
        gameObject.SetActive(false);
    }

}
