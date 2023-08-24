using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizradController : NetworkBehaviour
{
    public float _walkSpeed = 5.0f;
    Vector3 movement = Vector3.zero;

    GameObject _camera;
    GameObject _camRotateOrigin;
    GameObject _character;

    CharacterController _characterController;

    [Header("Camera")]
    // Camera 관련 함수

    [SerializeField]
    float _turnSpeedY = 1.0f;
    [SerializeField]
    float _turnSpeedX = 1.0f;

    float _rotationX = 0;
    float _threshold = 0.01f;

    float _distance;
    float _cos70;
    float _sin70;

    // Animation 관련 변수
    Animator _anim;
    bool _isClicked = false;

    // Jump 관련 변수
    public float JumpTimeout = 0.50f;
    public float FallTimeout = 0.15f;
    public bool Grounded = true;
    public float GroundedOffset = -0.15f;
    public float GroundedRadius = 0.45f;
    public float JumpHeight = 5.0f;
    public float Gravity = -6.0f;

    // 스킬 쿨타임 관련
    public float QCur;
    public float ECur;
    public float RCur;
    public float VoidCur;
    public float QCool = 5.0f;
    public float ECool = 10.0f;
    public float RCool = 15.0f;
    public float VoidCool = 30.0f;


    public GameObject _manaShield;
    GameObject obj;

    float _jumpTimeoutDelta;
    float _fallTimeoutDelta;

    float _verticalVelocity;
    float _terminalVelocity = 53.0f;

    void Start()
    {
        _camera = transform.GetChild(0).gameObject;
        _character = transform.GetChild(1).gameObject;
        _camRotateOrigin = _character.transform.GetChild(0).gameObject;
        _characterController = GetComponent<CharacterController>();
        _anim = _character.GetComponent<Animator>();

        _distance = 4.5f;
        _cos70 = Mathf.Cos(DegreeToRadian(70));
        _sin70 = Mathf.Sin(DegreeToRadian(70));

        _jumpTimeoutDelta = JumpTimeout;
        _fallTimeoutDelta = FallTimeout;

        QCur = QCool;
        ECur = ECool;
        RCur = RCool;
        VoidCur = VoidCool;
    }

    // Update is called once per frame
    void Update()
    {
        CamRotation();
        JumpAndGravity();
        CheckGround();
        CommonAttack();
        FireBall();
        ManaShield();
    }

    void FixedUpdate()
    {
        Move();
    }

    void Move() // 캐릭터 이동
    {
        float mv = Input.GetAxis("Vertical");
        float mh = Input.GetAxis("Horizontal");
        float shi = Input.GetAxis("Shift");
        float speed;

        // 시프트 누르는 상태
        if (!Mathf.Approximately(shi, 0))
        {
            if (mv > 0) { mv = _anim.GetFloat("xDir") + 2.0f * Time.deltaTime; }
            if (mv < 0) { mv = _anim.GetFloat("xDir") - 2.0f * Time.deltaTime; }
            if (mh > 0) { mh = _anim.GetFloat("yDir") + 2.0f * Time.deltaTime; }
            if (mh < 0) { mh = _anim.GetFloat("yDir") - 2.0f * Time.deltaTime; }

            if (mv > 2.0f) { mv = 2.0f; }
            if (mv < -2.0f) { mv = -2.0f; }
            if (mh > 2.0f) { mh = 2.0f; }
            if (mh < -2.0f) { mh = -2.0f; }
        }

        speed = _walkSpeed;
        Vector3 heading = mv * transform.forward + mh * transform.right;
        float moveSpeed = Mathf.Min(heading.magnitude, 1.0f) * speed;
        _characterController.Move((moveSpeed * Runner.DeltaTime * heading.normalized) + new Vector3(0, _verticalVelocity, 0) * Runner.DeltaTime);

        _anim.SetFloat("xDir", mv);
        _anim.SetFloat("yDir", mh);

    }

    void CamRotation() // 카메라 이동
    {
        Vector2 cameraMove = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        if (cameraMove.sqrMagnitude > _threshold)
        {
            //_rigidbody.constraints &= ~RigidbodyConstraints.FreezeRotationY;
            float yRotate = (cameraMove.x * _turnSpeedY) + transform.eulerAngles.y;
            _rotationX -= (cameraMove.y * _turnSpeedX);

            _rotationX = Mathf.Clamp(_rotationX, -20, 15);

            transform.eulerAngles = new Vector3(0, yRotate, 0);

            float radian = DegreeToRadian(-_rotationX);
            _camera.transform.position = _camRotateOrigin.transform.position + transform.forward * _distance * Mathf.Cos(radian) * _sin70;
            _camera.transform.position += transform.up * _distance * Mathf.Sin(radian);
            _camera.transform.position += transform.right * _distance * Mathf.Cos(radian) * _cos70;

            _camera.transform.eulerAngles = new Vector3(_rotationX, _camera.transform.eulerAngles.y, _camera.transform.eulerAngles.z);

            //_rigidbody.constraints |= RigidbodyConstraints.FreezeRotationY;
        }
    }

    void CommonAttack()
    {
        if (Input.GetMouseButton(0)) { Debug.Log("0"); }
        if(!Mathf.Approximately(Input.GetAxis("CommonAttack"), 0))
        {
            Debug.Log("1");
            _anim.SetBool("CommonAttack", true);
        }
        else
        {
            Debug.Log("2"); 
            _anim.SetBool("CommonAttack", false);
        }
    }

    void ManaShield()
    {
        if(VoidCur <= 0.0f)
        {
            VoidCur = VoidCool;
            _anim.SetBool("Blocking", false);
        }
        else if(VoidCur > 0.0f && VoidCur < VoidCool)
        {
            VoidCur -= Time.deltaTime;
            _anim.SetBool("Blocking", false);
        }




        if(!Mathf.Approximately(Input.GetAxis("VoidOrDef"), 0))
        {
            if (VoidCur <= 0.0f)
            {
                VoidCur = VoidCool;
                _anim.SetBool("Blocking", false);
            }
            else if (VoidCur > 0.0f && VoidCur < VoidCool)
            {
                VoidCur -= Time.deltaTime;
                _anim.SetBool("Blocking", false);
            }
            else
            {
                Debug.Log("0");
                obj = Instantiate(_manaShield, transform);
                _anim.SetBool("Blocking", true);
            }
        }
        else
        {
            Destroy(obj);
            _anim.SetBool("Blocking", false);
        }
    }

    void FireBall()
    {
        // 쿨타임 다 돌면 쿨타임 초기화
        if (QCur <= 0.0f)
        {
            QCur = QCool;
            _anim.SetBool("SkillQ", false);
        }
        // 쿨타임 도는 중
        else if (QCur > 0.0f && QCur < QCool)
        {
            QCur -= Time.deltaTime;
            _anim.SetBool("SkillQ", false);
        }

        if (!Mathf.Approximately(Input.GetAxis("SkillQ"), 0))
        {
            // 쿨타임 다 돌면 쿨타임 초기화
            if (QCur <= 0.0f)
            {
                QCur = QCool;
                _anim.SetBool("SkillQ", false);
            }
            // 쿨타임 도는 중
            else if (QCur > 0.0f && QCur < QCool)
            {
                QCur -= Time.deltaTime;
                _anim.SetBool("SkillQ", false);
            }
            // 스킬 쓸 수 있는 상태
            else
            {
                _anim.SetBool("SkillQ", true);
                QCur -= Time.deltaTime;
            }
        }
        Debug.Log(QCur);
    }


    void JumpAndGravity()
    {
        if (Grounded)
        {
            _fallTimeoutDelta = FallTimeout;

            _anim.SetBool("Jump", false);
            _anim.SetBool("Fall", false);

            if (_verticalVelocity < 0.0f)
            {
                _verticalVelocity = -2.0f;
            }

            if (Input.GetKeyDown(KeyCode.Space) && _jumpTimeoutDelta <= 0.0f)
            {
                // the square root of H * -2 * G = how much velocity needed to reach desired height
                _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
                _anim.SetBool("Jump", true);
            }

            if (_jumpTimeoutDelta >= 0.0f)
            {
                _jumpTimeoutDelta -= Time.deltaTime;
            }
        }

        else
        {
            _jumpTimeoutDelta = JumpTimeout;

            // fall timeout
            if (_fallTimeoutDelta >= 0.0f)
            {
                _fallTimeoutDelta -= Time.deltaTime;
            }

            else
                _anim.SetBool("Fall", true);
        }

        // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
        if (_verticalVelocity < _terminalVelocity)
        {
            _verticalVelocity += Gravity * Time.deltaTime;
        }
    }

    void CheckGround()
    {
        Grounded = Physics.CheckSphere(
            new Vector3(_character.transform.position.x, _character.transform.position.y - GroundedOffset, _character.transform.position.z)
            , GroundedRadius, ~(1 << 3), QueryTriggerInteraction.Ignore);

        _anim.SetBool("Ground", Grounded);
    }

    private float DegreeToRadian(float degree) { return Mathf.PI * degree / 180.0f; }
}
