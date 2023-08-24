using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SMP_0 : MonoBehaviour
{
    public int damage;
    public float moveSpeed=50f;
    public float duration=5f;
    private Vector3 initPos;
    private Quaternion initRot;
    private HashSet<Collider> attackedList=new HashSet<Collider>();
    public NetworkObject NO;
    // Start is called before the first frame update
    void Awake()
    {
        initPos = transform.localPosition;
        initRot = transform.localRotation;
    }
    private void OnEnable()
    {

        attackedList.Clear();
        transform.localPosition = initPos;
        StartCoroutine(moveCRT());
        StartCoroutine(stopCRT());
    }
    IEnumerator moveCRT()
    {
        while (true)
        {
            transform.localRotation = initRot;
            transform.Translate(Vector3.forward * Time.deltaTime * moveSpeed);
            yield return null;
        }
    }
    IEnumerator stopCRT()
    {
        yield return new WaitForSeconds(duration);
        StopCoroutine(moveCRT());
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
