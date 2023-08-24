using Gravitons.UI.Modal;
using TMPro;
using UnityEngine;

public class LoginBehaviour : MonoBehaviour
{

    private ModalContentBase dummy;

    public NetworkClientManager ClientManager;
    public TMP_InputField IdField;
    public TMP_InputField TextField;

    public void Login()
    {
        string username = IdField.GetComponent<TMP_InputField>().text;
        string password = TextField.GetComponent<TMP_InputField>().text;

        Debug.Log(username);
        Debug.Log(password);

        ClientManager.Login(username, password);

    }

    public void Register()
    {
        ModalManager.Show("Register", dummy, null);

    }
}
