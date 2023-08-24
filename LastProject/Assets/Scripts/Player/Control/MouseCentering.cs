using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseCentering : MonoBehaviour
{
    // Start is called before the first frame update
    void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false; // ���콺 Ŀ���� �� ���̰� ó��
    }
}