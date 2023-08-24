using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SMP_10 : MonoBehaviour
{
    public int damage;
    public float moveSpeed = 50f;
    public float duration = 5f;
    public Transform[] tds;
    private bool isGoing =false;
    private HashSet<Collider> attackedList = new HashSet<Collider>();
    public NetworkObject NO;
    // Start is called before the first frame update


    private void OnEnable()
    {
        attackedList.Clear();
        StartCoroutine(moveCRT());
        StartCoroutine(stopCRT());
    }
    IEnumerator moveCRT()
    {
        isGoing= true;
        while (isGoing)
        {
            foreach (Transform t in tds)
            {
                t.Translate(Vector3.forward * Time.deltaTime * moveSpeed);
            }
            yield return null;
        }
    }
    IEnumerator stopCRT()
    {
        yield return new WaitForSeconds(duration);
        isGoing = false;
        yield return null;
        foreach (Transform t in tds)
        {
            t.localPosition = Vector3.zero;
        }
        yield return null;
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
