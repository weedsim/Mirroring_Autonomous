using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phase2To3 : MonoBehaviour
{
    public Action BreakWall;
    public Action DestroyWall;
    public void ReceiveSignal1()
    {
        BreakWall.Invoke();
    }

    public void ReceiveSignal2()
    {
        Destroy(transform.GetChild(0).gameObject);
        Destroy(transform.GetChild(1).gameObject);
        Destroy(transform.GetChild(2).gameObject);
        Destroy(transform.GetChild(3).gameObject);
    }
}
