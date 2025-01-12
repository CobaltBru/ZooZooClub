using UnityEngine;
using UnityEngine.EventSystems;

public class TouchControl : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        RectTransform rectTransform = GetComponent<RectTransform>();

        // 클릭한 화면 좌표를 로컬 좌표로 변환
        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out localPoint))
        {
            Debug.Log($"Local Position: {(localPoint.x + 720)/8}, {(localPoint.y + 720) / 8}");
        }
    }
}
