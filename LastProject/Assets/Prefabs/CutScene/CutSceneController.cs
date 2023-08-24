using Cinemachine;
using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CutSceneController : NetworkBehaviour
{
    public NetworkObject StageOneBoss;
    public NetworkObject StageTwoBoss;
    public GameObject InGameUI;
    public GameObject stage1;
    public GameObject stage2;
    public CompleteGameUI completeGameUI;


    public void StageOneBossSpawn()
    {
        if (Object.HasStateAuthority)
        {
            Runner.Spawn(StageOneBoss, Utils.GetStageOneBossSpawnPoint(), Quaternion.identity, null, (runner, test) =>
            {
                RPC_UIActivate();
            });
        }
    }

    public void StageOneEnd()
    {
        //stage1.SetActive(false);
        //stage2.SetActive(true);
        if (Object.HasStateAuthority)
        {

            Runner.Spawn(StageTwoBoss, Utils.GetStageTwoBossSpawnPoint(), Quaternion.identity, null, (runner, test) =>
            {
                RPC_UIActivate(); 
                StageTwoPlayerSpawn();
            });
        }
    }

    public void CutScene3End()
    {
        if (Object.HasStateAuthority)
        {
            RPC_cutScene3End();
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_cutScene3End()
    {
        Camera.main.GetComponent<SoundManager>().PlayBGM(3);
        InGameUI.SetActive(true);
        GameObject boss = GameObject.FindGameObjectWithTag("Enemy");
        boss.transform.localScale = new Vector3(1f, 1f, 1f);
        boss.transform.position = Utils.GetStageTwoBossSpawnPoint();
        if (Object.HasStateAuthority) {
            boss.GetComponent<StrongMagicianManager>().endTask.Value = true;
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_UIActivate()
    {
        StartCoroutine(StartUI());
    }


    public void StageTwoPlayerSpawn()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        Debug.Log("found players count : " + players.Length);
        for (int i = 0; i < players.Length; i++)
        {
            players[i].transform.position = Utils.GetStageTwoPlayerSpawnPoint();
        }
    }

    IEnumerator StartUI()
    {
        bool check = false;
        
        while (true)
        {
            if (NetworkPlayer.Local != null && GameObject.FindGameObjectWithTag("Enemy") != null)
            {
                check = true;
                break;
            }
                

            if (NetworkPlayer.Local == null && GameObject.FindGameObjectWithTag("Enemy") != null)
                break;

            yield return null;
        }

        StageManager stageManager = GameObject.Find("StageManager").GetComponent<StageManager>();
        if (check) stageManager.ActiveInGameUI();
        else stageManager.ActiveDeathUI();
    }


    public void GameEnd()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        completeGameUI.CompleteGame();
    }

}
