using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseController : MonoBehaviour
{
    public float _walkSpeed = 5.0f;
    Vector3 movement = Vector3.zero;
    
    protected GameObject _camera;
    protected GameObject _camRotateOrigin;
    protected GameObject _character;
    
    protected Rigidbody _rigidbody;

    [Header("Camera")]
    // Camera 관련 함수

    [SerializeField]
    float _turnSpeedY = 1.0f;
    [SerializeField]
    float _turnSpeedX = 1.0f;

    float _rotationX = 0;
    float _threshold = 0.01f;

    protected float _distance;
    protected float _cos70;
    protected float _sin70;

    // Animation 관련 변수
    protected Animator _anim;

    // Jump 관련 변수
    public float JumpTimeout = 0.50f;
    public float FallTimeout = 0.50f;
    public bool Grounded = true;
    public float GroundedOffset = -0.15f;
    public float GroundedRadius = 0.8f;
    public float JumpHeight = 8.0f;
    public float Gravity = -6.0f;

    protected float _jumpTimeoutDelta;
    protected float _fallTimeoutDelta;

    float _verticalVelocity;
    float _terminalVelocity = 53.0f;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _camera = transform.GetChild(0).gameObject;
        _camRotateOrigin = transform.GetChild(1).GetChild(0).gameObject;

    }

    void Update()
    {
        CamRotation();
    }

    void FixedUpdate()
    {
        Move();
    }

    protected void Move() // 캐릭터 이동
    {
        float mv = Input.GetAxis("Vertical");
        float mh = Input.GetAxis("Horizontal");

        /*_anim.SetFloat("xDir", mv);
        _anim.SetFloat("yDir", mh);*/

        Vector3 heading = mv * transform.forward + mh * transform.right;

        float speed = _walkSpeed;

        float moveSpeed = Mathf.Min(heading.magnitude, 1.0f) * speed;

        _rigidbody.MovePosition(transform.position + (moveSpeed * Time.deltaTime * heading.normalized) + new Vector3(0, _verticalVelocity, 0) * Time.deltaTime);
    }

    protected void CamRotation() // 카메라 이동
    {
        Vector2 cameraMove = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        if (cameraMove.sqrMagnitude > _threshold)
        {
            _rigidbody.constraints &= ~RigidbodyConstraints.FreezeRotationY;
            float yRotate = (cameraMove.x * _turnSpeedY) + transform.eulerAngles.y;
            _rotationX -= (cameraMove.y * _turnSpeedX);

            _rotationX = Mathf.Clamp(_rotationX, -20, 15);

            transform.eulerAngles = new Vector3(0, yRotate, 0);

            float radian = DegreeToRadian(-_rotationX);
            _camera.transform.position = _camRotateOrigin.transform.position + transform.forward * _distance * Mathf.Cos(radian) * _sin70;
            _camera.transform.position += transform.up * _distance * Mathf.Sin(radian);
            _camera.transform.position += transform.right * _distance * Mathf.Cos(radian) * _cos70;
                
            _camera.transform.eulerAngles = new Vector3(_rotationX, _camera.transform.eulerAngles.y, _camera.transform.eulerAngles.z);

            _rigidbody.constraints |= RigidbodyConstraints.FreezeRotationY;
        }
    }

    protected void JumpAndGravity()
    {
        if (Grounded)
        {
            _fallTimeoutDelta = FallTimeout;
            
            _anim.SetBool("Jump", false);
            _anim.SetBool("Fall", false);

            if (_verticalVelocity < 0.0f)
            {
                _verticalVelocity = 0f;
            }

            if (Input.GetKeyDown(KeyCode.Space) && _jumpTimeoutDelta <= 0.0f)
            {
                // the square root of H * -2 * G = how much velocity needed to reach desired height
                _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * (Gravity - 9.8f));
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
            _verticalVelocity += (Gravity - 9.8f) * Time.deltaTime;
        }
    }

    protected void CheckGround()
    {
        Grounded = Physics.CheckSphere(
            new Vector3(_character.transform.position.x, _character.transform.position.y - GroundedOffset, _character.transform.position.z)
            , GroundedRadius, ~(1 << 3), QueryTriggerInteraction.Ignore);

        _anim.SetBool("Ground", Grounded);
    }

    protected float DegreeToRadian(float degree) { return Mathf.PI * degree / 180.0f; }
}
