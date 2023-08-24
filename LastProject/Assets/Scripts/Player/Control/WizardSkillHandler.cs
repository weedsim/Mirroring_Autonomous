using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using TMPro;
using Opsive.UltimateCharacterController.Character.Abilities.Items;
using Unity.VisualScripting;

public class WizardSkillHandler : NetworkBehaviour
{
    [Networked(OnChanged = nameof(OnCommomAttack))]
    public bool commonAttack { get; set; }

    [Networked(OnChanged = nameof(OnFireball))]
    public bool Qskill { get; set; }

    [Networked(OnChanged = nameof(OnMeteor))]
    public bool Eskill { get; set; }

    [Header("Skill Animation")]
    Animator _anim;

    [Header("SkillQ")]
    public GameObject _FireBall;
    public Transform camerapos;
    public GameObject target;
    public GameObject fireballStartPosition;

    [Header("Left Ctrl")]
    public GameObject _ManaShield;
    [SerializeField] NetworkObject _manaShield;

    [Header("SkillE")]
    public GameObject _Meteor;

    [Header("Skill Cooltime")]
    public float CCur;
    public float QCur;
    public float ECur;
    public float CCool = 1.0f;
    public float QCool = 1.0f;
    public float ECool = 10.0f;

    public float CtrlCur;
    public float CtrlCool = 1.0f;
    [SerializeField] float Ctrl_Remaining = 0.0f;
    float Ctrl_MaxTime = 3.0f;

    public GameObject CommonEffect;

    // 
    NetworkObject player;
    HPHandler hpHandler;
    public float usedManaShield;

    // 
    public TextMeshProUGUI tmptext;

    [Header("Sound Source")]
    public AudioSource _runAudioSource;



    void Awake()
    {
        player =  GetComponent<NetworkObject>();
        hpHandler = GetComponent<HPHandler>();
        _anim = GetComponentInChildren<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        CCur = 0;
        QCur = 0;
        ECur = 0;
        CtrlCur = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Ctrl_Remaining > 0.0f)
            Ctrl_Remaining -= Time.deltaTime;

        if (CCur > 0)
        {
            CCur -= Time.deltaTime;
        }
        else
            CCur = 0.0f;

        if (QCur > 0)
        {
            QCur -= Time.deltaTime;

            if (QCur <= QCool - 0.5f)
            {
                _anim.SetBool("SkillQ", false);
            }
        }
        else
        {
            QCur = 0.0f;
            _anim.SetBool("SkillQ", false);
        }


        if (CtrlCur > 0)
        {
            CtrlCur -= Time.deltaTime;

        }
        else
        {
            CtrlCur = 0.0f;
        }


        if(ECur > 0)
        {
            ECur -= Time.deltaTime;

            if (ECur <= ECool - (Runner.DeltaTime * 88.0f))
            {
                _anim.SetBool("SkillE", false);
                _anim.SetBool("CanWalk", true);
            }
            else
            {
                _anim.SetBool("CanWalk", false);
            }
        }

