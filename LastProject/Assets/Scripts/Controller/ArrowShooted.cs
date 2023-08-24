using MBT;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowShooted : MonoBehaviour
{
    Rigidbody _rb;
    public static bool _hasInput;

    [Header("Arrow Damage")]
    public int ArrowDamage = 100;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        CheckHit(_hasInput);
        Move();
    }

    void Move()
    {
        _rb.MovePosition(transform.position + (50.0f * Time.deltaTime * transform.forward));
    }
    public void CheckHit(bool hasInput)
    {
        RaycastHit hit;
        if(Physics.SphereCast(transform.position, 0.04f, transform.forward, out hit, 50.0f * Time.deltaTime, ~(1 << 3)))
        {
            Debug.Log(hit.transform.gameObject.name);
            if (hit.transform.gameObject.GetComponent<HPHandler>() != null && hasInput) {
                hit.transform.gameObject.GetComponent<HPHandler>().OnTakeDamage(ArrowDamage);
            }
            if (hit.transform.gameObject.CompareTag("Stone"))
            {
                Debug.Log("Bowman Stone Hit!!");
                hit.transform.gameObject.GetComponent<StoneManager>().OnTakeDamage();
            }
            Destroy(gameObject);
        }
    }
}
