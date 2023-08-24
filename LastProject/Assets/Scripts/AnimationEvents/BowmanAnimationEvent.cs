using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowmanAnimationEvent : MonoBehaviour
{
    [SerializeField]
    GameObject _arrow;

    [SerializeField]
    GameObject _target;

    Animator _animator;

    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    void DrawArrow()
    {
        _arrow.SetActive(true);
    }

    void CancelAttack()
    {
        _arrow.SetActive(false);
    }

    void ShootArrow()
    {
        GameObject shootArrow = Resources.Load<GameObject>("Prefabs/arrowToShoot");
        shootArrow.name = "ArrowShooted";
        shootArrow.transform.position = _arrow.transform.GetChild(0).position;
        
        Vector3 start = _target.transform.position - _target.transform.forward * (4.5f * Mathf.Sin(DegreeToRadian(70)));

        RaycastHit hit;
        if(Physics.Raycast(start, (_target.transform.position - start), out hit, 1000))
        {
            shootArrow.transform.rotation = Quaternion.LookRotation(hit.point - shootArrow.transform.position);
        }

        _arrow.SetActive(false);
        Instantiate(shootArrow);
    }

    float DegreeToRadian(float degree) { return Mathf.PI * degree / 180.0f; }
}
