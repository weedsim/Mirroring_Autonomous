using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public struct NetworkInputData : INetworkInput
{
    public Vector3 movementInput;
    public Vector2 cameraMove;
    public NetworkBool shiftPress;
    public NetworkBool jumpPress;
    public NetworkBool isSkillQ;
    public NetworkBool isSkillE;
    public NetworkBool isSkillR;
    public NetworkBool isVoidOrDef;
    public NetworkBool leftCtrlDown;
    public NetworkBool leftCtrlUp;
    public NetworkBool SkillEDown;
    public NetworkBool SkillEUp;
    public NetworkBool isCommonAttack;
    public NetworkBool isCommonAttackUp;
}
