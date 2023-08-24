using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MBT;

namespace MBTCommon
{
    [AddComponentMenu("")]
    [MBTNode("Common/Choice Target")]
    public class Common_choiceTarget : Leaf
    {
        public LayerMask mask = 3;
        [Tooltip("Sphere radius")]
        public float range = 50;
        public TransformReference variableToSet = new TransformReference(VarRefMode.DisableConstant);
        public override NodeResult Execute()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, range, mask, QueryTriggerInteraction.Ignore);
            if (colliders.Length > 0)
            {
                variableToSet.Value = colliders[Random.Range(0,colliders.Length)].transform;
                return NodeResult.success;
            }
            else
            {
                variableToSet.Value = null;
                return NodeResult.failure;
            }
        }

    }
}