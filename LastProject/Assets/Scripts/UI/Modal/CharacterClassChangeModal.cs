using Gravitons.UI.Modal;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterClassChangeModal : Modal
{
    [Tooltip("Modal title")]
    [SerializeField] protected TMP_Text m_Title;

    [SerializeField] protected CharacterClassChooserBehaviour m_CharacterClassChooserBehaviour;

    [Tooltip("OK Button")]
    [SerializeField] protected Button m_Ok;
    [Tooltip("Cancel Button")]
    [SerializeField] protected Button m_Cancel;

    public void Awake()
    {
        m_Title.text = "Change Character Class";
        m_Ok.gameObject.SetActive(true);
        m_Cancel.gameObject.SetActive(true);
        m_Ok.onClick.AddListener(() =>
        {
            try
            {
                PlayerManager.Instance.ChangeCharacterClass(m_CharacterClassChooserBehaviour.GetChoosedId());
                Close();
            }catch(InGameException e) 
            {
                ModalManager.Show("Error", e.Message, new ModalButton[] { new() { Text = "Close" } });
            }
        });

        m_Cancel.onClick.AddListener(() =>
        {
            Close();
        });

    }

    public override void Show(ModalContentBase modalContent, ModalButton[] modalButton)
    {
        m_Ok.gameObject.SetActive(true);
        m_Cancel.gameObject.SetActive(true);

    }
}