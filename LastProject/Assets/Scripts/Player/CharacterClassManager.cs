using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterClassManager : Singleton<CharacterClassManager>
{
    CharacterClassDatabase CharacterClassDatabase;

    Dictionary<int,CharacterClassInfo> CharacterClassInfos;

    CharacterClassInfo defaultClassInfo;

    bool _isInitialized;

    public bool UIRefreshNeeded { get; private set; }

    public bool IsInitialized()
    {
        return _isInitialized;
    }
    public void AnnounceUIRefreshed()
    {
        UIRefreshNeeded = false;
    }

    public override void Initialize()
    {
        CharacterClassDatabase = Resources.Load<CharacterClassDatabase>("Database/CharacterClassDatabase");
        _isInitialized = false;
        CharacterClassInfos = new Dictionary<int, CharacterClassInfo>();
        foreach (CharacterClassInfo cci in CharacterClassDatabase.CharacterClassInfos)
        {
            CharacterClassInfos[cci.CharacterClassId] = cci;
        }
        defaultClassInfo = CharacterClassDatabase.CharacterClassInfos[0];
        _isInitialized = true;
        UIRefreshNeeded = true;
    }

    public CharacterClassInfo GetCharacterClassInfo(int characterClassId)
    {
        return CharacterClassInfos[characterClassId];
    }
    
    public IEnumerable<CharacterClassInfo> GetAllCharacterClassInfo()
    {
        return CharacterClassInfos.Values;
    }

    public int GetDefaultCharacterClassId()
    {
        return defaultClassInfo.CharacterClassId;
    }

    public CharacterClassInfo GetDefaultCharacterClassInfo()
    {
        return defaultClassInfo;
    }



}