        else
        {
            ECur = 0.0f;
            _anim.SetBool("SkillE", false);
            _anim.SetBool("CanWalk", true);
        }

    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData networkInputData))
        {
            if (networkInputData.isCommonAttack)
            {
                if (CCur <= 0.0f)
                    CommonAttack();
            }

            if (networkInputData.isSkillQ)
            {
                if(QCur <= 0.0f)
                    Fireball();
            }


            ManaShield(networkInputData.leftCtrlDown, networkInputData.leftCtrlUp);
            ManaShieldExpire();

            if (networkInputData.isSkillE)
            {
                if (ECur <= 0.0f)
                    Meteor();
            }

        }
    }

    void CommonAttack()
    {
        StartCoroutine(CommonAttackCo());
    }
    IEnumerator CommonAttackCo()
    {
        commonAttack = true;
        CCur = CCool;
        _anim.SetTrigger("CommonAttack");
        CommonEffect.SetActive(true);
        yield return new WaitForSeconds(0.05f);
        commonAttack = false;
    }

    static void OnCommomAttack(Changed<WizardSkillHandler> changed)
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


    void Meteor() // Skill E
    {
        StartCoroutine(MeteorCo());
    }
    IEnumerator MeteorCo()
    {
        Eskill = true;

        ECur = ECool;
        _anim.SetBool("SkillE", true);

        Vector3 createPosition = target.transform.position - target.transform.forward * (4.5f * Mathf.Sin(DegreeToRadian(70)));

        RaycastHit hit;
        if (Physics.Raycast(createPosition, (target.transform.position - createPosition), out hit, 1000))
        {
            Vector3 _target = hit.point;
            _target.y += 10.0f;
            Runner.Spawn(_Meteor, _target, Quaternion.identity, Object.InputAuthority, (runner, spawnedMeteor) =>
            {
                spawnedMeteor.GetComponent<MeteorHandler>().Summon(_target);
                spawnedMeteor.GetComponent<MeteorHandler>().playerId = Object.InputAuthority.PlayerId;
            });
        }

        yield return new WaitForSeconds(0.05f);
        Eskill = false;
    }

    static void OnMeteor(Changed<WizardSkillHandler> changed)
    {
        bool isMeteorCurrent = changed.Behaviour.Eskill;
        changed.LoadOld();

        bool isMeteorOld = changed.Behaviour.Eskill;

        if (isMeteorCurrent && !isMeteorOld)
            changed.Behaviour.DoMeteor();
    }

    void DoMeteor()
    {
        if (!Object.HasInputAuthority)
        {
            _anim.SetBool("SkillE", true);
        }
    }

    float DegreeToRadian(float degree) { return Mathf.PI * degree / 180.0f; }

    void Fireball() // Skill Q
    {
        StartCoroutine(FireballCo());
    }

    IEnumerator FireballCo()
    {
        Qskill = true;

        QCur = QCool;
        _anim.SetBool("SkillQ", true);
        _anim.SetBool("CanWalk", false);

        Vector3 createPosition = target.transform.position - target.transform.forward * (4.5f * Mathf.Sin(DegreeToRadian(70)));
        //if(createPosition.y <= 0.5f) { createPosition.y += 2.0f; }

        RaycastHit hit;
        if (Physics.Raycast(createPosition, (target.transform.position - createPosition), out hit, 1000))
        {
            Vector3 _target = hit.point;
            Debug.Log(_target);
            Runner.Spawn(_FireBall, fireballStartPosition.transform.position, Quaternion.identity, Object.InputAuthority, (runner, spawnedFireball) =>
            {
                spawnedFireball.GetComponent<FireBallController>().Fire(_target);
                spawnedFireball.GetComponent<FireBallController>().playerId = Object.InputAuthority.PlayerId;
            });
        }

        yield return new WaitForSeconds(0.05f);
        Qskill = false;
    }

    static void OnFireball(Changed<WizardSkillHandler> changed)
    {
        bool isFireballCurrent = changed.Behaviour.Qskill;
        changed.LoadOld();

        bool isFireballOld = changed.Behaviour.Qskill;

        if (isFireballCurrent && !isFireballOld)
            changed.Behaviour.DoFireball();
    }

    void DoFireball()
    {
        if (!Object.HasInputAuthority)
        {
            _anim.SetBool("SkillQ", true);
        }
    }

    void ManaShieldExpire()
    {
        if (_manaShield == null) return;

        if (Ctrl_Remaining <= 0.0f)
        {
            _manaShield.GetComponent<ManaShieldHandler>().EndCast();
            _manaShield = null;
            _anim.SetBool("Blocking", false);
            hpHandler.isProtected = false;
        }
    }

    void ManaShield(bool keydown, bool keyup) // Defense
    {
        if (!keydown && !keyup) return;
        if (CtrlCur > 0.0f) return;

        // Start pressing the key
        if (keydown)
        {

            // Cast new shield
            if (_manaShield == null)
            {
                Debug.Log("casting shield");
                Runner.Spawn(_ManaShield, transform.position, Quaternion.identity, Object.InputAuthority, (runner, spawnedManaShield) =>
                {
                    _manaShield = spawnedManaShield;
                    _manaShield.GetComponent<ManaShieldHandler>().Cast(player);
                    _anim.SetBool("Blocking", true);
                    Ctrl_Remaining = Ctrl_MaxTime;
                    hpHandler.isProtected = true;   
                });
                return;
            }
            else // shield time expired
            {
                Debug.Log("expired");
                return;
            }
        }
        else if (keyup)
        {
            // keyup after shield expired
            if (_manaShield == null)
            {
                Debug.Log("keyup after shield expired");
                _anim.SetBool("Blocking", false);
                return;
            }
            Debug.Log("keyup    ");
            //null check for safety
            if (_manaShield != null)
                _manaShield.GetComponent<ManaShieldHandler>().EndCast();
            _manaShield = null;
            _anim.SetBool("Blocking", false);
            CtrlCur = CtrlCool;
            hpHandler.isProtected = false;
            return;
        }
    }

}
