using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MBT
{
    [AddComponentMenu("")]
    [MBTNode("Common/Wait For End Task")]
    public class Common_wailForEndTask : Leaf
    {
        public BoolReference endTask = new BoolReference(VarRefMode.DisableConstant);
        public override NodeResult Execute()
        {
            if (endTask.Value)
            {
                return NodeResult.success;
            }
            return NodeResult.running;
        }
    }

}