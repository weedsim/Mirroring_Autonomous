using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MBT;
using Unity.VisualScripting;
using Fusion;

public class UndeadKingManager : NetworkBehaviour
{
    public BoolReference endTask = new BoolReference(VarRefMode.DisableConstant);
    [SerializeField]
    public PatternManager[] pms;
    public float moveSpeed = 1.0f;
    public float turnSpeed = 3.0f;
    public Animator animator;
    [SerializeField]
    public GameObject[] gimmckResources;
    public ParticleSystem[] gimmcikParticle;

    private Rigidbody rb;
    private Transform target;

    public bool isIn;
    public bool isChecking;
    public bool upgradePattern;
    public bool gimmickOn;

    [Networked] public int nowPattern { get; set; }

    private HashSet<Collider> attackedList = new HashSet<Collider>();

    private void Start()
    {
        isIn = false;
        isChecking = false;
        upgradePattern = false;
        gimmickOn = false;
        rb = GetComponent<Rigidbody>();
    }
    public void resetVar()
    {
        isIn = false;
        isChecking = false;
        upgradePattern = false;
    }
    public void renewalTarget()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 300f, 1 << 3);
        if (colliders.Length == 0)
        {
            stopAi(); return;
        } 
        target = colliders[Random.Range(0, colliders.Length)].transform;
        Debug.Log($"타겟 설정 완료{target.name}");
    }

    public void RPC_doSmallPattern()
    {
        nowPattern = Random.Range(0, 4);
        Debug.Log($"{nowPattern}번 패턴 실행");
        renewalTarget();
        Debug.Log("타겟 설정 완료");
        StartCoroutine(chaseTargetCRT());
        StartCoroutine(chaseTimerCRT());
    }
    public void RPC_doBigPattern()
    {
        resetVar();
        nowPattern = Random.Range(4, 8);
        Debug.Log($"{nowPattern}번 패턴 실행");
        StartCoroutine(startPattern());
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_playAni(string name)
    {
        animator.CrossFade(name, 0.1f);
    }

    IEnumerator chaseTimerCRT()
    {
        yield return new WaitForSeconds(pms[nowPattern].chaseTime);
        upgradePattern = true;
    }
    IEnumerator chaseTargetCRT()
    {
        RPC_playAni("Walk");
        isChecking = true;
        pms[nowPattern].startChecking();
        while (!isIn && !upgradePattern)
        {
            if (target == null)
            {
                stopAi();
                StopCoroutine(chaseTargetCRT());
                break;
            }
            Vector3 dir = new Vector3(target.position.x - rb.position.x, 0, target.position.z - rb.position.z);
            Vector3 dirN = dir.normalized;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), Runner.DeltaTime * turnSpeed); //Fixed로 할거면 고쳐야할듯
            rb.MovePosition(rb.position + dirN * moveSpeed * Runner.DeltaTime);
            yield return null;
        }
        pms[nowPattern].stopChecking();
        if (upgradePattern)
        {
            Debug.Log("추적 실패");
            nowPattern = 8;
            upgradePattern = false;
        }
        else
        {
            Debug.Log("추적 성공");
            StopCoroutine(chaseTimerCRT());
        }
        rb.velocity = Vector3.zero;
        resetVar();
        StartCoroutine(startPattern());
    }


    IEnumerator startPattern()
    {
        RPC_playAni("Idle2");
        yield return new WaitForSeconds(pms[nowPattern].delay);
        RPC_playAni(pms[nowPattern].patternName);
    }

    public void nextStepListener()
    {
        attackedList.Clear();
        pms[nowPattern].nextStep();
    }
    public void endTaskListener()
    {
        pms[nowPattern].endStep();
        animator.CrossFade("Idle2", 0.1f);
        if (Object.HasStateAuthority) StartCoroutine(endTaskDelay());
    }
    IEnumerator endTaskDelay()
    {
        yield return new WaitForSeconds(pms[nowPattern].afterDelay);
        endTask.Value = true;
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_Gimmick()
    {
        animator.CrossFade("Roaring", 0.1f);
        foreach (ParticleSystem ps in gimmcikParticle) ps.Play();
        if (Object.HasStateAuthority) StartCoroutine(GimmickCRT());
    }

    IEnumerator GimmickCRT()
    {
        yield return new WaitForSeconds(3f);
        RPC_playAni("Agonizing");
        HPHandler hpm = GetComponent<HPHandler>();
        int high = 2*hpm.startingHP/3;
        int low = 2*hpm.startingHP/5;
        int tick = 2*hpm.startingHP / 100;

        while (hpm.HP < high && hpm.HP >=low)
        {
            yield return new WaitForSeconds(4f);
            hpm.OnTakeDamage(-tick);
            Collider[] colliders = Physics.OverlapSphere(transform.position, 100f, 1 << 3);
            for (int i = 0; i < colliders.Length; ++i)
            {
                int temp = Random.Range(0, 2);
                Runner.Spawn(gimmckResources[temp], colliders[i].transform.position, colliders[i].transform.rotation);
            }
            Runner.Spawn(gimmckResources[2], Utils.GetRandomSpawnPoint(), Quaternion.identity);

        }
        RPC_playAni("Roaring");
        yield return new WaitForSeconds(3f);
        endTask.Value = true;
    }
    public void stopAi()
    {
        RPC_playAni("Idle2");
        Debug.Log("남은 플레이어가 없습니다. 보스 AI 종료");
        transform.Find("MBT").gameObject.SetActive(false);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!Object.HasStateAuthority) return;
        if (isChecking)
        {
            if (other.transform == target) isIn = true;
        }
        else if (other.CompareTag("Player"))
        {
            if (!attackedList.Contains(other))
            {
                attackedList.Add(other);
                if (other.gameObject.GetComponentInParent<HPHandler>() != null)
                {
                    other.gameObject.GetComponentInParent<HPHandler>().OnTakeDamage(Utils.GetRandomDamage(pms[nowPattern].damage));
                }
            }
        }
    }
    
}
