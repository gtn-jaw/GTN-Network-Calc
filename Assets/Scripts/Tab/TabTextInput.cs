using System;
using TMPro;
using UnityEngine;

public class TabTextInput : TabInput
{
    [SerializeField]
    TMP_Text FieldName;

    [SerializeField]
    TMP_InputField TextInputField;

    public void Init(string fieldName, string defaultText)
    {
        InitTab(TabInputType.Text, fieldName);

        FieldName.text = fieldName;
        TextInputField.text = defaultText;

        TextInputField.onValueChanged.AddListener((value) => OnTextChanged(value));

        _text = defaultText;

        _isFieldValid = true;
    }

    public void SetValue(string newText)
    {
        TextInputField.text = newText;
        _text = newText;
    }

    private void OnTextChanged(string value)
    {
        _text = value;
        RaiseOnValueChanged();
    }
}
