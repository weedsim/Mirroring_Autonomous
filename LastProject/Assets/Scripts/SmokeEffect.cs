using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeEffect : MonoBehaviour
{
    public bool playSmoke = true;
    public ParticleSystem SmokeParticleObject;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            Invoke("ExplosionSmoke", 4f);
        
    }

    public void ExplosionSmoke()
    {
        if (playSmoke)
            SmokeParticleObject.Play();
        else
            SmokeParticleObject.Stop();
    }
}
