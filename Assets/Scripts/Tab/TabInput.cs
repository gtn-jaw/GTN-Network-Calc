using System;
using UnityEngine;

[Serializable]
public class TabInput : MonoBehaviour
{
    public enum TabInputType
    {
        Dropdown,
        IP,
        Mask,
        Text,
    }

    protected TabInputType _inputType;

    public event Action<TabInput> OnValueChanged;

    public void DesubscribeAll()
    {
        OnValueChanged = null;
    }

    protected void RaiseOnValueChanged()
    {
        OnValueChanged?.Invoke(this);
    }

    protected void InitTab(TabInputType type, string fieldName)
    {
        _inputType = type;
        _fieldName = fieldName;
    }

    #region Protected Values

    protected string _fieldName;
    protected int _dropdownIndex;
    protected string[] _dropdownOptions;
    protected IP _ip;
    protected Mask _mask;
    protected string _text;

    #endregion

    public string GetFieldName()
    {
        return _fieldName;
    }

    public (int index, string[] options) GetDropdownValue()
    {
        return (_dropdownIndex, _dropdownOptions);
    }

    public IP GetIPValue()
    {
        return _ip;
    }

    public Mask GetMaskValue()
    {
        return _mask;
    }

    public string GetTextValue()
    {
        return _text;
    }

    public TabInputType GetInputType()
    {
        return _inputType;
    }

    public Type GetAssignedValueType()
    {
        switch (_inputType)
        {
            case TabInputType.Dropdown:
                return typeof((string[] options, int index));
            case TabInputType.IP:
                return typeof(IP);
            case TabInputType.Mask:
                return typeof(Mask);
            case TabInputType.Text:
                return typeof(string);
            default:
                return null;
        }
    }

    public Type GetMainValueType()
    {
        switch (_inputType)
        {
            case TabInputType.Dropdown:
                return typeof(string);
            case TabInputType.IP:
                return typeof(IP);
            case TabInputType.Mask:
                return typeof(Mask);
            case TabInputType.Text:
                return typeof(string);
            default:
                return null;
        }
    }

    public object GetAssignedValue()
    {
        switch (_inputType)
        {
            case TabInputType.Dropdown:
                return GetDropdownValue();
            case TabInputType.IP:
                return GetIPValue();
            case TabInputType.Mask:
                return GetMaskValue();
            case TabInputType.Text:
                return GetTextValue();
            default:
                return null;
        }
    }

    public object GetCurrentValue()
    {
        switch (_inputType)
        {
            case TabInputType.Dropdown:
                return _dropdownOptions[_dropdownIndex];
            case TabInputType.IP:
                return GetIPValue();
            case TabInputType.Mask:
                return GetMaskValue();
            case TabInputType.Text:
                return GetTextValue();
            default:
                return null;
        }
    }

    public void SetValue(object value)
    {
        if (value.GetType() != GetAssignedValueType())
        {
            Debug.LogError("Value type does not match TabInput type.");
            return;
        }

        switch (_inputType)
        {
            case TabInputType.Dropdown:
                (string[] options, int index) dropdownValue = ((string[] options, int index))value;
                GetComponent<TabDropdownInput>()
                    .SetValues(dropdownValue.options, dropdownValue.index);
                break;
            case TabInputType.IP:
                GetComponent<TabIPInput>().SetValue((IP)value);
                break;
            case TabInputType.Mask:
                GetComponent<TabMaskInput>().SetValue((Mask)value);
                break;
            case TabInputType.Text:
                GetComponent<TabTextInput>().SetValue((string)value);
                break;
        }
    }
}
