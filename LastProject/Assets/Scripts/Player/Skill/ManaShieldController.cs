using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaShieldController : MonoBehaviour
{
    [Header("Mana Shield Particle")]
    public ParticleSystem ManaShield;
    public bool OnAndOff;

    // Start is called before the first frame update
    void Start()
    {
        ManaShield.Stop();
        OnAndOff = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (OnAndOff)
        {
            ManaShield.Play();
        }
        else
        {
            ManaShield.Stop();
        }
    }

    public void On()
    {
        OnAndOff = true;
    }

    public void Off()
    {
        OnAndOff = false;
    }


}
