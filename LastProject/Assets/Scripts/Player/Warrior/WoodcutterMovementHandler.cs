using Fusion;
using Opsive.UltimateCharacterController.Character.Abilities.Items;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WoodcutterMovementHandler : NetworkBehaviour
{
    bool _isClicked = false;
    public Animator _anim;

    WoodcutterController woodcutterController;

    private void Awake()
    {
        woodcutterController = GetComponent<WoodcutterController>();
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData networkInputData))
        {
            // Mouse Rotate
            //woodcutterController.CamRotation(networkInputData.cameraMove);

            // Move
            woodcutterController.Move(networkInputData.movementInput);

            if (networkInputData.jumpPress)
            {
                woodcutterController.Jump();
            }

            if (woodcutterController.IsGrounded)
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


}
