using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningHandler : NetworkBehaviour
{

    NetworkObject networkObject;

    [Header("Target Posotion")]
    public Vector3 target;

    // for create
    public void LightningCreate(Vector3 startPosition, Vector3 targetPosition)
    {
        Vector3 restartPosition = (targetPosition + startPosition) / 2.0f;
        transform.position = restartPosition;
        transform.LookAt(targetPosition);
        float lightningScale = Vector3.Distance(targetPosition, startPosition);
        if (lightningScale < 1.0f) { lightningScale = 2.0f; }
        transform.localScale = new Vector3(1.0f, 1.0f, (lightningScale - 1.0f ));
        networkObject = GetComponent<NetworkObject>();
    }

    // for delete
    public void LightningDelete()
    {
        if (networkObject != null)
        {
            Runner.Despawn(networkObject);
        }
    }

    // for change lightning length
    public void LightningUpdate(Vector3 startPosition, Vector3 targetPosition)
    {
        Vector3 restartPosition = (targetPosition + startPosition) / 2.0f;
        transform.position = restartPosition - startPosition;
        Debug.Log("restartPosition " + restartPosition);
        Debug.Log("targetPosition " + targetPosition);
        transform.LookAt(targetPosition - startPosition);
        float lightningScale = Vector3.Distance(targetPosition, startPosition);
        if(lightningScale < 1.0f) { lightningScale = 2.0f; }
        transform.localScale = new Vector3(1.0f, 1.0f, lightningScale - 1.0f);
    }

    public override void FixedUpdateNetwork()
    {
        //if (tickTimer.Expired(Runner))
        //{
        //    Runner.Despawn(networkObject);
        //    return;
        //}
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (collision.gameObject.GetComponentInParent<HPHandler>() != null)
            {
                collision.gameObject.GetComponentInParent<HPHandler>().OnTakeDamage(5);
            }
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (collision.gameObject.GetComponentInParent<HPHandler>() != null)
            {
                collision.gameObject.GetComponentInParent<HPHandler>().OnTakeDamage(5);
            }
        }
    }
}
