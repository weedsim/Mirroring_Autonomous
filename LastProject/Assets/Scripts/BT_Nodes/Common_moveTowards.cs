using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MBT;


namespace MBTCommon
{
    [AddComponentMenu("")]
    [MBTNode("Common/Move Towards")]
    public class Common_moveTowards : Leaf
    {
        public TransformReference targetTrans;
        public TransformReference transformToMove;
        public float speed = 0.1f;
        public float minDistance = 0f;

        public override NodeResult Execute()
        {
            Debug.Log("Àç½ÇÇà");
            Vector3 target = targetTrans.Value.position;
            Transform obj = transformToMove.Value;
            // Move as long as distance is greater than min. distance
            float dist = Vector3.Distance(target, obj.position);
            if (dist > minDistance)
            {
                // Move towards target
                obj.position = Vector3.MoveTowards(
                    obj.position,
                    target,
                    (speed > dist) ? dist : speed
                );
                return NodeResult.running;
            }
            else
            {
                return NodeResult.success;
            }
        }
    }
}