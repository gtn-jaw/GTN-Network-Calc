using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class IP
{
    [SerializeField]
    byte[] _ip;

    public override string ToString()
    {
        return string.Join(".", _ip);
    }

    public IP(byte[] ip)
    {
        if (!ValidateIP(ip))
            throw new System.Exception($"Invalid IP byte array. Value: {string.Join(".", ip)}");
        _ip = ip;
    }

    public IP(List<byte> ip)
    {
        if (!ValidateIPList(ip))
            throw new System.Exception($"Invalid IP byte list. Value: {string.Join(".", ip)}");
        _ip = ip.ToArray();
    }

    public IP(string ipString)
    {
        if (!ValidateIP(ipString))
            throw new System.Exception($"Invalid IP string. Value: {ipString}");
        string[] segments = ipString.Split('.');
        _ip = new byte[4];

        for (int i = 0; i < 4; i++)
        {
            _ip[i] = byte.Parse(segments[i]);
        }
    }

    public byte[] GetIPAsBytes()
    {
        return _ip;
    }

    public List<byte> GetIPAsList()
    {
        return new List<byte>(_ip);
    }

    public string GetIPAsString()
    {
        return string.Join(".", _ip);
    }

    public uint GetIPAsUInt32()
    {
        return NetManagement.ToUInt32(this);
    }

    public void SetIP(byte[] ip)
    {
        if (!ValidateIP(ip))
            throw new System.Exception($"Invalid IP byte array. Value: {string.Join(".", ip)}");
        _ip = ip;
    }

    public void SetIP(List<byte> ipList)
    {
        if (!ValidateIPList(ipList))
            throw new System.Exception($"Invalid IP byte list. Value: {string.Join(".", ipList)}");
        _ip = ipList.ToArray();
    }

    public void SetIP(string ipString)
    {
        if (!ValidateIP(ipString))
            throw new System.Exception($"Invalid IP string. Value: {ipString}");
        string[] segments = ipString.Split('.');
        _ip = new byte[4];

        for (int i = 0; i < 4; i++)
        {
            _ip[i] = byte.Parse(segments[i]);
        }
    }

    public bool ValidateIPList(List<byte> ipList)
    {
        if (ipList == null || ipList.Count != 4)
            return false;

        foreach (var segment in ipList)
        {
            if (segment < 0 || segment > 255)
                return false;
        }

        return true;
    }

    public bool ValidateIP(byte[] ip)
    {
        if (ip == null || ip.Length != 4)
            return false;

        return true;
    }

    public bool ValidateIP(string ipString)
    {
        if (string.IsNullOrEmpty(ipString))
            return false;

        string[] segments = ipString.Split('.');

        if (ipString == null || segments.Length != 4)
            return false;

        foreach (var segment in segments)
        {
            if (!byte.TryParse(segment, out byte byteSegment))
                return false;
        }

        return true;
    }

    public bool ValidateIP(IP ip)
    {
        return ip.ValidateIP(ip.GetIPAsBytes());
    }

    public IP GetNextIP()
    {
        byte[] nextIP = new byte[4];
        _ip.CopyTo(nextIP, 0);

        for (int i = 3; i >= 0; i--)
        {
            if (nextIP[i] < 255)
            {
                nextIP[i]++;
                break;
            }
            else
            {
                nextIP[i] = 0;
            }
        }

        return new IP(nextIP);
    }

    public IP GetPreviousIP()
    {
        byte[] prevIP = new byte[4];
        _ip.CopyTo(prevIP, 0);

        for (int i = 3; i >= 0; i--)
        {
            if (prevIP[i] > 0)
            {
                prevIP[i]--;
                break;
            }
            else
            {
                prevIP[i] = 255;
            }
        }

        return new IP(prevIP);
    }
}
