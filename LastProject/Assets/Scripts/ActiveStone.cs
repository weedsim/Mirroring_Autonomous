using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveStone : MonoBehaviour
{

    public Material newMaterial;
    float timer;

    // Start is called before the first frame update
    void Start()
    {
        timer = 0;

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            Invoke("ChangeColor", 10f);
        }
    }

     void ChangeColor() 
    {
        Renderer renderer = GetComponent<Renderer>();
        renderer.material = newMaterial;
    }
}
