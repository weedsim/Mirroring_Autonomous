using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MapChooserBehaviour : MonoBehaviour, IChooserBehaviour
{

    public TMP_Dropdown dropdown;

    MapManager _mapManager;

    private List<int> _dropdownToMapId;

    private void Start()
    {
        _mapManager = MapManager.Instance;
        _dropdownToMapId = new List<int>();
        BuildDropdown();
    }

    private void Update()
    {
        if (_mapManager.IsInitialized())
        {
            if (_mapManager.UIRefreshNeeded)
            {
                _mapManager.AnnounceUIRefreshed();
                BuildDropdown();
            }
        }
    }

    public void BuildDropdown()
    {
        dropdown.options.Clear();
        _dropdownToMapId.Clear();
        foreach (MapInfo mi in _mapManager.GetAllMapInfo())
        {
            TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData();
            option.text = mi.MapName;
            _dropdownToMapId.Add(mi.MapId);
            dropdown.options.Add(option);
        }
        dropdown.value = 0;
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
        return _dropdownToMapId[dropdown.value];
    }
}
