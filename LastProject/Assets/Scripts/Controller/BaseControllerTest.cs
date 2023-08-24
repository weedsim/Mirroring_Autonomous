using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(CharacterController))]
[OrderBefore(typeof(NetworkTransform))]
[DisallowMultipleComponent]
public class BaseControllerTest : NetworkBehaviour
{
    public float _walkSpeed = 10.0f;
    Vector3 movement = Vector3.zero;
    
    protected GameObject _camera;
    protected GameObject _camRotateOrigin;
    protected GameObject _character;
    
    protected CharacterController _characterController;

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
    public float Gravity = -30.0f;

    protected float _jumpTimeoutDelta;
    protected float _fallTimeoutDelta;

    float _verticalVelocity;
    float _terminalVelocity = 53.0f;

    private void Awake()
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
    }

    public virtual void Move(Vector3 moveDirection) // 캐릭터 이동
    {
        float speed = _walkSpeed;
        float mv = moveDirection.x;
        float mh = moveDirection.z;

        _anim.SetFloat("xDir", mv);
        _anim.SetFloat("yDir", mh);

        float moveSpeed = Mathf.Min(moveDirection.magnitude, 1.0f) * speed;

        _characterController.Move((moveSpeed * Time.deltaTime * moveDirection.normalized) + new Vector3(0, _verticalVelocity, 0) * Time.deltaTime);
    }

    public virtual void CamRotation(Vector2 cameraMove) // 카메라 이동
    {
        if (cameraMove.sqrMagnitude > _threshold)
        {
            float yRotate = (cameraMove.x * _turnSpeedY) + transform.eulerAngles.y;
            _rotationX -= (cameraMove.y * _turnSpeedX);

            _rotationX = Mathf.Clamp(_rotationX, -20, 15);

            transform.eulerAngles = new Vector3(0, yRotate, 0);

            float radian = DegreeToRadian(-_rotationX);
            _camera.transform.position = _camRotateOrigin.transform.position + transform.forward * _distance * Mathf.Cos(radian) * _sin70;
            _camera.transform.position += transform.up * _distance * Mathf.Sin(radian);
            _camera.transform.position += transform.right * _distance * Mathf.Cos(radian) * _cos70;
                
            _camera.transform.eulerAngles = new Vector3(_rotationX, _camera.transform.eulerAngles.y, _camera.transform.eulerAngles.z);
        }
    }

    public virtual void JumpAndGravity(bool jumpPressed)
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

            if (jumpPressed && _jumpTimeoutDelta <= 0.0f)
            {
                // the square root of H * -2 * G = how much velocity needed to reach desired height
                _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
                _anim.SetBool("Jump", true);
            }

            if (_jumpTimeoutDelta >= 0.0f)
            {
                _jumpTimeoutDelta -= Runner.DeltaTime;
            }
        }

        else
        {
            _jumpTimeoutDelta = JumpTimeout;

            // fall timeout
            if (_fallTimeoutDelta >= 0.0f)
            {
                _fallTimeoutDelta -= Runner.DeltaTime;
            }

            else
                _anim.SetBool("Fall", true);
        }

        // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
        if (_verticalVelocity < _terminalVelocity)
        {
            _verticalVelocity += Gravity * Runner.DeltaTime;
        }
    }

    public virtual void CheckGround()
    {
        Grounded = Physics.CheckSphere(
            new Vector3(_character.transform.position.x, _character.transform.position.y - GroundedOffset, _character.transform.position.z)
            , GroundedRadius, ~(1 << 3), QueryTriggerInteraction.Ignore);

        _anim.SetBool("Ground", Grounded);
    }

    public virtual void fallGround()
    {
        if (transform.position.y < -12)
            transform.position = Utils.GetRandomSpawnPoint();
    }

    protected float DegreeToRadian(float degree) { return Mathf.PI * degree / 180.0f; }
}
