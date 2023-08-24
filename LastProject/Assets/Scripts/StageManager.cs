using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Cinemachine;

public class StageManager : NetworkBehaviour
{ 
    static int _count = 0;
    public static List<GameObject> PlayerList { get; set; }

    bool check = false;

    public GameObject InGameUI;
    public GameObject PlayerDeathUI;
    public GameObject GameOverUI;
    public GameObject MouseCentering;

    public string bgmName = "";
    private GameObject CamObject;

    int _playerCount;
    public int _sceneLoadCount;

    void Start()
    {
        _sceneLoadCount = 0;
        _playerCount = RoomManager.Instance.GetNetworkInGameStarterManager().GetNetworkRunner().SessionInfo.Properties["playerCount"];

        Debug.Log(_playerCount);
    }
    void Update()
    {
        if (!check && _playerCount == _sceneLoadCount)
        {
            Debug.Log($"{_sceneLoadCount} / {_playerCount}");
            RPC_StartCutScene();
        }
    }
    public static IEnumerator StartCutScene1()
    {   
        GameObject cutScene = GameObject.FindWithTag("CutScene");
       
        cutScene = cutScene.transform.GetChild(0).gameObject;
        cutScene.SetActive(true);
        PlayableDirector cutScenePlay = cutScene.GetComponent<PlayableDirector>();
        cutScenePlay.Play();

        Camera.main.GetComponent<SoundManager>().PlayBGM(0);
        yield return null;
    }

    public static IEnumerator StartCutScene2or3or4(string name)
    {
        GameObject InGameUI = GameObject.Find("In-Game_UI");
        
        if (InGameUI != null)
            InGameUI.SetActive(false);

        GameObject cutScene = GameObject.FindWithTag("CutScene");
        if (name.Contains("One"))
        { 
            cutScene = cutScene.transform.GetChild(1).gameObject;
            Camera.main.GetComponent<SoundManager>().PlayBGM(1);
        
        }

        else
        {
            if (_count == 0)
            {
                cutScene = cutScene.transform.GetChild(2).gameObject;
                _count++;
                Camera.main.GetComponent<SoundManager>().PlayBGM(2);
            }
            else
            { 
                cutScene = cutScene.transform.GetChild(3).gameObject;
            }

        }

        cutScene.SetActive(true);

        PlayableDirector cutScenePlay = cutScene.GetComponent<PlayableDirector>();
        
        cutScenePlay.Play();
        
        yield return null;
    }

    public void ActiveInGameUI()
    {
        InGameUI.SetActive(true);
    }
    public void ActiveDeathUI()
    {
        MouseCentering.SetActive(false);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        PlayerDeathUI.SetActive(true);
    }

    public void ActiveGameOverUI()
    {
        MouseCentering.SetActive(false);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        GameOverUI.SetActive(true);
    }

    public void DeActiveInGameUI()
    {
        InGameUI.SetActive(false);
    }
    public void DeActiveDeathUI()
    {
        MouseCentering.SetActive(true);
        PlayerDeathUI.SetActive(false);
    }

    public void DeActiveGameOverUI()
    {
        GameOverUI.SetActive(false);
    }

    public void ChangeToGameOverUI()
    {
        DeActiveDeathUI();
        ActiveGameOverUI();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_StartCutScene()
    {
        StartCoroutine(StartCutScene1());

        GameObject[] cams = GameObject.FindGameObjectsWithTag("playerfollowcam");
        foreach (GameObject cam in cams)
        {
            CinemachineVirtualCamera vcam = cam.GetComponent<CinemachineVirtualCamera>();

            vcam.LookAt = vcam.Follow = NetworkPlayer.Local.transform.GetChild(0);
        }

        check = true;
    }
}
