using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MBT;
using Unity.VisualScripting;
using Fusion;

public class MobManager : NetworkBehaviour
{
    
    public BoolReference endTask = new BoolReference(VarRefMode.DisableConstant);
    public Animator animator;
    public GameObject model;
    public GameObject MBT;
    public ParticleSystem effect_bone;
    public ParticleSystem effect_exp1;
    public ParticleSystem effect_exp2;
    private Transform target;
    private Rigidbody rb;
    public float moveSpeed = 1f;
    public float turnSpeed = 1f;
    public float distanceToExplosion = 1.5f;
    public float ExplosionRad = 5f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void OnEnable()
    {
        model.SetActive(true);
        //MBT.SetActive(false); // HOST�� �ƴϸ� �̰� ����
        animator.Play("Stand Up");
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_Explosion()
    {
        animator.CrossFade("Scream", 0.1f);
        StartCoroutine(ExplosionCRT());
    }
    IEnumerator ExplosionCRT()
    {
        yield return new WaitForSeconds(2f);
        model.SetActive(false);
        effect_exp1.Play();
        effect_exp2.Play();
        
        if (Object.HasStateAuthority)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, ExplosionRad);
            foreach (Collider collider in colliders)
            {
                if (collider.CompareTag("Player"))
                {
                    // �÷��̾� ��ũ��Ʈ�� �ǰ��Լ� ȣ��
                    if (collider.gameObject.GetComponentInParent<HPHandler>() != null)
                    {
                        Debug.Log($"����� {collider.name}�� ���ȴ�!!");
                        collider.gameObject.GetComponentInParent<HPHandler>().OnTakeDamage(Utils.GetRandomDamage(500));
                    }
                }
            }
        }


        yield return new WaitForSeconds(2f);
        Runner.Despawn(GetComponent<NetworkObject>());
    }
    
    public void eventReady()
    {
        animator.CrossFade("Idle",0.1f);
    }
    public void eventStartBT()
    {
        endTask.Value = true;
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_choiceAndChaseTarget()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.localPosition, 300f, 1<<3);
        if (colliders.Length<=0)
        {
            Debug.Log("�÷��̾ �����ϴ� ��� AI ����");
            gameObject.SetActive(false);
        }
        target = colliders[Random.Range(0, colliders.Length)].transform; //���� �÷��̾� ����ȭ..?
        StartCoroutine(ChaseTargetCRT());
    }
    IEnumerator ChaseTargetCRT()
    {
        yield return null;
        animator.CrossFade("Walking", 0.1f);
        while (true)
        {
            if (target==null)
            {
                StartCoroutine(ExplosionCRT());
                break;
            }
            Vector3 dir = new Vector3(target.position.x - rb.position.x,0,target.position.z - rb.position.z);    
            if (dir.magnitude < distanceToExplosion) break;
            
            Vector3 dirN = dir.normalized;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * turnSpeed); //Fixed�� �ҰŸ� ���ľ��ҵ�
            rb.MovePosition(rb.position + dirN*moveSpeed*Time.deltaTime); // Fixed�� �ؾ��ϳ�?
            yield return null;
        }
        endTask.Value = true; // Host ������
    }

}
