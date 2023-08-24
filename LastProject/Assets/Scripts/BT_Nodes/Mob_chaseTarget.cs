using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MBT
{
    [AddComponentMenu("")]
    [MBTNode("Mob/Chase Target")]
    public class Mob_chaseTarget : Leaf
    {
        public GameObjectReference mObj = new GameObjectReference(VarRefMode.DisableConstant);
        public BoolReference endTask = new BoolReference(VarRefMode.DisableConstant);

        public override NodeResult Execute()
        {
            endTask.Value = false;
            mObj.Value.GetComponent<MobManager>().RPC_choiceAndChaseTarget();
            return NodeResult.success;
        }
    }
}