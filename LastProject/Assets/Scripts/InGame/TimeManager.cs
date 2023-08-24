using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [SerializeField]
    public float time;

    private void Start()
    {
        time = 0.0f;
    }
    private void Update()
    {
        time += Time.deltaTime;
    }
}
