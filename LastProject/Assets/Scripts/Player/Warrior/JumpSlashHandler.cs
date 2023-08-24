using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpSlashHandler : MonoBehaviour
{
    public int Damage = 50;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (collision.gameObject.GetComponentInParent<HPHandler>() != null)
            {
                collision.gameObject.GetComponentInParent<HPHandler>().OnTakeDamage(Damage);
            }
        }
    }
}
