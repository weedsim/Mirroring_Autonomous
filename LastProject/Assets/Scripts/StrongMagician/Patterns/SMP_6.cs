using Fusion;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SMP_6 : MonoBehaviour
{
    public int tickDamage;
    public float tickTime=0.5f;
    public int tickCount=30;
    public float hitRad = 3f;
    public Transform parentsTrs;
    public NetworkObject NO;

    public void OnEnable()
    {

        StartCoroutine(hit());
        transform.SetParent(GameObject.Find("소환물저장소").transform);
    }
    IEnumerator hit()
    {
        for (int i=0; i<tickCount; i++)
        {
            yield return new WaitForSeconds(tickTime);
            if (NO.HasStateAuthority)
            {
                Collider[] colliders = Physics.OverlapSphere(transform.position, hitRad,1<<3);
                foreach (Collider collider in colliders) {
                    HPHandler hpm = collider.gameObject.GetComponentInParent<HPHandler>();
                    if (hpm != null)
                    {
                        hpm.OnTakeDamage(Utils.GetRandomDamage(tickDamage));
                    }
                }
            }

        }
        yield return new WaitForSeconds(tickTime);
        transform.SetParent(parentsTrs);
        gameObject.SetActive(false);
    }
}
