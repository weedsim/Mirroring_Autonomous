using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterClass Database", menuName = "Scriptable Object Asset/CharacterClassDatabase")]
public class CharacterClassDatabase : ScriptableObject
{
    public CharacterClassInfo[] CharacterClassInfos;
}
