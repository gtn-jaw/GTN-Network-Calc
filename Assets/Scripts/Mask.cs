using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Mask
{
    [SerializeField]
    byte[] _mask;

    public override string ToString()
    {
        return string.Join(".", _mask);
    }

    public Mask(byte[] mask)
    {
        (bool valid, byte[] convertedMask) = ValidateAndConvert(mask);
        if (!valid)
            throw new System.Exception($"Invalid Mask byte array. Value: {string.Join(".", mask)}");
        _mask = convertedMask;
    }

    public Mask(List<byte> maskList)
    {
        (bool valid, byte[] convertedMask) = ValidateAndConvert(maskList);
        if (!valid)
            throw new System.Exception(
                $"Invalid Mask byte list. Value: {string.Join(".", maskList)}"
            );
        _mask = convertedMask;
    }

    public Mask(string maskString)
    {
        (bool valid, byte[] convertedMask) = ValidateAndConvert(maskString);
        if (!valid)
            throw new System.Exception($"Invalid Mask string. Value: {maskString}");

        _mask = convertedMask;
    }

    public byte[] GetMaskAsBytes()
    {
        return _mask;
    }

    public List<byte> GetMaskAsList()
    {
        return new List<byte>(_mask);
    }

    public string GetMaskAsStringDottedNotation()
    {
        return string.Join(".", _mask);
    }

    public string GetMaskAsStringSlashNotation()
    {
        int bits = GetMaskBitsCount();
        return $"/{bits}";
    }

    public uint GetMaskAsUInt32()
    {
        return NetManagement.ToUInt32(this);
    }

    public int GetMaskBitsCount()
    {
        return ConvertByte4TableToBitsCount(_mask);
    }

    public static int ConvertByte4TableToBitsCount(byte[] bytes)
    {
        int bits = 0;
        for (int i = 0; i < 4; i++)
        {
            for (int j = 7; j >= 0; j--)
            {
                if ((bytes[i] & (1 << j)) != 0)
                    bits++;
            }
        }
        return bits;
    }

    public static byte[] ConvertBitsCountToByte4Table(int bits)
    {
        byte[] bytes = new byte[4];
        for (int i = 0; i < 4; i++)
        {
            if (bits >= 8)
            {
                bytes[i] = 255;
                bits -= 8;
            }
            else
            {
                bytes[i] = (byte)(256 - Mathf.Pow(2, 8 - bits));
                bits = 0;
            }
        }
        return bytes;
    }

    public int GetNumberOfUsableHosts()
    {
        int bits = 32 - GetMaskBitsCount();
        if (bits <= 1)
            return 0;
        return (int)Mathf.Pow(2, bits) - 2;
    }

    public void SetMask(byte[] mask)
    {
        (bool valid, byte[] convertedMask) = ValidateAndConvert(mask);
        if (!valid)
            throw new System.Exception($"Invalid Mask byte array. Value: {string.Join(".", mask)}");
        _mask = convertedMask;
    }

    public void SetMask(List<byte> maskList)
    {
        (bool valid, byte[] convertedMask) = ValidateAndConvert(maskList);
        if (!valid)
            throw new System.Exception(
                $"Invalid Mask byte list. Value: {string.Join(".", maskList)}"
            );
        _mask = convertedMask;
    }

    public void SetMask(string maskString)
    {
        (bool valid, byte[] convertedMask) = ValidateAndConvert(maskString);
        if (!valid)
            throw new System.Exception($"Invalid Mask string. Value: {maskString}");

        _mask = convertedMask;
    }

    public static (bool valid, byte[] convertedMask) ValidateAndConvert(byte[] mask)
    {
        if (mask.Length != 4)
            return (false, null);

        foreach (var segment in mask)
        {
            if (!NetManagement.ValidateMaskByte(segment))
                return (false, null);
        }

        int count = ConvertByte4TableToBitsCount(mask);
        if (!ValidateMask(count))
            return (false, null);

        if (!ValidateStructure(mask))
            return (false, null);

        return (true, mask);
    }

    public static (bool valid, byte[] convertedMask) ValidateAndConvert(List<byte> maskList)
    {
        if (maskList.Count != 4)
            return (false, null);

        foreach (var segment in maskList)
        {
            if (!NetManagement.ValidateMaskByte(segment))
                return (false, null);
        }

        int count = ConvertByte4TableToBitsCount(maskList.ToArray());
        if (!ValidateMask(count))
            return (false, null);

        if (!ValidateStructure(maskList.ToArray()))
            return (false, null);

        return (true, maskList.ToArray());
    }

    public static (bool valid, byte[] convertedMask) ValidateAndConvert(string maskString)
    {
        if (string.IsNullOrEmpty(maskString))
            return (false, null);

        bool slashNotation = maskString.Contains("/");
        if (slashNotation)
        {
            string bitsString = maskString.Replace("/", "");
            if (!byte.TryParse(bitsString, out byte bites))
                return (false, null);

            if (bites < 0 || bites > 32)
                return (false, null);

            byte[] mask = ConvertBitsCountToByte4Table(bites);

            if (!ValidateStructure(mask))
                return (false, null);

            return (true, mask);
        }
        else
        {
            string[] segments = maskString.Split('.');

            if (segments.Length != 4)
                return (false, null);

            foreach (var segment in segments)
            {
                if (!byte.TryParse(segment, out byte byteSegment))
                    return (false, null);

                if (!NetManagement.ValidateMaskByte(byteSegment))
                    return (false, null);
            }

            byte[] segmentsBytes = segments.Select(s => byte.Parse(s)).ToArray();

            int count = ConvertByte4TableToBitsCount(segmentsBytes);
            if (!ValidateMask(count))
                return (false, null);

            if (!ValidateStructure(segmentsBytes))
                return (false, null);

            return (true, segmentsBytes);
        }
    }

    public static bool ValidateMask(int bitsCount)
    {
        return bitsCount >= 0 && bitsCount <= 32;
    }

    public static bool ValidateStructure(byte[] maskBytes)
    {
        byte[] maskBytesCopy = (byte[])maskBytes.Clone();
        Array.Reverse(maskBytesCopy);
        uint val = BitConverter.ToUInt32(maskBytesCopy, 0);

        if (val == 0)
            return true;
        uint inv = ~val; // 11110000 -> 00001111
        return (inv & (inv + 1)) == 0; // 00001111 & 00010000 == 0
    }
}
