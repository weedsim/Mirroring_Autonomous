using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Step
{
    public Collider col;
    public float onTime;
    public float offTime;
}
public class ParticleSystemManager : NetworkBehaviour
{
    public GameObject pre;
    public float delay = 1f;
    public float preDuration = 2f;
    private ParticleSystem ps;
    [SerializeField]
    public Step[] steps;

    private void Start()
    {
        ps = GetComponent<ParticleSystem>();
    }

    public void PlayPS()
    {
        if (pre != null)
        {
            pre.SetActive(true);
            StartCoroutine(offPre());
        }
        StartCoroutine(onColCRT());
    }
    IEnumerator onColCRT()
    {
        yield return new WaitForSeconds(delay);
        ps.Play();
        if (steps != null)
        {
            for (int i = 0; i < steps.Length; ++i)
            {
                yield return new WaitForSeconds(steps[i].onTime - ((i > 1) ? steps[i - 1].onTime : 0));
                steps[i].col.enabled = true;
                StartCoroutine(offColCRT(i, steps[i].offTime - steps[i].onTime));
            }
        }
    }
    IEnumerator offPre()
    {
        yield return new WaitForSeconds(preDuration);
        pre.SetActive(false);
    }
    IEnumerator offColCRT(int step, float duraiton)
    {
        yield return new WaitForSeconds(duraiton);
        steps[step].col.enabled = false;
    }

    

}
