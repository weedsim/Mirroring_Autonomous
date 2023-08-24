using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodRainManager : NetworkBehaviour
{
    public ParticleSystem cloudps;
    public ParticleSystem rainps;
    public ParticleSystem groundps;
    public int damage = 20;
    public float delay = 1f;
    public float damageDuration = 10f;
    public float tick = 0.5f;
    public float range = 3f;
    // Start is called before the first frame update

    //public override void Spawned()
    //{
    //    Debug.Log("ºñ ½ºÆùµÊ");

    //}

    void Start()
    {
        Debug.Log("ºñ ½ÃÀÛµÊ");
        StartCoroutine(Raining());
        StartCoroutine(StopCRT());
    }

    IEnumerator Raining()
    {
        yield return new WaitForSeconds(delay);
        rainps.Play();
        groundps.Play();
        if (Object.HasStateAuthority)
        {
            while (true)
            {
                yield return new WaitForSeconds(tick);
                Collider[] colliders = Physics.OverlapSphere(transform.position, range, 1 << 3);
                foreach (Collider collider in colliders)
                {

                    HPHandler hpm = collider.gameObject.GetComponentInParent<HPHandler>();
                    if (hpm != null)
                    {
                        hpm.OnTakeDamage(Utils.GetRandomDamage(damage));
                    }
                }
            }
        }

    }
    IEnumerator StopCRT()
    {
        yield return new WaitForSeconds(damageDuration);
        StopCoroutine(Raining());
        Destroy(gameObject);
    }

}
