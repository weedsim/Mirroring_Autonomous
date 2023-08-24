using Gravitons.UI.Modal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomCreateBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    public void ShowRoomCreater()
    {
        ModalManager.Show("Create Room", "Content", null);
    }
}