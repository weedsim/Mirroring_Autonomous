using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
    [SerializeField]
    GameObject Boss;
    [SerializeField]
    GameObject Player;

    [SerializeField]
    TextMeshProUGUI _bossName;
    [SerializeField]
    HPHandler _bossHP;
    [SerializeField]
    Slider _bossHPBar;

    HPHandler _playerHP;
    Slider _playerHPBar;

    GameObject _skill;

    Image _qCoolTime;
    Image _eCoolTime;
    //Image _rCoolTime;
    Image _ctrlCoolTime;

    TextMeshProUGUI _qCoolTimeText;
    TextMeshProUGUI _eCoolTimeText;
    //TextMeshProUGUI _rCoolTimeText;
    TextMeshProUGUI _ctrlCoolTimeText;

    TextMeshProUGUI _bossHPText;
    TextMeshProUGUI _playerHPText;
    TextMeshProUGUI _playerName;

    WizardSkillHandler _wizardSkillHandler;
    BowmanSkillHandler _bowmanSkillHandler;
    WarriorSkillHandler _warriorSkillHandler;

    Utils.PlayerClass _playerClass;

    const float ep = 0.000001f;
    void OnEnable()
    {
        Player = NetworkPlayer.Local.gameObject;
        Boss = GameObject.FindWithTag("Enemy");

        _bossName = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        string tmp = Boss.name.Substring(0, Boss.name.IndexOf("(Clone"));
        
        if(tmp.Contains("One"))
        {
            _bossName.color = new Color(255 / 255f, 153 / 255f, 204 / 255f, 204 / 255f);
            _bossName.text = "Pink PrinCess";
        }

        else
        {
            _bossName.color = Color.black;
            _bossName.text = "Strong Magician";
        }

        _bossHPBar = transform.GetChild(1).GetComponent<Slider>();
        _bossHP = Boss.GetComponent<HPHandler>();
        _bossHPBar.maxValue = _bossHP.startingHP;
        _bossHPBar.value = _bossHPBar.maxValue; // ���� ü�¹� ����

        _bossHPText = transform.GetChild(1).GetChild(2).GetComponent<TextMeshProUGUI>();
        _bossHPText.text = $"{_bossHP.startingHP} / {_bossHP.startingHP}";

        _playerHPBar = transform.GetChild(2).GetChild(2).GetComponent<Slider>();
        _playerHP = Player.GetComponent<HPHandler>();
        _playerHPBar.maxValue = _playerHP.startingHP;
        _playerHPBar.value = _playerHPBar.maxValue; // �÷��̾� ü�¹� ����

        _playerHPText = transform.GetChild(2).GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>();
        _playerHPText.text = $"{_playerHP.startingHP} / {_playerHP.startingHP}";

        _playerName = transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>();
        _playerName.text = AccountManager.Nickname;

        _playerClass = Player.GetComponent<BasicController>()._playerClass;
        switch(_playerClass)
        {
            case Utils.PlayerClass.Archer:
                transform.GetChild(2).GetChild(1).GetChild(0).gameObject.SetActive(true);
                _skill = transform.GetChild(3).GetChild(0).gameObject;
                _bowmanSkillHandler = Player.GetComponent<BowmanSkillHandler>();
                break;
            case Utils.PlayerClass.Wizard:
                transform.GetChild(2).GetChild(1).GetChild(1).gameObject.SetActive(true);
                _skill = transform.GetChild(3).GetChild(1).gameObject;
                _wizardSkillHandler = Player.GetComponent<WizardSkillHandler>();
                break;
            default:
                transform.GetChild(2).GetChild(1).GetChild(2).gameObject.SetActive(true);
                _skill = transform.GetChild(3).GetChild(2).gameObject;
                _warriorSkillHandler = Player.GetComponent<WarriorSkillHandler>();
                break;
        }

        _skill.SetActive(true);

        _qCoolTime = _skill.transform.GetChild(0).GetChild(1).GetComponent<Image>();
        _eCoolTime = _skill.transform.GetChild(1).GetChild(1).GetComponent<Image>();
        //_rCoolTime = _skill.transform.GetChild(3).GetChild(1).GetComponent<Image>();
        _ctrlCoolTime = _skill.transform.GetChild(2).GetChild(1).GetComponent<Image>();

        _qCoolTimeText = _qCoolTime.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        _eCoolTimeText = _eCoolTime.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        //_rCoolTimeText = _rCoolTime.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        _ctrlCoolTimeText = _ctrlCoolTime.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
    }
    void Update()
    {
        if (_bossHPBar != null && _bossHP != null)
        {
            _bossHPBar.value = _bossHP.HP >= 0 ? _bossHP.HP : 0;
            _bossHPText.text = $"{_bossHPBar.value} / {_bossHP.startingHP}";
        }

        if (_playerHPBar != null && _playerHP != null)
        {
            _playerHPBar.value = _playerHP.HP >= 0 ? _playerHP.HP : 0;
            _playerHPText.text = $"{_playerHPBar.value} / {_playerHP.startingHP}";
        }

        float tmp;
        switch (_playerClass)
        {
            case Utils.PlayerClass.Archer:

                tmp = _bowmanSkillHandler.QCur < 0 ? 0 : _bowmanSkillHandler.QCur;
                _qCoolTime.fillAmount = tmp / _bowmanSkillHandler.QCool;
                _qCoolTimeText.text = (tmp < ep) ? "" : ((int)tmp).ToString();

                tmp = _bowmanSkillHandler.ECur < 0 ? 0 : _bowmanSkillHandler.ECur;
                _eCoolTime.fillAmount = tmp / _bowmanSkillHandler.ECool;
                _eCoolTimeText.text = (tmp < ep) ? "" : ((int)tmp).ToString();

                //tmp = _bowmanSkillHandler.RCur < 0 ? 0 : _bowmanSkillHandler.RCur;
                //_rCoolTime.fillAmount = tmp / _bowmanSkillHandler.RCool;
                //_rCoolTimeText.text = (tmp < ep) ? "" : ((int)tmp).ToString();

                tmp = _bowmanSkillHandler.AvoidCur < 0 ? 0 : _bowmanSkillHandler.AvoidCur;
                _ctrlCoolTime.fillAmount = tmp / _bowmanSkillHandler.AvoidCool;
                _ctrlCoolTimeText.text = (tmp < ep) ? "" : ((int)tmp).ToString();

                break;
            case Utils.PlayerClass.Wizard:
                tmp = _wizardSkillHandler.QCur < 0 ? 0 : _wizardSkillHandler.QCur;
                _qCoolTime.fillAmount = tmp / _wizardSkillHandler.QCool;
                _qCoolTimeText.text = (tmp < ep) ? "" : ((int)tmp).ToString();

                tmp = _wizardSkillHandler.ECur < 0 ? 0 : _wizardSkillHandler.ECur;
                _eCoolTime.fillAmount = tmp / _wizardSkillHandler.ECool;
                _eCoolTimeText.text = (tmp < ep) ? "" : ((int)tmp).ToString();

                //tmp = _wizardSkillHandler.RCur < 0 ? 0 : _wizardSkillHandler.RCur;
                //_rCoolTime.fillAmount = tmp / _wizardSkillHandler.RCool;
                //_rCoolTimeText.text = (tmp < ep) ? "" : ((int)tmp).ToString();

                tmp = _wizardSkillHandler.CtrlCur < 0 ? 0 : _wizardSkillHandler.CtrlCur;
                _ctrlCoolTime.fillAmount = tmp / _wizardSkillHandler.CtrlCool;
                _ctrlCoolTimeText.text = (tmp < ep) ? "" : ((int)tmp).ToString();
                break;
            default:
                tmp = _warriorSkillHandler.QCur < 0 ? 0 : _warriorSkillHandler.QCur;
                _qCoolTime.fillAmount = tmp / _warriorSkillHandler.QCool;
                _qCoolTimeText.text = (tmp < ep) ? "" : ((int)tmp).ToString();

                tmp = _warriorSkillHandler.ECur < 0 ? 0 : _warriorSkillHandler.ECur;
                _eCoolTime.fillAmount = tmp / _warriorSkillHandler.ECool;
                _eCoolTimeText.text = (tmp < ep) ? "" : ((int)tmp).ToString();

                //tmp = _warriorSkillHandler.RCur < 0 ? 0 : _warriorSkillHandler.RCur;
                //_rCoolTime.fillAmount = tmp / _warriorSkillHandler.RCool;
                //_rCoolTimeText.text = (tmp < ep) ? "" : ((int)tmp).ToString();

                tmp = _warriorSkillHandler.AvoidCur < 0 ? 0 : _warriorSkillHandler.AvoidCur;
                _ctrlCoolTime.fillAmount = tmp / _warriorSkillHandler.AvoidCool;
                _ctrlCoolTimeText.text = (tmp < ep) ? "" : ((int)tmp).ToString();

                break;
        }
    }
}
