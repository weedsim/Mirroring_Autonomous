using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterInputHandler : MonoBehaviour
{
    private NetworkObject networkObject;
    private Vector3 targetPos;
    private Vector3 movementInput;
    private Vector2 cameraMove;
    private float shiftPress;
    private bool shift= false;
    private bool jumpPress = false;
    private bool isSkillQ = false;
    private bool isSkillE = false;
    private bool isSkillR = false;
    private bool leftCtrlDown = false;
    private bool leftCtrlUp = false;
    private bool SkillEDown = false;
    private bool SkillEUp = false;
    private bool isVoidOrDef = false;
    private bool isCommonAttack = false;
    private bool isCommonAttackUp = false;

    LocalCameraHandler localCameraHandler;
    BasicMovementHandler basicMovementHandler;

    private void Awake()
    {
        localCameraHandler = GetComponent<LocalCameraHandler>();
        basicMovementHandler = GetComponent<BasicMovementHandler>();
    }

    // Start is called before the first frame update
    void Start()
    {
        networkObject = GetComponent<NetworkObject>();
        Debug.Log("inputhandler start");
    }

    private void FixedUpdate()
    {
        /*float mv = Input.GetAxis("Vertical");
        float mh = Input.GetAxis("Horizontal");

        movementInput = mv * Vector3.forward + mh * Vector3.right;*/
    }

    // Update is called once per frame
    void Update()
    {
        if (!basicMovementHandler.Object.HasInputAuthority)
            return;

        float mv = Input.GetAxis("Vertical");
        float mh = Input.GetAxis("Horizontal");
        movementInput = mv * transform.forward + mh * transform.right; // character move

        cameraMove = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")); // view move

        if (Input.GetButtonDown("Jump")) // jump
            jumpPress = true;
        
        if (Input.GetButtonDown("Fire1"))
        {
            isCommonAttack = true;
            isCommonAttackUp = true;
        }
        if (Input.GetButtonUp("Fire1"))
        {
            isCommonAttackUp = false;
        }
        if (Input.GetButtonDown("SkillQ"))
            isSkillQ = true;
        if (Input.GetButtonDown("SkillE"))
        {
            isSkillE = true;
            SkillEDown = true;
        }
        if (Input.GetButtonDown("SkillR"))
            isSkillR = true;

        //if(Input.GetKey(KeyCode.LeftControl))
        //    leftCtrlDown = true;   

        if (Input.GetButtonUp("SkillE"))
            SkillEUp = true;

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            leftCtrlDown = true;
            isVoidOrDef = true;
        }

        if (Input.GetKeyUp(KeyCode.LeftControl))
            leftCtrlUp = true;


    }

    public NetworkInputData GetNetworkInput()
    {
        NetworkInputData networkInputData = new NetworkInputData();


        ////Aim data
        networkInputData.cameraMove = cameraMove;

        // Move data
        networkInputData.movementInput = movementInput;

        // jump data
        networkInputData.jumpPress = jumpPress;

        // skill q, e, r
        networkInputData.isSkillQ = isSkillQ;
        networkInputData.isSkillE = isSkillE;
        networkInputData.isSkillR = isSkillR;

        // shift data
        networkInputData.shiftPress = shift;

        // voidOrDef data
        networkInputData.isVoidOrDef = isVoidOrDef;

        // commonAttack data
        networkInputData.isCommonAttack = isCommonAttack;
        networkInputData.isCommonAttackUp = isCommonAttackUp;

        networkInputData.leftCtrlDown = leftCtrlDown;
        networkInputData.leftCtrlUp = leftCtrlUp;

        // for wizard Skill e
        networkInputData.SkillEDown = SkillEDown;
        networkInputData.SkillEUp = SkillEUp;

        shift = false;
        jumpPress = false;
        isSkillQ = false;
        isSkillE = false;
        isSkillR = false;
        isVoidOrDef = false;
        isCommonAttack = false;
        leftCtrlDown = false;
        leftCtrlUp = false;
        SkillEDown = false;
        SkillEUp = false;

        return networkInputData;
    }

}
