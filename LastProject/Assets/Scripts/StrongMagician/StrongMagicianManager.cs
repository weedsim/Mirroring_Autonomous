using ExitGames.Client.Photon;
using Fusion;
using JetBrains.Annotations;
using MBT;
using Opsive.UltimateCharacterController.Character.Abilities;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


[System.Serializable]
public struct Pattern
{
    public bool isReady;
    public float coolTime;
    public float chaseTime;
    public float atferDelay;
    public string animation;
    public GameObject[] elements;
}


public class StrongMagicianManager : NetworkBehaviour
{
    public BoolReference endTask = new BoolReference(VarRefMode.DisableConstant);
    [SerializeField]
    public Pattern[] patterns;
    public float closeDistance=5f;
    public float moveSpeed=5f;
    public float turnSpeed=7f;

    [Networked]
    public int nowPatternIdx { get; set;}
    //[Networked]
    public int nowStep; /*{ get; set; }*/
    //[Networked]
    public bool stopper; /*{ get; set; }*/
    public ParticleSystem upgradeParticle;


    public Transform target;
    private Rigidbody rb;
    private Animator ani;

    public override void Spawned()
    {
        base.Spawned();
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        ani = GetComponent<Animator>();
        nowStep = 0;
        stopper = false;
    }

    public override void FixedUpdateNetwork()
    {
        locationFix();
    }
    
    void locationFix()
    {
        Vector3 pos = transform.position;
        if (pos.y < -300.0f)
        {
            transform.position = Utils.GetStageTwoBossSpawnPoint();
        }
            

    }


    public void RPC_choicePatternAndTarget()
    {

        int tempIdx = Random.Range(0, patterns.Length);
        //Debug.Log("while 이전, tempidx  : " + tempIdx);
        while (!patterns[tempIdx].isReady)
        {
            tempIdx = Random.Range(0, patterns.Length);
        }
        //Debug.Log("while 이후, tempidx : " + tempIdx);

        nowPatternIdx = tempIdx;
        nowStep =0;
        Collider[] colliders = Physics.OverlapSphere(transform.position, 200f, 1 << 3);
        if (colliders.Length==0)
        {
            Debug.Log("남은 적이 없습니다. 보스 AI를 종료합니다");
            RPC_playAni("Idle");
            transform.Find("MBT").gameObject.SetActive(false);
        }
        target = colliders[Random.Range(0, colliders.Length)].transform;
        
        patterns[nowPatternIdx].isReady = false;
        Debug.Log($"{nowPatternIdx}번 고름");
        StartCoroutine(chaseTarget());
        RPC_playAni("걷기");
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_playAni(string aniname)
    {
        ani.CrossFade(aniname, 0.1f);
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_nextGenerator()
    {
        RPC_nextStep();
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_changeStep(int c)
    {
        nowStep = c;
        RPC_nextStep();
    }
    IEnumerator chaseTarget()
    {
        stopper = false;
        StartCoroutine(stopCRT(patterns[nowPatternIdx].chaseTime));
        while (!stopper)
        {
            Vector3 dir = new Vector3(target.position.x - rb.position.x, 0, target.position.z - rb.position.z);
            Vector3 dirN = dir.normalized;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * turnSpeed); //Fixed로 할거면 고쳐야할듯
            rb.MovePosition(rb.position + dirN * moveSpeed * Time.deltaTime);
            if (Vector3.Distance(target.position, transform.position) < closeDistance) break; 
            yield return null;
        }
        if (stopper) Debug.Log("추격 중지");
        else Debug.Log("추격 완료");
        rb.velocity = Vector3.zero;

        Debug.Log($"{nowPatternIdx}번 실행");

        yield return null;
        RPC_playAni(patterns[nowPatternIdx].animation);
    }

    public void RPC_nextStep()
    {
        if (nowStep == patterns[nowPatternIdx].elements.Length)
        {
            ani.CrossFade("Idle2", 0.1f);
            nowStep = 0;
            StartCoroutine(onCoolTime(nowPatternIdx));
            StartCoroutine(endPatterns());
        }
        else
        {
            patterns[nowPatternIdx].elements[nowStep].SetActive(true);
            nowStep++;
        }
    }
    public void setPOS()
    {
        if (Object.HasStateAuthority) RPC_SMP6setPOS(target.position);
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_SMP6setPOS(Vector3 pos)
    {
        patterns[6].elements[0].transform.position = pos;
    }
    public void RPC_action(string name)
    {
        if (Object.HasStateAuthority) StartCoroutine(name);
    }

    IEnumerator rotateCRT()
    {
        yield return new WaitForSeconds(2f);
        stopper = false;
        StartCoroutine(stopCRT(6.5f));
        while (!stopper)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, Time.time * turnSpeed * 6, 0), Time.deltaTime);
            yield return null;
        }
        RPC_nextGenerator();
    }

    IEnumerator jumpCRT()
    {
        Vector3 pos_to = Vector3.Lerp(transform.position,target.position,0.8f);
        Vector3 pos_from = transform.position;
        Vector3 center = (pos_to + pos_from)/2 -Vector3.up*0.1f;
        float moveTime = 0.4f;
        int frame = 60;
        float gap = 0.7f* moveTime / frame;
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezePositionX|RigidbodyConstraints.FreezePositionY|RigidbodyConstraints.FreezePositionZ|RigidbodyConstraints.FreezeRotationX|RigidbodyConstraints.FreezeRotationY|RigidbodyConstraints.FreezeRotationZ;
        for (int i = 1; i <= frame; i++)
        {
            Vector3 temp = Vector3.Slerp(pos_from-center, pos_to-center, (float)i / frame)+center;
            transform.position = temp;
            yield return new WaitForSeconds(gap);
        }
        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
    }
    IEnumerator counterCRT()
    {
        HPHandler hpm = GetComponent<HPHandler>();
        int initHP = hpm.HP;
        bool doCounter = false;
        stopper = false;
        StartCoroutine(stopCRT(4f));
        while (!stopper)
        {
            if (hpm.HP< initHP)
            {
                doCounter = true;
                break;
            }
            yield return null;
        }
        if (doCounter)
        {
            RPC_playAni("360베기");
        }
        else {
            RPC_changeStep(patterns[nowPatternIdx].elements.Length);
        }
    }
    public void UpgradeBossPattern()
    {
        for (int i = 0; i < patterns.Length; i++)
        {
            patterns[i].isReady= true;
        }
        RPC_playAni("강화");
    }
    public void UpgradeBossEffect()
    {
        upgradeParticle.Play();
    }
    public void UpgradeEnd()
    {
        endTask.Value = true;
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_Gimmick()
    {
        
        HPHandler hpm = GetComponent<HPHandler>();
        hpm.isProtected = true;
        StartCoroutine(StageManager.StartCutScene2or3or4(gameObject.name));
        transform.localScale = new Vector3(0, 0.01f, 0);
        transform.position = Utils.GetStageTwoBossSpawnPoint();
    }

    IEnumerator stopCRT(float delay)
    {
        yield return new WaitForSeconds(delay);
        stopper = true;
    }

    IEnumerator endPatterns()
    {
        yield return new WaitForSeconds(patterns[nowPatternIdx].atferDelay);
        endTask.Value = true;
    }

    IEnumerator onCoolTime(int patternIdx)
    {
        yield return new WaitForSeconds(patterns[patternIdx].coolTime);
        patterns[patternIdx].isReady = true;
    }
}
