using Fusion;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SMP_11 : NetworkBehaviour
{
    public int tickDamage;
    public float tickTime = 0.5f;
    public int tickCount = 30;
    public float hitRad = 3f;
    public float moveSpeed = 0.000001f;
    public Transform parentsTrs;
    private bool isOn;
    private Vector3 localPos;
    private Quaternion localRot;

    private void Awake()
    {
        localPos = transform.localPosition;
        localRot = transform.localRotation;
    }
    public void OnEnable()
    {
        isOn = true;
        transform.SetParent(GameObject.Find("소환물저장소").transform);
        StartCoroutine(hit());
        if (Object.HasStateAuthority) StartCoroutine(move());
    }
    IEnumerator move()
    {
        Transform target = GameObject.FindGameObjectWithTag("Enemy").GetComponent<StrongMagicianManager>().target;
        while (isOn)
        {
            float t = (moveSpeed*Time.deltaTime)/Vector3.Distance(transform.position, target.position);
            transform.position = Vector3.Lerp(transform.position,target.position,t);
            yield return new WaitForSeconds(0.05f);
        }
    }
    IEnumerator hit()
    {
        for (int i = 0; i < tickCount; i++)
        {
            yield return new WaitForSeconds(tickTime);
            if (Object.HasStateAuthority)
            {
                Collider[] colliders = Physics.OverlapSphere(transform.position, hitRad, 1 << 3);
                foreach (Collider collider in colliders)
                {
                    HPHandler hpm = collider.gameObject.GetComponentInParent<HPHandler>();
                    if (hpm != null)
                    {
                        hpm.OnTakeDamage(Utils.GetRandomDamage(tickDamage));
                    }
                }
            }

        }
        yield return new WaitForSeconds(tickTime);
        isOn = false;
        yield return new WaitForSeconds(1f);
        transform.SetParent(parentsTrs);
        transform.localPosition = localPos;
        transform.localRotation = localRot;
        gameObject.SetActive(false);
    }
}
