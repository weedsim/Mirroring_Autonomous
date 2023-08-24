using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardWeaponHandler : NetworkBehaviour
{
    public int Damage = 5;
    float hitDuration = 1f;
    float hitTimer = 0.0f;
    bool hitted = false;
    public bool isCommonAttack = false;
    [SerializeField] Collider col;
    public NetworkObject _networkObject;

    private void Update()
    {
        if(hitTimer > 0.0f)
        {
            hitTimer -= Runner.DeltaTime;
        }

        if(hitTimer <= 0.0f)
        {
            hitted = false;
            col = null;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (_networkObject.HasInputAuthority && !isCommonAttack)
        {
            Debug.Log("Try To Common Attack");
            //Debug.Log(other);
            //Debug.Log(other.gameObject);
            //Debug.Log(other.gameObject.transform);
            //Debug.Log(other.gameObject.transform.parent);
            //Debug.Log(other.gameObject.transform.parent.gameObject);
            Debug.Log("temp");
            if (other.gameObject.CompareTag("Enemy"))
            {
                if (other.gameObject.GetComponent<HPHandler>() != null)
                {
                    other.gameObject.GetComponentInParent<HPHandler>().OnTakeDamage(Damage);
                    hitted = true;
                    col = other;
                    hitTimer = hitDuration;
                }
            }
            else if (other.gameObject.CompareTag("Stone"))
            {
                Debug.Log("FIX");
                if (other.gameObject.GetComponent<StoneManager>() != null && !hitted && col != other)
                {
                    Debug.Log("Wizard Stone Hit!!");
                    other.gameObject.GetComponent<StoneManager>().OnTakeDamage();
                    hitted = true;
                    col = other;
                    hitTimer = hitDuration;
                }
            }
        }
    }
}
