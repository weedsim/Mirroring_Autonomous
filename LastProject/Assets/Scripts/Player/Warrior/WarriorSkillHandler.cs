using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using TMPro;
using Opsive.UltimateCharacterController.Character.Abilities.Items;
using Unity.VisualScripting;

public class WarriorSkillHandler : NetworkBehaviour
{
    [Networked(OnChanged = nameof(OnCommomAttack))]
    public bool commonAttack { get; set; }

    [Networked(OnChanged = nameof(OnFullTurn))]
    public bool Qskill { get; set; }

    [Networked(OnChanged = nameof(OnJumpAttack))]
    public bool Eskill { get; set; }

    [Networked(OnChanged = nameof(OnAvoid))]
    public bool Avoidskill { get; set; }

    float CACur;
    float CACool = 0.5f;

    [Header("Skill Q")]
    public float QCur;
    public float QCool = 1.0f;

    [Header("Skill E")]
    public float ECur;
    public float ECool = 1.0f;
    public GameObject SkillE_Effect;

    [Header("Avoidance")]
    public float AvoidCur;
    public float AvoidCool = 1.0f;

    [Header("Skill Animation")]
    Animator _anim;

    NetworkObject player;
    HPHandler hpHandler;

    public GameObject commonAttackObject;

    public CharacterController Controller { get; private set; }

    void Awake()
    {
        player = GetComponent<NetworkObject>();
        hpHandler = GetComponent<HPHandler>();
        _anim = GetComponentInChildren<Animator>();
        SkillE_Effect.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        QCur = 0.0f;
        ECur = 0.0f;
        AvoidCur = AvoidCool;
    }

    private void Update()
    {
        if (CACur > 0.0f)
        {
            CACur -= Time.deltaTime;
        }

        if(QCur > 0.0f)
        {
            QCur -= Time.deltaTime;
            /*if(QCur > QCool - Runner.DeltaTime * 58.0f)
            {
                _anim.SetBool("CanWalk", false);
            }
            else
            {
                _anim.SetBool("CanWalk", true);
            }*/
        }
        else
        {
            QCur = 0.0f;
            
            //_anim.SetBool("CanWalk", true);
        }

        if (ECur > 0.0f)
        {
            ECur -= Time.deltaTime;
            /*if (ECur > ECool - Runner.DeltaTime * 55.0f)
            {
                _anim.SetBool("CanWalk", false);
            }
            else
            {
                _anim.SetBool("CanWalk", true);
            }*/
        }
        else
        {
            ECur = 0.0f;
            
            //_anim.SetBool("CanWalk", true);
        }

        if (AvoidCur > 0.0f)
        {
            AvoidCur -= Time.deltaTime;
        }
        else
        {
            AvoidCur = 0.0f;
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData networkInputData))
        {
            if (networkInputData.isCommonAttack)
            {
                if (CACur <= 0.0f)
                {
                    CommonAttack();
                }
            }

            if (networkInputData.isSkillQ)
            {
                if(QCur <= 0.0f)
                {
                    FullTurn();
                }
            }

            if (networkInputData.isSkillE)
            {
                if(ECur <= 0.0f)
                {
                    JumpAttack();
                }
            }

            
            if (networkInputData.isVoidOrDef)
            {
                if (AvoidCur <= 0.0f)
                {
                    Avoid();
                }
                if (AvoidCur > AvoidCool - (Runner.DeltaTime * 71.0f))
                {
                    transform.position += transform.forward * networkInputData.movementInput.z * Runner.DeltaTime / 2.0f;
                    transform.position += transform.right * networkInputData.movementInput.x * Runner.DeltaTime / 2.0f;
                }
            }
        }
        
    }

    void CommonAttack()
    {
        CACur = CACool;
        StartCoroutine(CommonAttackCo());
    }

    IEnumerator CommonAttackCo()
    {
        commonAttack = true;
        _anim.SetTrigger("CommonAttack");
        commonAttackObject.SetActive(true);
        yield return new WaitForSeconds(0.05f);
        commonAttack = false;
    }

    static void OnCommomAttack(Changed<WarriorSkillHandler> changed)
    {
        bool isCommonAttackCurrent = changed.Behaviour.commonAttack;
        changed.LoadOld();

        bool isCommonAttackOld = changed.Behaviour.commonAttack;

        if (isCommonAttackCurrent && !isCommonAttackOld)
            changed.Behaviour.DoCommonAttack();
    }
    void DoCommonAttack()
    {
        if (!Object.HasInputAuthority)
        {
            _anim.SetTrigger("CommonAttack");
        }
    }

    void FullTurn() // Skill Q
    {
        QCur = QCool;
        StartCoroutine(FullTurnCo());
    }

    IEnumerator FullTurnCo()
    {
        Qskill = true;
        _anim.SetTrigger("SkillQ");
        yield return new WaitForSeconds(0.05f);
        Qskill = false;
    }

    static void OnFullTurn(Changed<WarriorSkillHandler> changed)
    {
        bool isOnFullTurnCurrent = changed.Behaviour.Qskill;
        changed.LoadOld();
        bool isOnFullTurnOld = changed.Behaviour.Qskill;

        if (isOnFullTurnCurrent && !isOnFullTurnOld)
            changed.Behaviour.DoFullTurn();
    }
    void DoFullTurn()
    {
        if (!Object.HasInputAuthority)
        {
            _anim.SetTrigger("SkillQ");
        }
    }

    void JumpAttack() // Skill E
    {
        ECur = ECool;
        StartCoroutine(JumpAttackCo());
    }

    IEnumerator JumpAttackCo()
    {
        Eskill = true;
        _anim.SetTrigger("SkillE");
        SkillE_Effect.SetActive(true);
        yield return new WaitForSeconds(0.05f);
        Eskill = false;
    }

    static void OnJumpAttack(Changed<WarriorSkillHandler> changed)
    {
        bool isOnJumpAttackCurrent = changed.Behaviour.Eskill;
        changed.LoadOld();
        bool isOnJumpAttackOld = changed.Behaviour.Eskill;

        if (isOnJumpAttackCurrent && !isOnJumpAttackOld)
            changed.Behaviour.DoJumpAttack();
    }

    void DoJumpAttack()
    {
        if (!Object.HasInputAuthority)
        {
            _anim.SetTrigger("SkillE");
            SkillE_Effect.SetActive(true);
        }
    }

    void Avoid()
    {
        AvoidCur = AvoidCool;
        _anim.SetTrigger("Avoid");
    }

    IEnumerator AvoidCo()
    {
        Avoidskill = true;
        _anim.SetTrigger("Avoid");
        yield return new WaitForSeconds(0.05f);
        Avoidskill = false;
    }
    static void OnAvoid(Changed<WarriorSkillHandler> changed)
    {
        bool isOnAvoidCurrent = changed.Behaviour.Avoidskill;
        changed.LoadOld();
        bool isOnAvoidOld = changed.Behaviour.Avoidskill;

        if (isOnAvoidCurrent && !isOnAvoidOld)
            changed.Behaviour.DoCommonAttack();
    }
    void DoAvoid()
    {
        if (!Object.HasInputAuthority)
        {
            _anim.SetTrigger("Avoid");
        }
    }
}
