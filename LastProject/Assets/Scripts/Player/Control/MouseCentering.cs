using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseCentering : MonoBehaviour
{
    // Start is called before the first frame update
    void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false; // 마우스 커서가 안 보이게 처리
    }
}