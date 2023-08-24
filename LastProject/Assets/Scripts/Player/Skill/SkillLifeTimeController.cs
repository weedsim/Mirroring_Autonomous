using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillLifeTimeController : MonoBehaviour
{
    public float lifetime = 3.0f;

    private float m_CurrentTime = 0.0f;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        m_CurrentTime += Time.deltaTime;

        if (m_CurrentTime >= lifetime)
        {
            Destroy(gameObject);
        }
    }
}
