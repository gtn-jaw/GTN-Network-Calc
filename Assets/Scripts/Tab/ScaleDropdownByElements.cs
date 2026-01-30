using UnityEngine;

public class ScaleDropdownByElements : MonoBehaviour
{
    RectTransform dropdownTransform;
    [SerializeField] int elementMaxHeightCount = 5;
    [SerializeField] RectTransform sourceElementTransform;

    void Start()
    {
    }

    void Update()
    {
        try
        {
            dropdownTransform = transform.GetChild(3).GetComponent<RectTransform>();
            dropdownTransform.sizeDelta = new Vector2(
                dropdownTransform.sizeDelta.x,
                sourceElementTransform.sizeDelta.y *
                Mathf.Min(
                    elementMaxHeightCount,
                    dropdownTransform.GetChild(0).GetChild(0).childCount - 1 // Exclude template element (Item)
                )
            );
        }
        catch (System.Exception) { }
    }
}
