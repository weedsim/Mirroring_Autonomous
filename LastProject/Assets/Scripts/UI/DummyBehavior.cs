using Fusion;
using Gravitons.UI.Modal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    public PopupUIBehaviour pub;

    //NetworkRunner _runner;

    private void Start()
    {
        //_runner = NetworkRunner.GetRunnerForGameObject(gameObject);
    }

    public void OpenRoomCreater()
    {
        ModalManager.Show("RoomCreater", new ModalContentBase(), null);

    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            NetworkObject player = collider.GetComponent<NetworkObject>();
            if (player.HasInputAuthority)
            {
                if (!pub.gameObject.activeSelf)
                {
                    RoomManager.Instance.JoinLobby("default");
                    pub.Open();
                }
            }
        }
    }

    
}
