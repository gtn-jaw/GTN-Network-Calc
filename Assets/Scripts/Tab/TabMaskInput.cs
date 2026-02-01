using System;
using System.Linq;
using TMPro;
using UnityEngine;

public class TabMaskInput : TabInput
{
    [SerializeField]
    TMP_Text FieldName;

    [SerializeField]
    TMP_InputField[] MaskBytes;

    [SerializeField]
    TMP_InputField SlashNotation;

    [SerializeField]
    Color ValidColor = Color.white;

    [SerializeField]
    Color InvalidColor = Color.red;

    [SerializeField]
    bool[] validityOfBytes = new bool[4];

    public void Init(string fieldName, Mask defaultMask)
    {
        InitTab(TabInputType.Mask, fieldName);

        FieldName.text = fieldName;

        // 4 bytes
        for (int i = 0; i < 4; i++)
        {
            MaskBytes[i].SetTextWithoutNotify(defaultMask.GetMaskAsBytes()[i].ToString());
            int index = i; // Capture index for the listener
            MaskBytes[i].onValueChanged.AddListener((value) => OnMaskByteChanged(index, value));
        }

        // Slash notation
        SlashNotation.SetTextWithoutNotify(defaultMask.GetMaskBitsCount().ToString());
        SlashNotation.onValueChanged.AddListener((value) => OnMaskSlashChanged(value));

        _mask = defaultMask;

        validityOfBytes = validityOfBytes.Select(v => true).ToArray();
        _isFieldValid = true;
    }

    public void SetValue(Mask newMask)
    {
        for (int i = 0; i < 4; i++)
        {
            MaskBytes[i].SetTextWithoutNotify(newMask.GetMaskAsBytes()[i].ToString());
        }

        SlashNotation.SetTextWithoutNotify(newMask.GetMaskBitsCount().ToString());

        _mask = newMask;
        _isFieldValid = true;
    }

    private void OnMaskByteChanged(int index, string value)
    {
        byte[] newMaskBytes = new byte[4];
        _mask.SetMask(newMaskBytes);

        bool priviousBytesValid = true;

        for (int i = 0; i < 4; i++)
        {
            if (CheckIntegrity(i, priviousBytesValid))
            {
                newMaskBytes[i] = byte.Parse(MaskBytes[i].text);
                _mask.SetMask(newMaskBytes);
            }
            else priviousBytesValid = false;
        }

        UpdateVisuals();

        RaiseOnValueChanged();
    }

    private bool CheckIntegrity(int index, bool previousBytesValid)
    {
        if (!previousBytesValid)
        {
            MarkByteAsInvalid(index);
            return false;
        }

        if (byte.TryParse(MaskBytes[index].text, out byte byteSegment))
        {
            if (!NetManagement.ValidateMaskByte(byteSegment)) // Value must be valid mask byte not only from range 0-255
            {
                MarkByteAsInvalid(index);
                return false;
            }

            byte[] maskBytes = (byte[])_mask.GetMaskAsBytes().Clone();

            maskBytes[index] = byteSegment;
            Debug.Log($"Mask changed to {string.Join(".", maskBytes)} from {_mask}");

            if (!Mask.ValidateAndConvert(maskBytes).valid) // Check if the new mask is valid
            {
                MarkByteAsInvalid(index);
                return false;
            }

            MarkByteAsValid(index);
            return true;
        }
        else
        {
            MarkByteAsInvalid(index);
            return false;
        }
    }

    private void MarkByteAsInvalid(int index)
    {
        validityOfBytes[index] = false;
        _isFieldValid = validityOfBytes.All(v => v);
    }

    private void MarkByteAsValid(int index)
    {
        validityOfBytes[index] = true;
        _isFieldValid = validityOfBytes.All(v => v);
    }

    public void UpdateVisuals()
    {
        UpdateDottedNotation();
        UpdateSlashNotation();
    }

    private void OnMaskSlashChanged(string value)
    {
        if (int.TryParse(value, out int num))
        {
            if (num < 0 || num > 32)
            {
                validityOfBytes = validityOfBytes.Select(v => false).ToArray();
                _isFieldValid = false;
                SlashNotation.image.color = InvalidColor;
                RaiseOnValueChanged();
                return;
            }

            _mask.SetMask($"/{num}");
            validityOfBytes = validityOfBytes.Select(v => true).ToArray();
            _isFieldValid = true;
            UpdateDottedNotation();
            UpdateSlashNotation();
            RaiseOnValueChanged();
            return;
        }
        else
        {
            SlashNotation.image.color = InvalidColor;
            validityOfBytes = validityOfBytes.Select(v => false).ToArray();
            _isFieldValid = false;
            RaiseOnValueChanged();
            return;
        }
    }

    private void UpdateDottedNotation()
    {
        for (int i = 0; i < 4; i++)
        {
            MaskBytes[i].image.color = validityOfBytes[i] ? ValidColor : InvalidColor;
            if (validityOfBytes[i])
                MaskBytes[i].SetTextWithoutNotify(_mask.GetMaskAsBytes()[i].ToString());
        }
        Debug.Log("Dotted notation updated.");
    }

    private void UpdateSlashNotation()
    {
        if (_isFieldValid)
        {
            int bits = _mask.GetMaskBitsCount();
            SlashNotation.SetTextWithoutNotify(bits.ToString());
            SlashNotation.image.color = ValidColor;
        }
        else
        {
            SlashNotation.SetTextWithoutNotify("");
            SlashNotation.image.color = InvalidColor;
        }
        Debug.Log("Slash notation updated.");
    }
}
