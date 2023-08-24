using Gravitons.UI.Modal;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RegisterModal : Modal
{
    [Tooltip("Modal title")]
    [SerializeField] protected TMP_Text m_Title;

    [Tooltip("Create Button")]
    [SerializeField] protected Button m_Create;
    [Tooltip("Create Button")]
    [SerializeField] protected Button m_Cancel;
    [Tooltip("Validator")]
    [SerializeField] protected RegisterValidatorBehaviour m_RegisterValidator;

    public void Awake()
    {
        m_Title.text = "Register";
        m_Create.gameObject.SetActive(true);
        m_Cancel.gameObject.SetActive(true);
        m_Create.onClick.AddListener(() =>
        {
            CreateAccount();
        });

        m_Cancel.onClick.AddListener(() =>
        {
            Close();
        });

    }

    public override void Show(ModalContentBase modalContent, ModalButton[] modalButton)
    {
        m_Create.gameObject.SetActive(true);
        m_Cancel.gameObject.SetActive(true);
    }

    public void CreateAccount()
    {
        if (m_RegisterValidator.IsAllChecked())
        {
            AccountInfo accountInfo = new();
            accountInfo.id = m_RegisterValidator.IDInputField.text;
            accountInfo.password = m_RegisterValidator.PasswordInputField.text;
            accountInfo.nickname = m_RegisterValidator.NicknameInputField.text;
            AccountHttpManager.Instance.PostAccount(accountInfo, (AuthResponse response) =>
            {
                if (response.ResultCode == 1)
                {
                    ModalManager.Show("Register Complete", "Register Complete. Login Please!!", new ModalButton[] { new() {
                        Text = "OK",
                        Callback = () =>
                        {
                            Close();
                        }
                    } });
                }
                else
                {
                    ModalManager.Show("Register Failed", response.Message, new ModalButton[] { new() { Text = "Close" } });
                }
            },
            (string errorText) =>
            {
                ModalManager.Show("Server Error", errorText, new ModalButton[] { new() { Text = "Close" } });
            }
            );
        }
        else
        {
            ModalManager.Show("Register Failed", "Validation Failed, Check the message!!", new ModalButton[] { new() { Text = "Close" } });
        }
    }
}
