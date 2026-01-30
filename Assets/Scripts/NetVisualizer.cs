using System;
using System.Collections.Generic;
using UnityEngine;

public class NetVisualizer : MonoBehaviour
{
    static GameObject _networkVisualizerPrefab;
    static GameObject _netDataVPrefab;

    void Start()
    {
        Test();
    }

    [ContextMenu("Test Visualize Network")]
    public void Test()
    {
        NetworkData networkData = new NetworkData();
        NetHolder.SetNetworkData(networkData);

        Network network1 = new Network(
            "Network Root 1",
            new IP(new byte[] { 192, 168, 1, 0 }),
            new Mask("/24")
        );

        network1.AddTag(
            new Tag("Office", network1, network1.GetIP().GetNextIP(), network1.GetMask())
        );
        network1.AddTag(
            new Tag(
                "SecondFloor",
                network1,
                network1.GetIP().GetNextIP().GetNextIP(),
                network1.GetMask()
            )
        );

        networkData.AddNetworkBase(network1);

        Network network2 = new Network(
            "Network Root 2",
            new IP(new byte[] { 10, 5, 0, 0 }),
            new Mask("/24")
        );

        network2.AddTag(
            new Tag("FirstOffice", network2, new IP(new byte[] { 10, 5, 0, 1 }), network2.GetMask())
        );
        network2.AddTag(
            new Tag(
                "SecondOffice",
                network2,
                new IP(new byte[] { 10, 5, 0, 45 }),
                network2.GetMask()
            )
        );
        network2.AddTag(
            new Tag(
                "ThirdOffice",
                network2,
                new IP(new byte[] { 10, 5, 0, 78 }),
                network2.GetMask()
            )
        );
        network2.AddTag(
            new Tag(
                "FourthOffice",
                network2,
                new IP(new byte[] { 10, 5, 0, 130 }),
                network2.GetMask()
            )
        );

        networkData.AddNetworkBase(network2);

        network2.SubNet(new Mask("/25"))[0].SubNet(new Mask("/26"));

        networkData.SortAll();

        VisualizeNetwork();
    }

    public static void VisualizeNetwork()
    {
        if (_networkVisualizerPrefab == null)
        {
            _networkVisualizerPrefab = Resources.Load<GameObject>("Prefabs/NetworkVisualization");
            if (_networkVisualizerPrefab != null)
                Debug.Log("Loaded NetworkVisualization prefab.");
            else
                Debug.LogError("Failed to load NetworkVisualization prefab.");
        }

        if (_netDataVPrefab == null)
        {
            _netDataVPrefab = Resources.Load<GameObject>("Prefabs/NetDataV");
            if (_netDataVPrefab != null)
                Debug.Log("Loaded NetDataV prefab.");
            else
                Debug.LogError("Failed to load NetDataV prefab.");
        }

        NetworkData networkData = NetHolder.GetNetworkData();
        List<Network> networks = networkData.GetNetworkBases();

        foreach (Network network in networks)
        {
            VisualizeNetwork(network);
        }
    }

    static void VisualizeNetwork(Network network)
    {
        Mask mask = network.GetMask();
        IP net_ip = network.GetIP();
        IP broadcast_ip = network.GetBroadcastIP();
        IP first_usable_ip = net_ip.GetNextIP();
        IP last_usable_ip = broadcast_ip.GetPreviousIP();
        int hostsCount = mask.GetNumberOfUsableHosts();

        NetworkVisualization networkV = SpawnNetworkVisualizer(network);

        string indent = "";
        for (int i = 0; i < 0; i++)
        {
            indent += "|   ";
        }

        List<NetData> netDatas = new List<NetData>();

        void AddNDListToList(List<NetData> datas)
        {
            datas.ForEach(data =>
            {
                netDatas.Add(data);
            });
        }

        void AddNDToList(NetData data)
        {
            netDatas.Add(data);
        }

        void PushNetDataList()
        {
            foreach (NetData netData in netDatas)
            {
                SpawnNetDataV(networkV, netData);
            }
            netDatas.Clear();
        }

        AddNDListToList(
            new()
            {
                NetData.Create(networkV, indent + $"----------------------------------"),
                NetData.Create(networkV, "Name: " + network.GetName()),
                NetData.Create(
                    networkV,
                    $"Network IP: {net_ip}{mask.GetMaskAsStringSlashNotation()} ({mask.GetMaskAsStringDottedNotation()})"
                ),
                NetData.Create(networkV, first_usable_ip, mask, indent + "First Usable IP: "),
                NetData.Create(networkV, last_usable_ip, mask, indent + "Last Usable IP: "),
                NetData.Create(networkV, broadcast_ip, mask, indent + "Broadcast IP: "),
                NetData.Create(networkV, indent + "Number of Usable Hosts: " + hostsCount),
                NetData.Create(
                    networkV,
                    indent + (network.GetTags().Count > 0 ? "Tags:" : "No Tags")
                ),
            }
        );

        network
            .GetTags()
            .ForEach(tag =>
            {
                AddNDToList(
                    NetData.Create(networkV, tag.ip, tag.mask, indent + "Tag-- " + tag.Text)
                );
            });
        PushNetDataList();
    }

    static NetworkVisualization SpawnNetworkVisualizer(Network network)
    {
        GameObject networkVObj = GameObject.Instantiate(_networkVisualizerPrefab);
        NetworkVisualization networkV = networkVObj
            .GetComponent<NetworkVisualization>()
            .Init(network);
        networkVObj.name = "NetworkVisualizer_" + network.GetName();
        networkVObj.transform.parent = NetHolder.instance.rootVTransform;
        networkVObj.transform.SetAsLastSibling();
        networkVObj.transform.localScale = Vector3.one;

        return networkV;
    }

    static NetDataV SpawnNetDataV(NetworkVisualization parentNetworkV, NetData _netData)
    {
        GameObject netDataVObj = GameObject.Instantiate(_netDataVPrefab);
        NetDataV netDataV = netDataVObj.GetComponent<NetDataV>();
        netDataV.SetNetData(_netData);
        netDataVObj.name = "NetDataV_" + _netData.GetText();
        netDataVObj.transform.parent = parentNetworkV.transform;
        netDataVObj.transform.SetAsLastSibling();
        netDataVObj.transform.localScale = Vector3.one;

        netDataV.SetText();

        parentNetworkV.AddNetDataV(netDataV);

        return netDataV;
    }
}
