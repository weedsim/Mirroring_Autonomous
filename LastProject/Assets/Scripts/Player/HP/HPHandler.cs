using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.UI;
using TMPro;
using MBT;
using UnityEngine.Playables;

public class HPHandler : NetworkBehaviour
{
    [Networked]public int whoIsAttack { get; set; }
    [SerializeField]
    [Networked(OnChanged = nameof(OnHPChanged))]
    public int HP { get; set; }

    [Networked(OnChanged = nameof(OnStateChanged))]
    public bool isDead { get; set; }

    public GameObject hpObject;

    [SerializeField]
    public int startingHP = 10000;

    [Networked] public bool isProtected { get; set; }

    public int myPlayerID;
    public bool isBoss = false;
    [SerializeField]
    public IntReference[] hpList;
    public DynamicTextData[] dtds;
    public float offsetY = 3.7f;
    public float offsetXZ = 1f;
    public float randomRange = 0.3f;
    private void Awake()
    {
        //characterMovementHandler = GetComponent<CharacterMovementHandler>();
        //hpText = GetComponentInChildren<TextMeshProUGUI>();
    }

    // Start is called before the first frame update

    public override void Spawned()
    {
        HP = startingHP;
        isDead = false;
        isProtected = false;

        

        if (isBoss)
        {
            NetworkRunner[] networkRunners = FindObjectsOfType<NetworkRunner>();
            foreach (NetworkRunner networkRunner in networkRunners)
            {
                if (networkRunner.SessionInfo.Name != "Lobby")
                {
                    myPlayerID = networkRunner.LocalPlayer.PlayerId;
                    break;
                }
            }

            if (Object.HasStateAuthority)
            {
                hpList[0].Value = HP;
                hpList[1].Value = 5*HP / 10;
                if (hpList.Length > 2) hpList[2].Value = 3*HP / 10;
            }
        }
    }

    IEnumerator ServerReviveCO()
    {
        yield return new WaitForSeconds(2.0f);

        //characterMovementHandler.RequestRespawn();
    }


    //Function only called on the server
    public void OnTakeDamage(int damage, int playerID=-1)
    {
        if (playerID != -1) whoIsAttack = playerID;
        //Only take damage while alive
        if (isDead)
            return;
        if (isProtected)
            damage = isBoss ? -damage : 1;
        HP -= damage;
        if (isBoss)
        {
            if (Object.HasStateAuthority) hpList[0].Value = HP;
        }
        //Debug.Log($"{Time.time} {transform.name} took damage got {HP} left ");
        //Player died
        if (HP <= 0)
        {
            //Debug.Log($"{Time.time} {transform.name} died");
            //StartCoroutine(ServerReviveCO());
            isDead = true;
        }
    }


    static void OnHPChanged(Changed<HPHandler> changed)
    {
        int newHP = changed.Behaviour.HP;
        changed.LoadOld();
        int oldHP = changed.Behaviour.HP;
        changed.Behaviour.DamageTextPop(oldHP - newHP);
    }

    private void DamageTextPop(int damage)
    {
        StartCoroutine(DamageCRT(damage));
    }
    IEnumerator DamageCRT(int damage)
    {
        yield return null;
        if (!isBoss)
        {
            DynamicTextManager.CreateText(GetRandomOffset(transform.position), damage.ToString(), dtds[0]);
        }
        else
        {
            if (damage < 0) DynamicTextManager.CreateText(GetRandomOffset(transform.position), $"+{(-damage).ToString()}", dtds[1]);
            else if (whoIsAttack == myPlayerID)
            {
                DynamicTextManager.CreateText(GetRandomOffset(transform.position), damage.ToString(), dtds[2]);
                Debug.Log("내공격");
            }

            else
            {
                DynamicTextManager.CreateText(GetRandomOffset(transform.position), damage.ToString(), dtds[0]);
                Debug.Log("남의공격");
            }
            if (Object.HasStateAuthority) whoIsAttack = -1;
        }
    }



    private Vector3 GetRandomOffset(Vector3 pos)
    {
        Vector3 newOffset = transform.forward * (-offsetXZ) + Vector3.up * (offsetY+Random.Range(-randomRange,randomRange)) + transform.right*Random.Range(-randomRange,randomRange);
        return new Vector3(pos.x+newOffset.x, pos.y+newOffset.y, pos.z+newOffset.z);
    }

    static void OnStateChanged(Changed<HPHandler> changed)
    {
        Debug.Log($"{Time.time} OnStateChanged isDead {changed.Behaviour.isDead}");

        bool isDeadCurrent = changed.Behaviour.isDead;

        if (isDeadCurrent)
            changed.Behaviour.OnDeath();
    }

    private void OnDeath()
    {
        if (gameObject.CompareTag("Enemy"))
        {
            StartCoroutine(StageManager.StartCutScene2or3or4(gameObject.name));
        }

        if (NetworkPlayer.Local != null && gameObject == NetworkPlayer.Local.gameObject)
        {
            StageManager stageManager = GameObject.Find("StageManager").GetComponent<StageManager>();
            stageManager.DeActiveInGameUI();

            stageManager.ActiveDeathUI();
        }

        if (gameObject.CompareTag("Player"))
        {
            Debug.LogError(StageManager.PlayerList.Remove(gameObject));
        }

        Debug.Log($"{Time.time} OnDeath");
        //hpObject.gameObject.SetActive( false );
        Destroy(gameObject);
        //hitboxRoot.HitboxRootActive = false;
    }
}
