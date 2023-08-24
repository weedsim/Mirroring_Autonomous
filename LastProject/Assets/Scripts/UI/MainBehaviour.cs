using Gravitons.UI.Modal;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainBehaviour : MonoBehaviour
{

    public GameObject LobbyFrame;
    public GameObject RankingFrame;

    public MainStatus Status { get; private set; } = MainStatus.None;

    public void Update()
    {
        if (Status == MainStatus.None && RoomManager.Instance.IsInitialized() && PlayerManager.Instance.IsInitialized())
        {
            Debug.Log("Enter Lobby init");
            LobbyButtonClicked();
        }
    }

    public void LobbyButtonClicked()
    {
        try
        {
            LobbyFrame.SetActive(true);
            RankingFrame.SetActive(false);
            RoomManager.Instance.JoinLobby("default");
            Status = MainStatus.Lobby;
        }
        catch (NotAllocatedPlayerByServerException e)
        {
            ModalManager.Show("Error", e.Message, new ModalButton[] { new() {Text="OK", Callback=()=>
                {
                    SceneManager.LoadScene((int)SceneDefs.SERVER);
                }
            }});
        }
        catch (InGameException e)
        {
            ModalManager.Show("Error", e.Message, new ModalButton[] { new() { Text = "Close" } });
        }
        
    }

    public void RankingButtonClicked()
    {
        
        try
        {
            LobbyFrame.SetActive(false);
            RankingFrame.SetActive(true);
            Status = MainStatus.Ranking;
        }
        catch (NotAllocatedPlayerByServerException e)
        {
            ModalManager.Show("Error", e.Message, new ModalButton[] { new() {Text="OK", Callback=()=>
                {
                    SceneManager.LoadScene((int)SceneDefs.SERVER);
                }
            }});
        }
        catch (InGameException e)
        {
            ModalManager.Show("Error", e.Message, new ModalButton[] { new() { Text = "Close" } });
        }
    }
}

public enum MainStatus
{
    None = 0,
    Lobby = 1,
    Ranking = 2,
}