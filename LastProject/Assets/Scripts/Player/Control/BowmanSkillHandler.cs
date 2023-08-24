using Fusion;
using Opsive.UltimateCharacterController.Character.Abilities.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowmanSkillHandler : NetworkBehaviour
{
    [Networked(OnChanged = nameof(OnAim))]
    public bool commonAttack { get; set; }

    [Networked(OnChanged = nameof(OnThreeBurst))]
    public bool Qskill { get; set; }

    [Networked(OnChanged = nameof(OnBigArrow))]
    public bool Eskill { get; set; }

    [Networked(OnChanged = nameof(OnAvoid))]
    public bool Avoidskill { get; set; }

    [Header("Skill Q")]
    public float QCur;
    public float QCool = 1.0f;
    public GameObject _ThreeBurst;

    [Header("Skill E")]
    public float ECur;
    public float ECool = 1.0f;
    public GameObject _BigArrow;
    public GameObject arrowStartPosition;
    public Animator _anim;

    [Header("Avoid")]
    public float AvoidCur;
    public float AvoidCool = 1.0f;

    [Header("GameObjects")]
    public GameObject rayCasttarget;




    void Awake()
    {
        _anim = GetComponentInChildren<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        QCur = 0.0f;
        ECur = 0.0f;
        AvoidCur = 0.0f;
    }

    private void Update()
    {
        if (QCur > 0.0f)
        {
            QCur -= Time.deltaTime;
        }
        else
        {
            QCur = 0.0f;
        }

        if (QCur > 0.0f)
        {
            QCur -= Time.deltaTime;
        }
        else
        {
            QCur = 0.0f;
        }

        if (ECur > 0.0f)
        {
            ECur -= Time.deltaTime;
            /*if (ECur < ECool - 1.5f)
            {
                _anim.SetBool("CanWalk", true);
            }*/
        }
        else
        {
            ECur = 0.0f;
            //SkillR_Effect.SetActive(false);
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
            Aim(networkInputData.isCommonAttackUp);

            if (networkInputData.isSkillQ)
            {
                if (QCur <= 0.0f)
                    ThreeBurst();
            }

            if (networkInputData.isSkillE)
            {
                if (ECur <= 0.0f)
                    BigArrow();
            }

            if (networkInputData.isVoidOrDef)
            {
                if (AvoidCur <= 0.0f)
                    Avoid();
                if (AvoidCur > AvoidCool - (Runner.DeltaTime * 71.0f))
                {
                    transform.position += transform.forward * networkInputData.movementInput.z * Runner.DeltaTime / 2.0f;
                    transform.position += transform.right * networkInputData.movementInput.x * Runner.DeltaTime / 2.0f;
                }
            }
        }
    }

    void ThreeBurst() // Skill Q
    {
        StartCoroutine(ThreeBurstCo());
    }

    void BigArrow() // Skill E
    {
        StartCoroutine(BigArrowCo());
    }

    IEnumerator ThreeBurstCo()
    {
        Qskill = true;
        QCur = QCool;
        _anim.SetTrigger("SkillQ");
        yield return new WaitForSeconds(0.05f);
        Qskill = false;
    }

    static void OnThreeBurst(Changed<BowmanSkillHandler> changed)
    {
        bool isThreeBurstCurrent = changed.Behaviour.Qskill;
        changed.LoadOld();
        bool isThreeBurstOld = changed.Behaviour.Qskill;

        if (isThreeBurstCurrent && !isThreeBurstOld)
            changed.Behaviour.DoThreeBurst();
    }

    void DoThreeBurst()
    {
        if (!Object.HasInputAuthority)
        {
            _anim.SetTrigger("SkillQ");
        }
    }

    IEnumerator BigArrowCo()
    {
        Eskill = true;
        ECur = ECool;

        _anim.SetTrigger("SkillE");

        Debug.LogError("skillE ���");
        Vector3 createPosition = rayCasttarget.transform.position - rayCasttarget.transform.forward * (4.5f * Mathf.Sin(DegreeToRadian(70)));

        RaycastHit hit;
        if (Physics.Raycast(createPosition, (rayCasttarget.transform.position - createPosition), out hit, 1000))
        {
            Runner.Spawn(_BigArrow, arrowStartPosition.transform.position, Quaternion.identity, Object.InputAuthority, (runner, spawnedWhatArrow) =>
            {
                spawnedWhatArrow.GetComponent<BigArrowHandler>().Fire(hit.point);
            });
        }

        yield return new WaitForSeconds(0.05f);

        Eskill = false;
    }

    static void OnBigArrow(Changed<BowmanSkillHandler> changed)
    {
        bool isBigArrowCurrent = changed.Behaviour.Eskill;
        changed.LoadOld();
        bool isBigArrowOld = changed.Behaviour.Eskill;

        if (isBigArrowCurrent && !isBigArrowOld)
            changed.Behaviour.DoBigArrow();
    }

    void DoBigArrow()
    {
        if (!Object.HasInputAuthority)
            _anim.SetTrigger("SkillE");
    }

    void Aim(bool clickCheck)
    {
        commonAttack = clickCheck;
        _anim.SetBool("IsClicked", clickCheck);
    }

    static void OnAim(Changed<BowmanSkillHandler> changed)
    {
        changed.Behaviour.DoAim(changed.Behaviour.commonAttack);
    }

    void DoAim(bool clickCheck)
    {
        if (!Object.HasInputAuthority)
            _anim.SetBool("IsClicked", clickCheck);
    }

    void Avoid()
    {
        StartCoroutine(AvoidCo());
    }

    IEnumerator AvoidCo()
    {
        Avoidskill = true;
        AvoidCur = AvoidCool;
        _anim.SetTrigger("Avoid");
        yield return new WaitForSeconds(0.05f);
        Avoidskill = false;
    }

    static void OnAvoid(Changed<BowmanSkillHandler> changed)
    {
        bool isAvoidCurrent = changed.Behaviour.Avoidskill;
        changed.LoadOld();
        bool isAvoidOld = changed.Behaviour.Avoidskill;

        if (isAvoidCurrent && !isAvoidOld)
            changed.Behaviour.DoAvoid();
    }

    void DoAvoid()
    {
        _anim.SetTrigger("Avoid");
    }

    float DegreeToRadian(float degree) { return Mathf.PI * degree / 180.0f; }
}
