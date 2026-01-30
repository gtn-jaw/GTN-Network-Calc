using TMPro;
using UnityEngine;

public class TabMaskInput : TabInput
{
    [SerializeField]
    TMP_Text FieldName;

    [SerializeField]
    TMP_InputField[] IPBytes;

    [SerializeField]
    TMP_InputField SlashNotation;

    [SerializeField]
    Color ValidColor = Color.white;

    [SerializeField]
    Color InvalidColor = Color.red;

    public void Init(string fieldName, Mask defaultMask)
    {
        InitTab(TabInputType.Mask, fieldName);

        FieldName.text = fieldName;

        // 4 bytes
        for (int i = 0; i < 4; i++)
        {
            IPBytes[i].text = defaultMask.GetMaskAsBytes()[i].ToString();
            int index = i; // Capture index for the listener
            IPBytes[i].onValueChanged.AddListener((value) => OnMaskByteChanged(index, value));
        }

        // Slash notation
        SlashNotation.text = defaultMask.GetMaskBitsCount().ToString();
        SlashNotation.onValueChanged.AddListener((value) => OnMaskSlashChanged(value));

        _mask = defaultMask;
    }

    public void SetValue(Mask newMask)
    {
        for (int i = 0; i < 4; i++)
        {
            IPBytes[i].text = newMask.GetMaskAsBytes()[i].ToString();
        }

        SlashNotation.text = newMask.GetMaskBitsCount().ToString();

        _mask = newMask;
    }

    private void OnMaskByteChanged(int index, string value)
    {
        if (byte.TryParse(value, out byte byteSegment))
        {
            if (!NetManagement.ValidateMaskByte(byteSegment)) // Value must be valid mask byte not only from range 0-255
            {
                IPBytes[index].image.color = InvalidColor;
                return;
            }

            byte[] maskBytes = _mask.GetMaskAsBytes();

            maskBytes[index] = byteSegment;
            _mask = new Mask(maskBytes);
            IPBytes[index].image.color = ValidColor;
            UpdateSlashNotation();
        }
        else
        {
            IPBytes[index].image.color = InvalidColor;
        }
        RaiseOnValueChanged();
        Debug.Log("Mask byte changed");
    }

    private void OnMaskSlashChanged(string value)
    {
        if (int.TryParse(value, out int num))
        {
            if (num < 0 || num > 32)
            {
                SlashNotation.image.color = InvalidColor;
                return;
            }

            _mask.SetMask($"/{num}");
            SlashNotation.image.color = ValidColor;
            UpdateDottedNotation();
        }
        else
        {
            SlashNotation.image.color = InvalidColor;
        }
        RaiseOnValueChanged();
        Debug.Log("Mask slash changed");
    }

    private void UpdateDottedNotation()
    {
        byte[] maskBytes = _mask.GetMaskAsBytes();
        for (int i = 0; i < 4; i++)
        {
            IPBytes[i].text = maskBytes[i].ToString();
        }
    }

    private void UpdateSlashNotation()
    {
        int bits = _mask.GetMaskBitsCount();
        SlashNotation.text = bits.ToString();
    }
}
