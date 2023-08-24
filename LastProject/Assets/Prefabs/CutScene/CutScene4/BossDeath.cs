using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDeath : MonoBehaviour
{
    public Action _Launch;
    public void Launch()
    {
        _Launch.Invoke();
    }
}
