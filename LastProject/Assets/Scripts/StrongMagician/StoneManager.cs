using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.CullingGroup;

public class StoneManager : NetworkBehaviour
{
    public int count;
    
    [Networked(OnChanged = nameof(NowChanged))]
    public int Now { get; set; }

    [Networked(OnChanged = nameof(OnStateChanged))]
    public bool Break { get; set; }

    public StonesManager stm;
    
    public Material newMaterial;
    public GameObject ps;
    public DynamicTextData dtd;

    public void OnTakeDamage()
    {
        Debug.Log($"{Now} / {count}");
        if (Now > 0)
            Now--;
        
        if (Now == 0)
            Break = true;
    }

    static void NowChanged(Changed<StoneManager> changed)
    {
        int NowCurrent = changed.Behaviour.Now;
        changed.LoadOld();
        int NowOld = changed.Behaviour.Now;

        if (NowCurrent < NowOld)
            changed.Behaviour.PrintNow();
    }
    
    void PrintNow()
    {
        DynamicTextManager.CreateText(transform.position + (Vector3.down * 2.0f) + (transform.forward * 2.0f), $"{Now - 1} / {count}", dtd);
    }

    static void OnStateChanged(Changed<StoneManager> changed)
    {
        bool BreakCurrent = changed.Behaviour.Break;
        changed.LoadOld();
        bool BreakOld = changed.Behaviour.Break;

        if (BreakCurrent && !BreakOld)
            changed.Behaviour.UpdateCount();
    }

    void UpdateCount()
    {
        Renderer renderer = GetComponent<Renderer>();
        renderer.material = newMaterial;
        stm.SetCount();
        ps.SetActive(false);
    }

}
