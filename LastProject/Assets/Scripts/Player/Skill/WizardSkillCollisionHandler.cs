using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardSkillCollisionHandler : MonoBehaviour
{
    [Header("E Effect")]
    public ParticleSystem SkillEParticle;




    //private void OnParticleCollision(Collision collision)
    //{
    //    if (collision.collider.gameObject.CompareTag("Enum"))
    //    {
    //        Debug.Log("Hit! Particle");
    //    }
    //}
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject);
        Debug.Log("충돌 물체" + collision.collider.gameObject);
        if (collision.collider.gameObject.CompareTag("Enum"))
        {
            Debug.Log("Hit!");
        }
    }
}
