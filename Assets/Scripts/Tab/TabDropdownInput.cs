using TMPro;
using UnityEngine;

public class TabDropdownInput : TabInput
{
    [SerializeField] TMP_Text FieldName;
    [SerializeField] TMP_Dropdown Dropdown;

    public void Init(string fieldName, string[] options, int defaultIndex)
    {
        InitTab(TabInputType.Dropdown, fieldName);

        FieldName.text = fieldName;

        SetValues(options, defaultIndex);

        Dropdown.onValueChanged.AddListener(OnDropdownValueChanged);
    }

    public void SetValues(string[] options, int selectedIndex)
    {
        Dropdown.ClearOptions();
        Dropdown.AddOptions(new System.Collections.Generic.List<string>(options));
        Dropdown.value = selectedIndex;

        _dropdownOptions = options;
        _dropdownIndex = selectedIndex;
    }

    private void OnDropdownValueChanged(int newIndex)
    {
        _dropdownIndex = newIndex;
        RaiseOnValueChanged();
    }
}
