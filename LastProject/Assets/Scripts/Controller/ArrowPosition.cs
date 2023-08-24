using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowPosition : MonoBehaviour
{
    [SerializeField]
    GameObject RightHand;
    [SerializeField]
    GameObject Bow;

    Vector3 _dir;

    void Update()
    {
        _dir = Bow.transform.position - RightHand.transform.position;
        transform.rotation = Quaternion.LookRotation(_dir);
        transform.position = RightHand.transform.position;
    }
}
