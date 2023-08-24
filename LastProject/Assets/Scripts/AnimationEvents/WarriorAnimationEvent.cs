using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarriorAnimationEvent : MonoBehaviour
{
    public GameObject SkillQ_Effect;
    public GameObject SkillE_Effect;

    void QActive()
    {
        SkillQ_Effect.SetActive(true);
    }
 
    void QDeActive()
    {
        //SkillQ_Effect.SetActive(false);
    }

    void EDeActive()
    {
        //StartCoroutine(TimeCheck(1.0f));
        //SkillE_Effect.SetActive(false);
    }

    IEnumerator TimeCheck(float time)
    {
        yield return new WaitForSeconds(time);
    }
}
