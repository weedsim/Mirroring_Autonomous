using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : NetworkBehaviour
{

    public GameObject enemy;
    public GameObject UI;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Runner.Spawn(enemy,Utils.GetStageOneBossSpawnPoint(), Quaternion.identity);
            UI.SetActive(true);
        }
    }
}
