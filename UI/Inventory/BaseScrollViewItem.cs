using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// ScrollView의 각 콘텐츠 슬롯에서 공통으로 사용하는 아웃라인 하이라이트 처리
/// </summary>
public class BaseScrollViewItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Outline Highlight")]
    [SerializeField] protected Outline outline;

    protected virtual void Awake()
    {
        if (outline != null)
            outline.enabled = false; // 시작 시 비활성화
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (outline != null)
            outline.enabled = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (outline != null)
            outline.enabled = false;
    }
}