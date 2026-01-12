using UnityEngine;
using UnityEngine.EventSystems;

public abstract class Checker : MonoBehaviour, IPointerClickHandler
{
    protected NetDataV _netDataV;
    bool _isChecked = false;
    protected void InitChecker(NetDataV netDataV)
    {
        _netDataV = netDataV;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        NetData.EDataType dataType = _netDataV.GetNetData().GetDataType();
        // Replace _netDataV in NetHolder 
    }

    public bool IsChecked()
    {
        return _isChecked;
    }

    public void Check()
    {
        _isChecked = true;
    }

    public void UnCheck()
    {
        _isChecked = false;
    }
}
