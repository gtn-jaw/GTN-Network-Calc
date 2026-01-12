using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class NetworkData
{
    [SerializeField] List<Network> _netBases = new List<Network>();

    public NetworkData()
    {
    }

    public List<Network> GetNetworkBases()
    {
        return _netBases;
    }

    public void AddNetworkBase(Network netBase)
    {
        if (!_netBases.Contains(netBase))
            _netBases.Add(netBase);
    }

    public void RemoveNetworkBase(Network netBase)
    {
        if (_netBases.Contains(netBase))
            _netBases.Remove(netBase);
    }

    public void SortAll()
    {
        if (_netBases.Count <= 1)
            return;
        _netBases = NetManagement.SortNetworks(_netBases);
    }
}
