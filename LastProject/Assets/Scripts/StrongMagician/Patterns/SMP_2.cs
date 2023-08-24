using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SMP_2 : MonoBehaviour
{
    public int damage;
    public float duration = 8.5f;
    public float bombDuration = 5f;
    public float hitRad = 10f;
    public Transform parentsTrs;
    public GameObject bomb;
    private Vector3 pos;
    public NetworkObject NO;
    public void Awake()
    {
        pos = transform.localPosition;
    }

    public void OnEnable()
    {
        StartCoroutine(explo());
        transform.SetParent(GameObject.Find("소환물저장소").transform);
    }
    IEnumerator explo()
    {
        yield return new WaitForSeconds(duration);
        bomb.SetActive(true);
        if (NO.HasStateAuthority)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, hitRad, 1 << 3);
            foreach (Collider collider in colliders)
            {
                HPHandler hpm = collider.gameObject.GetComponentInParent<HPHandler>();
                if (hpm != null)
                {
                    hpm.OnTakeDamage(Utils.GetRandomDamage(damage));
                }
            }
        }


        yield return new WaitForSeconds(bombDuration);
        bomb.SetActive(false);
        transform.SetParent(parentsTrs);
        transform.localPosition= pos;
        gameObject.SetActive(false);
    }
}
