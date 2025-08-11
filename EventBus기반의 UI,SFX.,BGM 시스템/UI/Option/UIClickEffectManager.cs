using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class UIClickEffectManager : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private RectTransform effectPrefab;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!IsPointerOverUI())
            {
                CreateEffect(Input.mousePosition);
                EventBus.OnSFXRequested(SFXType.BlankClick);
            }
            else
            {
               // Debug.Log("UI 위 클릭 → 무시");
            }
        }
    }

    private bool IsPointerOverUI()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        var results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        return results.Count > 0;
    }

    private void CreateEffect(Vector2 screenPos)
    {
        RectTransform effect = Instantiate(effectPrefab, canvas.transform);
        effect.gameObject.SetActive(true); // 혹시라도 비활성화되어 있을 경우 대비

        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            screenPos,
            null, // Overlay 모드일 땐 반드시 null
            out localPos
                );

        effect.anchoredPosition = localPos;
        effect.localScale = Vector3.zero;

        CanvasGroup group = effect.GetComponent<CanvasGroup>();
        if (group == null) group = effect.gameObject.AddComponent<CanvasGroup>();
        group.alpha = 1f;

        // DOTween 연출
        Sequence seq = DOTween.Sequence();

        // 축소 → 팡! 확산 → 사라짐
        effect.localScale = Vector3.one * 0.3f;
        group.alpha = 1f;
        Image img = effect.GetComponent<Image>();
        seq.Append(effect.DOScale(1.2f, 0.15f).SetEase(Ease.OutBack))
           .Join(group.DOFade(0f, 0.15f))
           .OnComplete(() => Destroy(effect.gameObject));
    }
}
