using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeapronHandler : MonoBehaviour
{
    HashSet<Collider> attackedList = new HashSet<Collider>();
    public int Damage = 5;
    public float hitDuration = 1f;
    public bool isCommon=false;
    public Collider col;
    public NetworkObject no;
    public int playerId=-1;

    
    private void OnEnable()
    {
        if (playerId == -1) playerId = no.InputAuthority.PlayerId;
        if (col.enabled == true)
        {
            if (!no.HasStateAuthority)
                col.enabled = false;
            attackedList.Clear();
        }
        StartCoroutine(StopCRT());
    }
    IEnumerator StopCRT()
    {
        yield return new WaitForSeconds(hitDuration);
        gameObject.SetActive(false);
    }
    

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Enemy") && !attackedList.Contains(col))
        {
            attackedList.Add(col);
            if (col.gameObject.GetComponentInParent<HPHandler>() != null)
            {
                col.gameObject.GetComponentInParent<HPHandler>().OnTakeDamage(Utils.GetRandomDamage(Damage),playerId);
            }
        }
        else if (isCommon && col.gameObject.CompareTag("Stone") && !attackedList.Contains(col))
        {
            attackedList.Add(col);
            if (col.gameObject.GetComponent<StoneManager>() != null)
            {
                col.gameObject.GetComponent<StoneManager>().OnTakeDamage();
            }
        }
    }
}
