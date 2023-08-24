using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
    public int damage=100;
    public float hitBoxDuration = 0.4f;
    private HashSet<Collider> attackedList= new HashSet<Collider>();
    // Start is called before the first frame update
    public NetworkObject NO;

    private void OnEnable()
    {
        attackedList.Clear();
        StartCoroutine(offHitBox());
    }
    IEnumerator offHitBox()
    {
        yield return new WaitForSeconds(hitBoxDuration);
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
                hpm.OnTakeDamage(Utils.GetRandomDamage(damage));
            }
        }
    }
}
