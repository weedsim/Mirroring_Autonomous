using Gravitons.UI.Modal;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CompleteGameModal : Modal
{

    [SerializeField] protected CompleteGameBehaviour m_CompleteGameBehaviour;

    [Tooltip("Buttons in the modal")]
    [SerializeField] protected Button[] m_Buttons;


    public void Awake()
    {
        m_CompleteGameBehaviour.gameObject.SetActive(true);
    }

    public override void Show(ModalContentBase modalContent, ModalButton[] modalButton)
    {
        CompleteGameModalContent cgmc = (CompleteGameModalContent)modalContent;
        m_CompleteGameBehaviour.SetGameCompleteTime(cgmc.CompleteTime);

        for (int i = 0; i < modalButton.Length; i++)
        {
            if (i >= m_Buttons.Length)
            {
                Debug.LogError($"Maximum number of buttons of this modal is {m_Buttons.Length}. But {modalButton.Length} ModalButton was given. To display all buttons increase the size of the button array to at least {modalButton.Length}");
                return;
            }
            m_Buttons[i].gameObject.SetActive(true);
            m_Buttons[i].GetComponentInChildren<TMP_Text>().text = modalButton[i].Text;
            int index = i; //Closure
            m_Buttons[i].onClick.AddListener(() =>
            {
                if (modalButton[index].Callback != null)
                {
                    modalButton[index].Callback();
                }

                if (modalButton[index].CloseModalOnClick)
                {
                    Close();
                }
                m_Buttons[index].onClick.RemoveAllListeners();
            });
        }
    }
}

