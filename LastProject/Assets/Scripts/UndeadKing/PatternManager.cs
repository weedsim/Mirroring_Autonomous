using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public struct PatternDetail{
    public Collider melee;
    public ParticleSystemManager[] particles;
    public GameObject creature;
    public bool createToAll;
}

public class PatternManager : NetworkBehaviour
{
    public PatternDetail[] patternInfo;
    public string patternName;
    public int damage;
    public float delay;
    public float afterDelay;
    public float chaseTime;
    [SerializeField]
    public Collider checkRange;
    public bool isCreateToAll;

    private int totalStep = 0;
    private int nowStep = 0;
    private void Start()
    {
        totalStep = patternInfo.Length;
    }
    public void startChecking()
    {
        checkRange.enabled = true ;
    }
    public void stopChecking()
    {
        checkRange.enabled = false;
    }

    public void nextStep()
    {
       
        if (totalStep<=nowStep) { Debug.Log("패턴 진행 에러"); return; }
        Debug.Log($"패턴 step {nowStep} 진행 중");
        if (patternInfo[nowStep].melee != null)
        {
            patternInfo[nowStep].melee.enabled = true;
            StartCoroutine(disable(nowStep));
        }
        if (patternInfo[nowStep].particles!= null)
        {
            foreach (ParticleSystemManager psm in patternInfo[nowStep].particles)
            {
                psm.PlayPS();   
            }
        }
        if (patternInfo[nowStep].creature!= null && Object.HasStateAuthority)
        {
            Debug.Log("소환 시작");
            Collider[] colliders = Physics.OverlapSphere(transform.position,100f,1<<3);
            if (patternInfo[nowStep].createToAll)
            {
                for (int i=0; i<colliders.Length; i++)
                {
                    //Instantiate(patternInfo[nowStep].creature, colliders[i].transform.position, Quaternion.identity).SetActive(true);
                    Runner.Spawn(patternInfo[nowStep].creature, colliders[i].transform.position, Quaternion.identity);
                }
            }
            else
            {
                //Instantiate(patternInfo[nowStep].creature, colliders[Random.Range(0, colliders.Length)].transform.position, Quaternion.identity).SetActive(true);
                Runner.Spawn(patternInfo[nowStep].creature, colliders[Random.Range(0, colliders.Length)].transform.position, Quaternion.identity);
            }
        }
        nowStep++;
    }
    public void endStep()
    {
        nowStep = 0;
    }
    IEnumerator disable(int now)
    {
        yield return new WaitForSeconds(0.5f);
        patternInfo[now].melee.enabled = false;
    }

    
}
