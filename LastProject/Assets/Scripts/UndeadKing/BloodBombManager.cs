using Fusion;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BloodBombManager : NetworkBehaviour
{
    public ParticleSystem before;
    public ParticleSystem Bomb1;
    public ParticleSystem Bomb2;
    public int damage = 350;
    public float delay;
    public float explosionDistance = 3f;
    // Start is called before the first frame update
    private void Start()
    {
        StartCoroutine(explosion());
    }
    IEnumerator explosion()
    {
        yield return new WaitForSeconds(delay);
        before.Stop();
        Bomb1.Play();
        Bomb2.Play();
        if (Object.HasStateAuthority)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, explosionDistance, 1 << 3);
            foreach (Collider collider in colliders)
            {
                HPHandler hpm = collider.gameObject.GetComponentInParent<HPHandler>();
                if (hpm != null)
                {
                    hpm.OnTakeDamage(Utils.GetRandomDamage(damage));
                }
            }
        }
        
        Destroy(gameObject, Bomb1.main.duration);
    }
}
