using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupUIBehaviour : MonoBehaviour
{
    public TMP_Text headerText;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
        SetHeaderText("Lobby");
    }

    public void Open()
    {
        Debug.Log("OPEN UI");
        gameObject.SetActive(true);
    }

    public void Close()
    {
        Debug.Log("CLOSE UI");
        gameObject.SetActive(false);
    }

    public void SetHeaderText(string name)
    {
        headerText.text = name;
    }
}
