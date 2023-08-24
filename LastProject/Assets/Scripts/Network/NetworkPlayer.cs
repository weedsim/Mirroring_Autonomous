using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Cinemachine;
using UnityEngine.Playables;

public class NetworkPlayer : NetworkBehaviour, IPlayerLeft
{
    public static NetworkPlayer Local { get; set; }

    public Transform playerModel;
    GameObject playerCameraObject;
    CinemachineVirtualCamera cinemachineVirtualCamera;

    public Transform playerCameraRoot;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public override void Spawned()
    {
        if(StageManager.PlayerList == null)
            StageManager.PlayerList = new List<GameObject>(); 

        StageManager.PlayerList.Add(gameObject);

        if (Object.HasInputAuthority)
        {
            Local = this;

            RPC_countUP(gameObject.name);

            Debug.Log("Spawned local player");

            Debug.LogError($"Spawned local player P_{Object.Id}");
        }
        
        else
        {
            Debug.Log("Spawned remote player");

            Debug.LogError($"Spawned remote player P_{Object.Id}");
        }

        transform.name = $"P_{Object.Id}";

        if (Object.HasStateAuthority)
        {
            Debug.LogError("Host입니다.");
            ArrowShooted._hasInput = true;
        }
    }

    public void PlayerLeft(PlayerRef player)
    {
        if (player == Object.InputAuthority)
        {
            Runner.Despawn(Object);
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_countUP(string name)
    {
        Debug.Log($"{name}가 호출");
        StageManager stageManager = GameObject.Find("StageManager").GetComponent<StageManager>();
        stageManager._sceneLoadCount++;
    }
}
