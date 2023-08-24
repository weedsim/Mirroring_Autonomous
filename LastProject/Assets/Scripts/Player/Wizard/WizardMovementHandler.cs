using Fusion;
using Opsive.UltimateCharacterController.Character.Abilities.Items;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WizardMovementHandler : NetworkBehaviour
{
    bool _isClicked = false;
    public Animator _anim;

    WizardController wizardController;

    void Awake()
    {

        wizardController = GetComponent<WizardController>();
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData networkInputData))
        {
            // Mouse Rotate
            //wizardController.CamRotation(networkInputData.cameraMove);

            // Move
            wizardController.Move(networkInputData.movementInput);

            if (networkInputData.jumpPress)
                wizardController.Jump();

            if (wizardController.IsGrounded)
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
