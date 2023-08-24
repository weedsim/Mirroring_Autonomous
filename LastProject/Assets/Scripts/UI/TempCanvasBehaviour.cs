using UnityEngine;

public class TempCanvasBehaviour : MonoBehaviour
{

    public PopupUIBehaviour PopupUIBehavior;

    // Start is called before the first frame update
    void Start()
    {

#if UNITY_SERVER
    gameObject.SetActive(false);
#endif
    }

    public void JoinLobby()
    {
        if(RoomManager.Instance.IsInitialized() && PlayerManager.Instance.IsInitialized())
        {
            RoomManager.Instance.JoinLobby("default");
            PopupUIBehavior.Open();
        }
    }

    public void ChangeCharacterClass()
    {
        if(PlayerManager.Instance.IsInitialized())
        {

        }
    }


}
