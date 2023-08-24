using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StonesManager : MonoBehaviour
{
    int count;
    bool flag = false;

    void Start()
    {
        count = transform.childCount;
    }

    void Update()
    {
        if (!flag && count <= 0)
        {
            Debug.Log("±â¹Í ÆÄÈÑ ¿Ï·á");
            GameObject.FindGameObjectWithTag("Enemy").GetComponent<HPHandler>().isProtected = false;
            flag = true;
        }    
    }

    public void SetCount()
    {
        count--;
    }
}
