using System;
using System.Collections.Generic;

[Serializable]
public class Network
{
    [UnityEngine.SerializeField] string _name;
    [UnityEngine.SerializeField] IP _ip;
    [UnityEngine.SerializeField] Mask _mask;
    [UnityEngine.SerializeField] Network _parentNetwork;
    [UnityEngine.SerializeField] List<Tag> _tags = new List<Tag>();
    
    public Network(string name, IP ip, Mask mask)
    {
        this._name = name;
        this._ip = ip;
        this._mask = mask;
    }

    public void ChangeIP(IP newIP)
    {
        _ip = newIP;
    }

    public void ChangeMask(Mask newMask)
    {
        _mask = newMask;
    }

    public void ChangeName(string newName)
    {
        if (string.IsNullOrEmpty(newName))
            throw new System.Exception("Network name cannot be null or empty.");
        if (newName == "")
            throw new System.Exception("Network name cannot be an empty string.");

        _name = newName;
    }

    public void AddTag(Tag tag)
    {
        _tags.Add(tag);
    }

    public void AddTagRange(List<Tag> tags)
    {
        _tags.AddRange(tags);
    }

    public void RemoveTag(Tag tag)
    {
        _tags.Remove(tag);
    }

    public string GetName()
    {
        return _name;
    }

    public IP GetIP()
    {
        return _ip;
    }

    public Mask GetMask()
    {
        return _mask;
    }

    public Network GetParentNetwork()
    {
        return _parentNetwork;
    }

    public List<Tag> GetTags()
    {
        return _tags;
    }

    public IP GetBroadcastIP()
    {
        byte[] ipBytes = _ip.GetIPAsBytes();
        byte[] maskBytes = _mask.GetMaskAsBytes();
        byte[] broadcastBytes = new byte[4];

        for (int i = 0; i < 4; i++)
        {
            broadcastBytes[i] = (byte)(ipBytes[i] | (~maskBytes[i]));
        }

        return new IP(broadcastBytes);
    }

    public List<Network> SubNet(Mask newMask)
    {
        List<Network> subNets = NetManagement.SubnetNetwork(this, newMask);
        NetManagement.ApplyNetChanges(new() { this }, subNets);
        return subNets;
    }
}