using UnityEngine;

public class NetHolder : MonoBehaviour
{
    static NetDataV _checkedNetDataV;
    static NetworkData _networkData;

    [SerializeField]
    NetworkData _networkDataInspector;

#if UNITY_EDITOR
    void Update()
    {
        _networkDataInspector = _networkData;
    }
#endif

    public Transform rootVTransform;

    public static NetHolder instance = new NetHolder();

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public static void NewData()
    {
        _networkData = new NetworkData();
        _checkedNetDataV = null;
    }

    public static void SetNetworkData(NetworkData networkData)
    {
        _networkData = networkData;
    }

    public static NetworkData GetNetworkData()
    {
        return _networkData;
    }

    public static void SetCheckedNetDataV(NetDataV netDataV)
    {
        _checkedNetDataV.UnCheck();
        _checkedNetDataV = netDataV;
        _checkedNetDataV.Check();
    }

    public static NetDataV GetCheckedNetDataV()
    {
        return _checkedNetDataV;
    }
}
