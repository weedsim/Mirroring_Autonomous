using Gravitons.UI.Modal;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using WebSocketSharp;

public class RegisterValidatorBehaviour : MonoBehaviour
{

    public TMP_InputField IDInputField;

    public TMP_InputField PasswordInputField;

    public TMP_InputField PasswordConfirmInputField;

    public TMP_InputField NicknameInputField;

    public TMP_Text IDIInputMessage;

    public TMP_Text PasswordInputMessage;

    public TMP_Text PasswordConfirmInputMessage;

    public TMP_Text NicknameInputMessage;

    bool _idChecked;

    bool _idDuplicatedChecked;

    bool _passwordChecked;

    bool _passwordConfirmChecked;

    bool _nicknameChecked;


    public void Start()
    {
        _idChecked = false;
        _passwordChecked = false;
        _passwordConfirmChecked = false;
        _nicknameChecked = false;
        _idDuplicatedChecked = false;
        ClearMessage(IDIInputMessage);
        ClearMessage(PasswordInputMessage);
        ClearMessage(PasswordConfirmInputMessage);
        ClearMessage(NicknameInputMessage);
    }

    public bool IsAllChecked()
    {
        return _idChecked && _passwordChecked && _passwordConfirmChecked && _nicknameChecked && _idDuplicatedChecked;
    }

    public void CheckIDInput()
    {
        _idChecked = Between(IDInputField, IDIInputMessage, 4, 32, "ID");
        if (_idChecked)
        {
            OKMessage(IDIInputMessage, "OK, But, you should press Duplication Check Next.");
        }
        _idDuplicatedChecked = false;
    }

    public void CheckPasswordInput()
    {
        _passwordChecked = Between(PasswordInputField, PasswordInputMessage, 4, 32, "Password");
    }

    public void CheckPasswordConfirmInput()
    {
        _passwordConfirmChecked = ConfirmEquals(PasswordInputField, PasswordConfirmInputField, PasswordConfirmInputMessage);
    }

    public void CheckNicknameInput()
    {
        _nicknameChecked = Between(NicknameInputField, NicknameInputMessage, 4, 16, "Nickname");
    }

    

    public void CheckIDDUplication()
    {
        if (_idChecked)
        {
            if (!_idDuplicatedChecked)
            {
                AccountHttpManager.Instance.IdDuplicationCheck(IDInputField.text, (AuthResponse response) =>
                {
                    if (response.ResultCode == 1)
                    {
                        OKMessage(IDIInputMessage, response.Message);
                        _idDuplicatedChecked = true;
                    }
                    else
                    {
                        ErrorMessage(IDIInputMessage, response.Message);
                    }
                },
                (string errorText) =>
                {
                    ModalManager.Show("Server Error", errorText, new ModalButton[] { new() { Text = "Close" } });
                }
                );
            }
        }
    }

    public bool Between(TMP_InputField inputField, TMP_Text inputMessage, int least, int most, string target)
    {
        int length = inputField.text.Length;
        if(length >= least && length <= most)
        {
            ClearMessage(inputMessage);
            return true;
        }
        else
        {
            ErrorMessage(inputMessage, target + " must have 4~32 characters.");
            return false;
        }
    }

    public bool ConfirmEquals(TMP_InputField source, TMP_InputField target, TMP_Text inputMessage)
    {
        if (target.text.IsNullOrEmpty())
        {
            ClearMessage(inputMessage);
            return false;
        }
        if (source.text.Equals(target.text))
        {
            OKMessage(inputMessage, "OK");
            return true;
        }
        else
        {
            ErrorMessage(inputMessage, "Password is not equals with confirm");
            return false;
        }
    }

    public static void ErrorMessage(TMP_Text inputMessage,  string message)
    {
        inputMessage.text = message;
        inputMessage.color = Color.red;
    }

    public static void ClearMessage(TMP_Text inputMessage)
    {
        inputMessage.text = string.Empty;
    }

    public static void OKMessage(TMP_Text inputMessage, string message)
    {
        inputMessage.text = message;
        inputMessage.color = Color.green;
    }


}
