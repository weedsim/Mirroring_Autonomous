using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [System.Serializable]
    public struct BGMType
    {
        public string name;
        public AudioClip audio;
    }
    public BGMType[] BGMList;
    private AudioSource BGM;
    private string NowBGMname = "";
    public int count = 0;

    void Start()
    {
        BGM = gameObject.AddComponent<AudioSource>();
        BGM.loop = true;
    }

    public void PlayBGM(int test)
    {
        BGM.clip = BGMList[test].audio;
        BGM.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
