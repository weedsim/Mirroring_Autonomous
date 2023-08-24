using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMovementHandler : NetworkBehaviour
{
    public Animator _anim;
    BasicController basicController;
    public float jumpCool = 0.8f;
    private bool canJump = true;
    void Awake()
    {
        basicController = GetComponent<BasicController>();
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData networkInputData))
        {
            basicController.Move(networkInputData.movementInput); // Move

            basicController.CamRotation(networkInputData.cameraMove);

            if (canJump && networkInputData.jumpPress)
            {
                canJump = false;
                StartCoroutine(jumpCRT());
                basicController.Jump();
            }
            if (basicController.IsGrounded)
            {
                _anim.SetBool("Ground", true);
            }

            // fall ground check
            //fallGround();
        }
    }
    IEnumerator jumpCRT()
    {
        yield return new WaitForSeconds(jumpCool);
        canJump = true;
    }

    void fallGround()
    {
        if (transform.position.y < -12 || transform.position.y > 500)
            transform.position = Utils.GetRandomSpawnPoint();
    }

    
}
