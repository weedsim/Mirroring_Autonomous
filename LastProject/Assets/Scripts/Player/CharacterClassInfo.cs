using System;
using UnityEngine;

[Serializable]
public class CharacterClassInfo
{
    public int CharacterClassId;
    public string CharacterClassName;
    public Sprite CharacterClassImage;
    public NetworkPlayer playerPrefab;
}
