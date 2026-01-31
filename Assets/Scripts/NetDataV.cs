using TMPro;
using UnityEngine;

public class NetDataV : Checker
{
    [SerializeField]
    TMP_Text TMPText;

    [SerializeField]
    NetData _netData;

    void Start()
    {
        InitChecker(this);
    }

    public void SetText()
    {
        TMPText.text = _netData.GetText();
    }

    public void SetNetData(NetData netData)
    {
        _netData = netData;
        SetText();
    }

    public NetData GetNetData()
    {
        return _netData;
    }
}
