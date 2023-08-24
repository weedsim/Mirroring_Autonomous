using Cinemachine;
using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeathUI : NetworkBehaviour
{
    public CinemachineVirtualCamera[] _playerFollowCam;

    int index = 0;
    void OnEnable()
    {
        if (StageManager.PlayerList[index] == NetworkPlayer.Local.gameObject)
            index++;

        if (StageManager.PlayerList[index] != null)
        {
            _playerFollowCam[0].Follow = _playerFollowCam[1].Follow = StageManager.PlayerList[index].transform.GetChild(0);
            _playerFollowCam[0].LookAt = _playerFollowCam[1].Follow = StageManager.PlayerList[index].transform.GetChild(0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (StageManager.PlayerList.Count == 0)
        {
            if (Object.HasStateAuthority)
                RPC_GameOver();
        }

        if (_playerFollowCam[0].LookAt == null)
        { 
            index = (index + 1) % StageManager.PlayerList.Count;
        }
        
        if (Input.GetKeyDown(KeyCode.Tab)) 
        {
            index = (index + 1) % StageManager.PlayerList.Count;

            if (StageManager.PlayerList[index] != null)
            {
                _playerFollowCam[0].Follow = _playerFollowCam[1].Follow = StageManager.PlayerList[index].transform.GetChild(0);
                _playerFollowCam[0].LookAt = _playerFollowCam[1].Follow = StageManager.PlayerList[index].transform.GetChild(0);
            }

        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_GameOver()
    {
        StageManager stageManager = GameObject.Find("StageManager").GetComponent<StageManager>();
        stageManager.ChangeToGameOverUI();
    }
}
