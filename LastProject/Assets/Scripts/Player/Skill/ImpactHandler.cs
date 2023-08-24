using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactHandler : MonoBehaviour
{

    Collision _Impact = null;

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Skill"))
        {
            _Impact = collision;
        }
    }

    public Collision impact()
    {
        return _Impact;
    }
}
