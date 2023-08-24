using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    static Vector3[] StageOneSpawnPos =
    { new Vector3(-71.45f,4.10f,27.70f),
          new Vector3(-57.00f,3.50f,26.73f),
          new Vector3(-66.57f,1.00f,-18.20f),
          new Vector3(-81.51f,1.00f,-17.09f)
        };


    static Vector3[] StageTwoSpawnPos =
    { new Vector3(-109.00f,-238.0f,26.00f),
          new Vector3(-175.00f,-238.0f,28.73f),
          new Vector3(-179.00f,-238.0f,-32.20f),
          new Vector3(-124.51f,-238.0f,-26.09f)
    };

    public static int GetRandomDamage(int i)
    {
        int temp = i / 10;
        return i + Random.Range(-temp, temp+1);
    }
    public static Vector3 GetRandomSpawnPoint()
    {
        return new Vector3(Random.Range(-53f, -54f), 1.5f, Random.Range(-1, 1));
    }

    public static Vector3 GetStageOnePlayerSpawnPoint()
    {
        int posIdx = Random.Range(0, 4);
        return StageOneSpawnPos[posIdx];
    }

    public static Vector3 GetStageTwoPlayerSpawnPoint()
    {
        int posIdx = Random.Range(0, 4);
        return StageTwoSpawnPos[posIdx];
    }

    public static Vector3 GetStageThreeTestPlayerSpawnPoint()
    {
        return new Vector3(-40.00f, -17.00f, 18.00f);
    }


    public static Vector3 GetStageOneBossSpawnPoint()
    {
        return new Vector3(-56.0f,0.0f,0.0f);
    }

    public static Vector3 GetStageTwoBossSpawnPoint()
    {
        return new Vector3(-148.71f, -237.0f, -4.05f);
    }


    public static void SetRenderLayerInChildren(Transform transform, int layerNumber)
    {
        foreach (Transform trans in transform.GetComponentsInChildren<Transform>(true))
            trans.gameObject.layer = layerNumber;
    }

    public enum PlayerClass
    {
        Wizard,
        Archer,
        Warrior,
    }


}
