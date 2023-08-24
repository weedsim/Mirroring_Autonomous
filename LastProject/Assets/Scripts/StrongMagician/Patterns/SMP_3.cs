using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SMP_3 : MonoBehaviour
{
    public float duration = 8.5f;
    public float tickTime = 0.5f;
    public int damage;
    private bool isOn = false;
    private HashSet<Collider> attackedList = new HashSet<Collider>();
    public NetworkObject NO;


    private void OnEnable()
    {

        StartCoroutine(off());
        attackedList.Clear();
        isOn= true;
    }
    IEnumerator off()
    {
        yield return new WaitForSeconds(duration);
        isOn = false;
        gameObject.SetActive(false);
    }
    IEnumerator tick()
    {
        while (isOn)
        {
            yield return new WaitForSeconds(tickTime);
            attackedList.Clear();
        }
    }

    private void OnTriggerStay(Collider other)
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
