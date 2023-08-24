using TMPro;

public interface IChooserBehaviour
{
    void BuildDropdown();

    public void OnValueChanged(int value);

    public TMP_Dropdown GetDropdown();

    public int GetChoosedId();
}
