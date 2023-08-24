using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Indicator : MonoBehaviour
{
    public float duration = 1f;
    private void OnEnable()
    {
        StartCoroutine(off());
    }
    IEnumerator off()
    {
        yield return new WaitForSeconds(duration);
        gameObject.SetActive(false);
    }

}
