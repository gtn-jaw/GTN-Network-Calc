using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NetVisualizer : MonoBehaviour
{
    static GameObject _networkVisualizerPrefab;
    static GameObject _netDataVPrefab;

    void Start()
    {
        NetworkData networkData = new NetworkData();
        NetHolder.SetNetworkData(networkData);
        Test();
    }

    [ContextMenu("Test Visualize Network")]
    public void Test()
    {
        NetworkData networkData = NetHolder.GetNetworkData();

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

    public static void DeleteLastVisualization()
    {
        foreach (Transform child in NetHolder.instance.rootVTransform)
        {
            Destroy(child.gameObject);
        }
    }

    public static void VisualizeNetwork()
    {
        DeleteLastVisualization();

        if (_networkVisualizerPrefab == null)
        {
            _networkVisualizerPrefab = Resources.Load<GameObject>("Prefabs/NetworkVisualization");
            if (_networkVisualizerPrefab == null)
                Debug.LogError("Failed to load NetworkVisualization prefab.");
        }

        if (_netDataVPrefab == null)
        {
            _netDataVPrefab = Resources.Load<GameObject>("Prefabs/NetDataV");
            if (_netDataVPrefab == null)
                Debug.LogError("Failed to load NetDataV prefab.");
        }

        NetworkData networkData = NetHolder.GetNetworkData();
        List<Network> networks = networkData.GetNetworkBases();

        for (int i = 0; i < networks.Count; i++)
        {
            VisualizeNetwork(networks[i], i > 0);
        }
    }

    static void VisualizeNetwork(Network network, bool mustAddLine)
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

        List<(NetData data, NetDataV.SubColorType color)> netDatas =
            new List<(NetData data, NetDataV.SubColorType color)>();

        void AddNDListToList(List<(NetData data, NetDataV.SubColorType color)> datas)
        {
            datas.ForEach(data =>
            {
                netDatas.Add(data);
            });
        }

        void AddNDToList((NetData data, NetDataV.SubColorType color) data)
        {
            netDatas.Add(data);
        }

        void PushNetDataList()
        {
            foreach ((NetData data, NetDataV.SubColorType color) netData in netDatas)
            {
                SpawnNetDataV(networkV, netData.data, netData.color);
            }
            netDatas.Clear();
        }

        if (mustAddLine)
            AddNDToList(
                (
                    NetData.Create(networkV, indent + $"----------------------------------"),
                    NetDataV.SubColorType.DEFAULT
                )
            );
        AddNDListToList(
            new()
            {
                (
                    NetData.Create(networkV, "Name: " + network.GetName()),
                    NetDataV.SubColorType.HEADER
                ),
                (
                    NetData.Create(
                        networkV,
                        $"Network IP: {net_ip}{mask.GetMaskAsStringSlashNotation()} ({mask.GetMaskAsStringDottedNotation()})"
                    ),
                    NetDataV.SubColorType.DEFAULT
                ),
                (
                    NetData.Create(networkV, first_usable_ip, mask, indent + "First Usable IP: "),
                    NetDataV.SubColorType.WARNING
                ),
                (
                    NetData.Create(networkV, last_usable_ip, mask, indent + "Last Usable IP: "),
                    NetDataV.SubColorType.DEFAULT
                ),
                (
                    NetData.Create(networkV, broadcast_ip, mask, indent + "Broadcast IP: "),
                    NetDataV.SubColorType.ERROR
                ),
                (
                    NetData.Create(networkV, indent + "Number of Usable Hosts: " + hostsCount),
                    NetDataV.SubColorType.DEFAULT
                ),
                (
                    NetData.Create(
                        networkV,
                        indent + (network.GetTags().Count > 0 ? "Tags:" : "No Tags")
                    ),
                    NetDataV.SubColorType.DEFAULT
                ),
            }
        );

        network
            .GetTags()
            .ForEach(tag =>
            {
                AddNDToList(
                    (
                        NetData.Create(networkV, tag.ip, tag.mask, indent + "+ Tag: " + tag.Text),
                        NetDataV.SubColorType.DEFAULT
                    )
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

    static NetDataV SpawnNetDataV(
        NetworkVisualization parentNetworkV,
        NetData _netData,
        NetDataV.SubColorType colorType = NetDataV.SubColorType.DEFAULT
    )
    {
        GameObject netDataVObj = GameObject.Instantiate(_netDataVPrefab);
        NetDataV netDataV = netDataVObj.GetComponent<NetDataV>();
        netDataV.SetNetData(_netData);
        netDataVObj.name = "NetDataV_" + _netData.GetText();
        netDataVObj.transform.parent = parentNetworkV.transform;
        netDataVObj.transform.SetAsLastSibling();
        netDataVObj.transform.localScale = Vector3.one;
        netDataV.ChangeColor(colorType);

        netDataV.SetText();

        parentNetworkV.AddNetDataV(netDataV);

        return netDataV;
    }
}
