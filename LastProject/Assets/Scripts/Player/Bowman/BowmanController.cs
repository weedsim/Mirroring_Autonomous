using System;
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
public class BowmanController : NetworkTransform {
  [Header("Character Controller Settings")]
   float gravity       = -15.0f;
   float jumpImpulse   = 8.0f;
   float acceleration  = 50.0f;
   float braking       = 10.0f;
   float maxSpeed      = 7.0f; 
   float rotationSpeed = 15.0f;
   float viewUpDownRotationSpeed = 50.0f;


  [Header("Camera Controller Settings")]
  public GameObject _camera;
    public GameObject _camRotateOrigin;
    public float _turnSpeedY = 1.0f;
    public float _turnSpeedX = 1.0f;
    public float _rotationX = 0;
    public float _threshold = 0.01f;

    protected float _distance;
    protected float _cos70;
    protected float _sin70;


    public Animator _anim;

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

  protected override void Awake() {
    base.Awake();
    CacheController();

        _distance = 4.5f;
        _cos70 = Mathf.Cos(DegreeToRadian(70));
        _sin70 = Mathf.Sin(DegreeToRadian(70));
    }

  public override void Spawned() {
    base.Spawned();
    CacheController();

    // Caveat: this is needed to initialize the Controller's state and avoid unwanted spikes in its perceived velocity
    Controller.Move(transform.position);
  }

  private void CacheController() {
    if (Controller == null) {
      Controller = GetComponent<CharacterController>();

      Assert.Check(Controller != null, $"An object with {nameof(NetworkCharacterControllerPrototype)} must also have a {nameof(CharacterController)} component.");
    }
  }

  protected override void CopyFromBufferToEngine() {
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
  public virtual void Jump(bool ignoreGrounded = false, float? overrideImpulse = null) {
    if (IsGrounded || ignoreGrounded) {
      _anim.SetTrigger("Jump");

      var newVel = Velocity;
      newVel.y += overrideImpulse ?? jumpImpulse;
      Velocity =  newVel;
    }
   }

  /// <summary>
  /// Basic implementation of a character controller's movement function based on an intended direction.
  /// <param name="direction">Intended movement direction, subject to movement query, acceleration and max speed values.</param>
  /// </summary>
  public virtual void Move(Vector3 direction) {
    var deltaTime    = Runner.DeltaTime;
    var previousPos  = transform.position;
    var moveVelocity = Velocity;

    direction = direction.normalized;

    if (IsGrounded && moveVelocity.y < 0) {
      moveVelocity.y = 0f;
    }

    moveVelocity.y += gravity * Runner.DeltaTime;

    var horizontalVel = default(Vector3);
    horizontalVel.x = moveVelocity.x;
    horizontalVel.z = moveVelocity.z;

    if (direction == default) {
      horizontalVel = Vector3.Lerp(horizontalVel, default, braking * deltaTime);
    } else {
      horizontalVel      = Vector3.ClampMagnitude(horizontalVel + direction * acceleration * deltaTime, maxSpeed);
    }

    moveVelocity.x = horizontalVel.x;
    moveVelocity.z = horizontalVel.z;

    Controller.Move(moveVelocity * deltaTime);

    Velocity   = (transform.position - previousPos) * Runner.Simulation.Config.TickRate;
    IsGrounded = Controller.isGrounded;


    _anim.SetFloat("xDir", direction.x);
    _anim.SetFloat("yDir", direction.z);

    }

    public void Rotate(float rotationY)
    {
        transform.Rotate(0, rotationY * Runner.DeltaTime * rotationSpeed, 0);
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

    protected float DegreeToRadian(float degree) { return Mathf.PI * degree / 180.0f; }
}