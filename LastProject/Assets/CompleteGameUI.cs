using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompleteGameUI : NetworkBehaviour
{

    public InGameCompleteManager CompleteGameManager;

    public TimeManager Timer;


    [Rpc(sources:RpcSources.StateAuthority, targets:RpcTargets.All)]
    public void RPC_CompleteGame(long completeTime)
    {
        CompleteGameModalContent modalContent = new() { CompleteTime = completeTime };

        if (Runner.IsServer)
        {
            CompleteGameManager.RequestCompleteGame(Runner, modalContent);
        }
        else
        {
            CompleteGameManager.PopupCompleteModal(Runner, modalContent);
        }
    }

    public void CompleteGame()
    {
        float recordTime = Timer.time;
        long completeTime = (long)recordTime;
        RPC_CompleteGame(completeTime);
    }

}
