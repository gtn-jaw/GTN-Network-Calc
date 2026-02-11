using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NetDataV : Checker
{
    [SerializeField]
    TMP_Text TMPText;

    [SerializeField]
    NetData _netData;

    [SerializeField]
    Image thisImage;

    [SerializeField]
    Color defaultColor = new Color(67, 67, 67, 1);

    public enum SubColorType
    {
        HEADER,
        ERROR,
        WARNING,
        DEFAULT,
    }

    [SerializeField]
    Color[] subColors;

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

    public NetDataV ChangeColor(SubColorType type)
    {
        switch (type)
        {
            case SubColorType.HEADER:
                thisImage.color = subColors[0];
                break;
            case SubColorType.ERROR:
                thisImage.color = subColors[1];
                break;
            case SubColorType.WARNING:
                thisImage.color = subColors[2];
                break;
            default:
                SetDefaultColor();
                break;
        }
        return this;
    }

    private void SetDefaultColor()
    {
        thisImage.color = defaultColor;
    }
}
