using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDeathComponent : MonoBehaviour
{
    void OnEnable()
    {
        transform.parent.GetComponent<BossDeath>()._Launch -= Launch;
        transform.parent.GetComponent<BossDeath>()._Launch += Launch;
    }

    void Launch()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.AddForce(transform.up * 100);
        rb.AddTorque(transform.forward * 100);
    }
}
