using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SMP_13 : MonoBehaviour
{
    public int damage;
    public float effectDuration = 3.5f;
    public float bombHitBoxDuration=0.5f;
    public float nextBombTime = 0.25f;
    public GameObject nextBomb;
    public Collider col;
    private HashSet<Collider> attackedList = new HashSet<Collider>();
    public NetworkObject NO;
    // Start is called before the first frame update

    private void OnEnable()
    {

        col.enabled = true;
        StartCoroutine(off());
        if (nextBomb != null) StartCoroutine(next());
    }
    IEnumerator next()
    {
        yield return new WaitForSeconds(nextBombTime);
        nextBomb.SetActive(true);
    }
    IEnumerator off()
    {
        yield return new WaitForSeconds(bombHitBoxDuration);
        col.enabled = false;
        yield return new WaitForSeconds(effectDuration);
        gameObject.SetActive(false);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (!NO.HasStateAuthority) return;
        if (other.CompareTag("Player") && !attackedList.Contains(other))
        {
            attackedList.Add(other);
            HPHandler hpm = other.gameObject.GetComponentInParent<HPHandler>();
            if (hpm != null)
            {
                hpm.OnTakeDamage(damage);
            }
        }
    }

}
