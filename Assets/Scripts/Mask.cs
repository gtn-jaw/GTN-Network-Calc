using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Mask
{
    [SerializeField] byte[] _mask;

    public override string ToString()
    {
        return string.Join(".", _mask);
    }

    public Mask(byte[] mask)
    {
        if (!ValidateMask(mask))
            throw new System.Exception($"Invalid Mask byte array. Value: {string.Join(".", mask)}");
        _mask = mask;
    }

    public Mask(List<byte> mask)
    {
        if (!ValidateMask(mask))
            throw new System.Exception($"Invalid Mask byte list. Value: {string.Join(".", mask)}");
        _mask = mask.ToArray();
    }

    public Mask(string maskString)
    {
        if (!ValidateMask(maskString))
            throw new System.Exception($"Invalid Mask string. Value: {maskString}");

        bool slashNotation = maskString.Contains("/");
        if (slashNotation)
        {
            int bits = int.Parse(maskString.Replace("/", ""));
            _mask = new byte[4];
            for (int i = 0; i < 4; i++)
            {
                if (bits >= 8)
                {
                    _mask[i] = 255;
                    bits -= 8;
                }
                else
                {
                    _mask[i] = (byte)(256 - Mathf.Pow(2, 8 - bits));
                    bits = 0;
                }
            }
        }
        else
        {
            string[] segments = maskString.Split('.');
            _mask = new byte[4];

            for (int i = 0; i < 4; i++)
            {
                _mask[i] = byte.Parse(segments[i]);
            }
        }
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
        int bits = 0;
        for (int i = 0; i < 4; i++)
        {
            for (int j = 7; j >= 0; j--)
            {
                if ((_mask[i] & (1 << j)) != 0)
                    bits++;
            }
        }
        return bits;
    }

    public int GetNumberOfUsableHosts()
    {
        int bits = 32 - GetMaskBitsCount();
        if (bits <= 0)
            return 0;
        if (bits == 1)
            return 0;
        return (int)Mathf.Pow(2, bits) - 2;
    }

    public void SetMask(byte[] mask)
    {
        if (!ValidateMask(mask))
            throw new System.Exception($"Invalid Mask byte array. Value: {string.Join(".", mask)}");
        _mask = mask;
    }

    public void SetMask(List<byte> maskList)
    {
        if (!ValidateMask(maskList))
            throw new System.Exception($"Invalid Mask byte list. Value: {string.Join(".", maskList)}");
        _mask = maskList.ToArray();
    }

    public void SetMask(string maskString)
    {
        if (!ValidateMask(maskString))
            throw new System.Exception($"Invalid Mask string. Value: {maskString}");

        bool slashNotation = maskString.Contains("/");
        if (slashNotation)
        {
            int bits = int.Parse(maskString.Replace("/", ""));
            _mask = new byte[4];
            for (int i = 0; i < 4; i++)
            {
                if (bits >= 8)
                {
                    _mask[i] = 255;
                    bits -= 8;
                }
                else
                {
                    _mask[i] = (byte)(256 - Mathf.Pow(2, 8 - bits));
                    bits = 0;
                }
            }
        }
        else
        {
            string[] segments = maskString.Split('.');
            _mask = new byte[4];

            for (int i = 0; i < 4; i++)
            {
                _mask[i] = byte.Parse(segments[i]);
            }
        }
    }

    public bool ValidateMask(byte[] mask)
    {
        if (mask.Length != 4)
            return false;

        return true;
    }

    public bool ValidateMask(List<byte> maskList)
    {
        if (maskList.Count != 4)
            return false;

        foreach (var segment in maskList)
        {
            if (segment < 0 || segment > 255)
                return false;
        }

        return true;
    }

    public bool ValidateMask(string maskString)
    {
        if (string.IsNullOrEmpty(maskString))
            return false;

        bool slashNotation = maskString.Contains("/");
        if (slashNotation)
        {
            string bitsString = maskString.Replace("/", "");
            if (!int.TryParse(bitsString, out int bits))
                return false;

            if (bits < 0 || bits > 32)
                return false;
        }
        else
        {
            string[] segments = maskString.Split('.');

            if (segments.Length != 4)
                return false;

            foreach (var segment in segments)
            {
                if (!byte.TryParse(segment, out byte byteSegment))
                    return false;
            }
        }

        return true;
    }
}
