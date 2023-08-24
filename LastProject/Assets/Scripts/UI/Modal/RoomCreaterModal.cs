using Gravitons.UI.Modal;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomCreaterModal : Modal
{
    [Tooltip("Modal title")]
    [SerializeField] protected TMP_Text m_Title;

    [SerializeField] protected TMP_InputField m_RoomName;

    [SerializeField] protected MapChooserBehaviour m_MapChooserBehaviour;

    [Tooltip("Create Button")]
    [SerializeField] protected Button m_Create;
    [Tooltip("Create Button")]
    [SerializeField] protected Button m_Cancel;

    public void Awake()
    {
        m_Title.text = "Create Room";
        m_Create.gameObject.SetActive(true);
        m_Cancel.gameObject.SetActive(true);
        m_MapChooserBehaviour.dropdown.gameObject.SetActive(true);
        m_Create.onClick.AddListener(() =>
        {
            if(m_RoomName.text.Length == 0)
            {
                ModalManager.Show("Room Create Failed", "Room Name must not be empty !!", new ModalButton[] { new() { Text="OK", Callback = (() =>{Close();}) } });
                return;
            }
            try
            {
                RoomManager.Instance.CreateRoom(m_RoomName.text, m_MapChooserBehaviour.GetChoosedId());
            }
            catch (InGameException e)
            {
                ModalManager.Show("Error", e.Message, new ModalButton[] { new() { Text = "Close" } });
            }
            
            Close();
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
        m_MapChooserBehaviour.dropdown.gameObject.SetActive(true);

    }
}