using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Scene1CharacterSelect : MonoBehaviour
{
    // Start is called before the first frame update
    public void ReceiveSignal()
    {
        Utils.PlayerClass pClass = Utils.PlayerClass.Wizard;
        
        if(NetworkPlayer.Local != null)
            pClass = NetworkPlayer.Local.GetComponent<BasicController>()._playerClass;
        
        transform.GetChild((int)pClass).gameObject.SetActive(true);
    }
}
