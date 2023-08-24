using Fusion;
using Opsive.UltimateCharacterController.Character.Abilities.Items;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BowmanMovementHandler : NetworkBehaviour
{
    bool _isClicked = false;
    public Animator _anim;

    BowmanController bowmanContoller;

    void Awake()
    {

        bowmanContoller = GetComponent<BowmanController>();
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData networkInputData))
        {
            Aim();

            // Mouse Rotate
            //bowmanContoller.CamRotation(networkInputData.cameraMove);

            // Move
            bowmanContoller.Move(networkInputData.movementInput);

            if (networkInputData.jumpPress)
                bowmanContoller.Jump();

            if (bowmanContoller.IsGrounded)
            {
                _anim.SetBool("Ground", true);
            }

            // fall ground check
            fallGround();
        }
    }

    void fallGround()
    {
        if (transform.position.y < -12)
            transform.position = Utils.GetRandomSpawnPoint();
    }

    void Aim() // 조준 애니메이션 관련
    {
        if (Input.GetMouseButtonDown(0))
        {
            _isClicked = true;
            _anim.SetBool("IsClicked", _isClicked);
        }

        if (Input.GetMouseButtonUp(0))
        {
            _isClicked = false;
            _anim.SetBool("IsClicked", _isClicked);
        }
    }

}
