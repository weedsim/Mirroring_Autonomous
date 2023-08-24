using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamController : MonoBehaviour
{
    public CinemachineVirtualCamera[] cam;

    void Update()
    {
        if (cam[0].LookAt != null)
        {
            Vector3 end = cam[0].LookAt.position - cam[0].LookAt.forward * (4.5f * Mathf.Sin(DegreeToRadian(70)));

            RaycastHit hit;
            if (Physics.Raycast(cam[0].transform.position, end - cam[0].transform.position, out hit, 7.5f))
            {
                if (hit.collider.gameObject != cam[0].LookAt.gameObject)
                {
                    cam[1].Priority = 15;
                }
            }

            else
                cam[1].Priority = 9;
        }
    }

    float DegreeToRadian(float degree) { return Mathf.PI * degree / 180.0f; }
}
