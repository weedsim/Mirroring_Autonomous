using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MBT
{
    [AddComponentMenu("")]
    [MBTNode("Common/Boss Invoke")]
    public class Common_bossInvoke : Leaf
    {
        public BossEvent evnetName = new BossEvent();
        public BoolReference endTask = new BoolReference(VarRefMode.DisableConstant);
        public override NodeResult Execute()
        {
            endTask.Value = false;
            evnetName.Invoke();
            return NodeResult.success;
        }
    }
    [System.Serializable]
    public class BossEvent : UnityEvent{ };
}