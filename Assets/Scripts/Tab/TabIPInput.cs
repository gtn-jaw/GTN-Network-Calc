using TMPro;
using UnityEngine;

public class TabIPInput : TabInput
{
    [SerializeField]
    TMP_Text FieldName;

    [SerializeField]
    TMP_InputField[] IPBytes;

    [SerializeField]
    Color ValidColor = Color.white;

    [SerializeField]
    Color InvalidColor = Color.red;

    public void Init(string fieldName, IP defaultIP)
    {
        InitTab(TabInputType.IP, fieldName);

        FieldName.text = fieldName;

        for (int i = 0; i < 4; i++)
        {
            IPBytes[i].text = defaultIP.GetIPAsBytes()[i].ToString();
            int index = i; // Capture index for the listener
            IPBytes[i].onValueChanged.AddListener((value) => OnIPByteChanged(index, value));
        }

        _ip = defaultIP;
    }

    public void SetValue(IP newIP)
    {
        for (int i = 0; i < 4; i++)
        {
            IPBytes[i].text = newIP.GetIPAsBytes()[i].ToString();
        }

        _ip = newIP;
    }

    private void OnIPByteChanged(int index, string value)
    {
        if (NetManagement.ValidateByteAsString(value))
        {
            byte byteValue = byte.Parse(value);
            byte[] ipBytes = _ip.GetIPAsBytes();

            ipBytes[index] = byteValue;
            _ip = new IP(ipBytes);
            IPBytes[index].image.color = ValidColor;
        }
        else
        {
            IPBytes[index].image.color = InvalidColor;
        }
        RaiseOnValueChanged();
    }
}
