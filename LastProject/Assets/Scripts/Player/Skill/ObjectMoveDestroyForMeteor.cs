using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMoveDestroyForMeteor : MonoBehaviour
{
    public GameObject m_gameObjectMain;
    GameObject m_makedObject;
    public float maxLength;
    public bool isDestroy;
    public float ObjectDestroyTime;
    public float TailDestroyTime;
    public float HitObjectDestroyTime;
    public float maxTime = 1;
    public float MoveSpeed = 30;
    public bool isCheckHitTag;
    public string mtag;

    float time;
    bool ishit;
    float m_scalefactor;

    private void Start()
    {
        m_scalefactor = VariousEffectsScene.m_gaph_scenesizefactor;//transform.parent.localScale.x;
        time = Time.time;
    }

    void LateUpdate()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * MoveSpeed * m_scalefactor);
        if (!ishit)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, maxLength))
                HitObj(hit);
        }
    }

    void HitObj(RaycastHit hit)
    {
        if (isCheckHitTag)
            if (hit.collider.CompareTag(mtag))
                return;
        ishit = true;
    }
}
