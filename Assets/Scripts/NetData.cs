using UnityEngine;

public class NetData
{
    public enum EDataType
    {
        DESCRIPTION,
        IPDESCRIPTION,
        COUNT,
        NONE,
    }

    public string _text { get; private set; }
    public Tag _tag { get; private set; }
    public IP _IP { get; private set; }
    public Mask _mask { get; private set; }
    public NetworkVisualization _parentNetworkV { get; private set; }
    EDataType _dataType;

    public EDataType GetDataType()
    {
        return _dataType;
    }

    public static NetData Create(NetData netData)
    {
        return new NetData().Set(netData);
    }

    public static NetData Create(NetworkVisualization parentNetworkV, string text)
    {
        return new NetData().Set(parentNetworkV, text);
    }

    public static NetData Create(NetworkVisualization parentNetworkV, IP ip, Mask mask, string text)
    {
        return new NetData().Set(parentNetworkV, ip, mask, text);
    }

    public static NetData Create(NetworkVisualization parentNetworkV, Tag tag)
    {
        return new NetData().Set(parentNetworkV, tag);
    }

    public static NetData Create(NetworkVisualization parentNetworkV)
    {
        return new NetData().Set(parentNetworkV);
    }

    public static NetData Create(NetworkVisualization parentNetworkV, int count)
    {
        return new NetData().Set(parentNetworkV, count);
    }

    public NetData Set(NetData netData)
    {
        _dataType = netData.GetDataType();
        _text = netData._text;
        _IP = netData._IP;
        _mask = netData._mask;
        _tag = netData._tag;
        _parentNetworkV = netData._parentNetworkV;
        return this;
    }

    public NetData Set(NetworkVisualization parentNetworkV, string text)
    {
        _dataType = EDataType.DESCRIPTION;
        _parentNetworkV = parentNetworkV;
        _text = text;
        return this;
    }

    public NetData Set(NetworkVisualization parentNetworkV, IP ip, Mask mask, string text)
    {
        _dataType = EDataType.IPDESCRIPTION;
        _parentNetworkV = parentNetworkV;
        _IP = ip;
        _mask = mask;
        _tag = new Tag(text, parentNetworkV.GetNetwork(), ip, mask);
        _text = text;
        return this;
    }

    public NetData Set(NetworkVisualization parentNetworkV, Tag tag)
    {
        _dataType = EDataType.IPDESCRIPTION;
        _tag = tag;
        _parentNetworkV = parentNetworkV;
        _IP = tag.ip;
        _mask = tag.mask;
        _text = tag.Text;
        return this;
    }

    public NetData Set(NetworkVisualization parentNetworkV)
    {
        _dataType = EDataType.NONE;
        _parentNetworkV = parentNetworkV;
        return this;
    }

    public NetData Set(NetworkVisualization parentNetworkV, int count)
    {
        _dataType = EDataType.COUNT;
        _parentNetworkV = parentNetworkV;
        _text = count.ToString();
        return this;
    }

    public string GetText()
    {
        switch (_dataType)
        {
            case EDataType.DESCRIPTION:
                return _text;
            case EDataType.IPDESCRIPTION:
                return $"{_text} {_IP}{_mask.GetMaskAsStringSlashNotation()}";
            case EDataType.COUNT:
                return _text;
            case EDataType.NONE:
                return "";
            default:
                return "";
        }
    }
}
