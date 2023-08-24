using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterClassChooserBehaviour : MonoBehaviour, IChooserBehaviour
{

    public TMP_Dropdown dropdown;

    PlayerManager _playerManager;
    CharacterClassManager _characterClassManager;

    private List<int> _dropdownToCharacterClassId;

    private void Start()
    {
        _playerManager = PlayerManager.Instance;
        _characterClassManager = CharacterClassManager.Instance;
        _dropdownToCharacterClassId = new List<int>();
        BuildDropdown();
    }

    private void Update()
    {
        if (_characterClassManager.IsInitialized() && _playerManager.IsInitialized()) 
        {
            if (_characterClassManager.UIRefreshNeeded)
            {
                BuildDropdown();
                _characterClassManager.AnnounceUIRefreshed();
            }
        }
    }

    public void BuildDropdown()
    {
        dropdown.options.Clear();
        _dropdownToCharacterClassId.Clear();
        int defaultId = _playerManager.GetLocalPlayerInfo().CharacterClassId;
        foreach (CharacterClassInfo cci in _characterClassManager.GetAllCharacterClassInfo())
        {
            TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData();
            option.text = cci.CharacterClassName;
            _dropdownToCharacterClassId.Add(cci.CharacterClassId);
            dropdown.options.Add(option);
            if(cci.CharacterClassId == defaultId && _dropdownToCharacterClassId.Count > 0)
            {
                dropdown.value = _dropdownToCharacterClassId.Count - 1;
            }
        }
    }

    public void OnValueChanged(int value)
    {
        Debug.Log("Choose value : " + value);
    }

    public TMP_Dropdown GetDropdown()
    {
        return dropdown;
    }

    public int GetChoosedId()
    {
        return _dropdownToCharacterClassId[dropdown.value];
    }
}
