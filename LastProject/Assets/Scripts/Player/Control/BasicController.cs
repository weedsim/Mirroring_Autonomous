using System;
using System.Collections;
using Fusion;
using Opsive.Shared.Camera;
using Opsive.UltimateCharacterController.Character.Abilities.Items;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CharacterController))]
[OrderBefore(typeof(NetworkTransform))]
[DisallowMultipleComponent]
// ReSharper disable once CheckNamespace
public class BasicController : NetworkTransform
{
    [Networked(OnChanged = nameof(OnJumped))]
    public bool IsJumped { get; set; }

    [Header("Character Move Settings")]
    public float gravity = -15.0f;
    public float acceleration = 50.0f;
    public float braking = 10.0f;
    public float maxSpeed = 7.0f;

    [Header("Character Jump Setting")]
    public float GroundedOffset = -0.14f;
    public float GroundedRadius = 0.28f;
    public float JumpTimeout = 0.50f;
    public float JumpHeight = 10.0f;

    public Animator _anim;

    [Header("Camera Controller Settings")]
    public GameObject _camera;
    public GameObject _camRotateOrigin;
    public float _rotationX = 0;
    float _threshold = 0.01f;

    public float rotationSpeed = 15.0f;
    public float viewUpDownRotationSpeed = 50.0f;

    [Networked]
    [HideInInspector]
    public bool IsGrounded { get; set; }

    [Networked]
    [HideInInspector]
    public Vector3 Velocity { get; set; }

    /// <summary>
    /// Sets the default teleport interpolation velocity to be the CC's current velocity.
    /// For more details on how this field is used, see <see cref="NetworkTransform.TeleportToPosition"/>.
    /// </summary>
    protected override Vector3 DefaultTeleportInterpolationVelocity => Velocity;

    /// <summary>
    /// Sets the default teleport interpolation angular velocity to be the CC's rotation speed on the Z axis.
    /// For more details on how this field is used, see <see cref="NetworkTransform.TeleportToRotation"/>.
    /// </summary>
    protected override Vector3 DefaultTeleportInterpolationAngularVelocity => new Vector3(0f, 0f, rotationSpeed);

    public CharacterController Controller { get; private set; }

    public Utils.PlayerClass _playerClass;

    protected override void Awake()
    {
        base.Awake();
        CacheController();
    }

    public override void Spawned()
    {
        base.Spawned();
        CacheController();

        // Caveat: this is needed to initialize the Controller's state and avoid unwanted spikes in its perceived velocity
        Controller.Move(transform.position);
    }

    private void CacheController()
    {
        if (Controller == null)
        {
            Controller = GetComponent<CharacterController>();

            Assert.Check(Controller != null, $"An object with {nameof(NetworkCharacterControllerPrototype)} must also have a {nameof(CharacterController)} component.");
        }
    }

    protected override void CopyFromBufferToEngine()
    {
        // Trick: CC must be disabled before resetting the transform state
        Controller.enabled = false;

        // Pull base (NetworkTransform) state from networked data buffer
        base.CopyFromBufferToEngine();

        // Re-enable CC
        Controller.enabled = true;
    }

    /// <summary>
    /// Basic implementation of a jump impulse (immediately integrates a vertical component to Velocity).
    /// <param name="ignoreGrounded">Jump even if not in a grounded state.</param>
    /// <param name="overrideImpulse">Optional field to override the jump impulse. If null, <see cref="jumpImpulse"/> is used.</param>
    /// </summary>
    public virtual void Jump(bool ignoreGrounded = false, float? overrideImpulse = null)
    {
        if (IsGrounded || ignoreGrounded)
        {
            StartCoroutine(JumpCo());
        }
    }
    IEnumerator JumpCo()
    {
        IsJumped = true;
        var newVel = Velocity;
        newVel.y += Mathf.Sqrt(JumpHeight * -2f * gravity);
        Velocity = newVel;
        _anim.SetTrigger("Jump");
        yield return new WaitForSeconds(0.05f);
        IsJumped = false;
    }

    static void OnJumped(Changed<BasicController> changed)
    {
        bool isJumpCurrent = changed.Behaviour.IsJumped;
        changed.LoadOld();
        bool isJumpOld = changed.Behaviour.IsJumped;

        if (isJumpCurrent && !isJumpOld)
            changed.Behaviour.DoJump();
    }

    void DoJump()
    {
        if (!Object.HasInputAuthority)
        {
            _anim.SetTrigger("Jump");
        }
    }

    /// <summary>
    /// Basic implementation of a character controller's movement function based on an intended direction.
    /// <param name="direction">Intended movement direction, subject to movement query, acceleration and max speed values.</param>
    /// </summary>
    public virtual void Move(Vector3 direction)
    {
        var deltaTime = Runner.DeltaTime;
        var previousPos = transform.position;
        var moveVelocity = Velocity;

        direction = direction.normalized;

        if (IsGrounded && moveVelocity.y < 0)
        {
            moveVelocity.y = -2.0f;
        }

        moveVelocity.y += gravity * deltaTime;

        var horizontalVel = default(Vector3);
        horizontalVel.x = moveVelocity.x;
        horizontalVel.z = moveVelocity.z;

        if (direction == default)
        {
            horizontalVel = Vector3.Lerp(horizontalVel, default, braking * deltaTime);
        }

        else
        {
            horizontalVel = Vector3.ClampMagnitude(horizontalVel + direction * acceleration * deltaTime, maxSpeed);
        }

        moveVelocity.x = horizontalVel.x;
        moveVelocity.z = horizontalVel.z;

        _anim.SetFloat("xDir", direction.x);
        _anim.SetFloat("yDir", direction.z);

        Controller.Move(moveVelocity * deltaTime);

        Velocity = (transform.position - previousPos) * Runner.Simulation.Config.TickRate;
        IsGrounded = CheckGround();
    }

    public void CamRotation(Vector2 cameraMove)
    {
        if (cameraMove.sqrMagnitude > _threshold)
        {
            transform.Rotate(0, cameraMove.x * rotationSpeed * Runner.DeltaTime, 0);

            _rotationX -= (cameraMove.y * viewUpDownRotationSpeed * Runner.DeltaTime);
            _rotationX = Mathf.Clamp(_rotationX, -20, 15);

            float radian = DegreeToRadian(-_rotationX);
            _camera.transform.position = _camRotateOrigin.transform.position + transform.forward * 4.5f * Mathf.Cos(radian) * Mathf.Sin(DegreeToRadian(70));
            _camera.transform.position += transform.up * 4.5f * Mathf.Sin(radian);
            _camera.transform.position += transform.right * 4.5f * Mathf.Cos(radian) * Mathf.Cos(DegreeToRadian(70));

            _camera.transform.localRotation = Quaternion.Euler(_rotationX, 0, 0);
        }
    }

    bool CheckGround()
    {
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
        return Physics.CheckSphere(spherePosition, GroundedRadius, ~(1 << 3), QueryTriggerInteraction.Ignore);
    }

    protected float DegreeToRadian(float degree) { return Mathf.PI * degree / 180.0f; }
}