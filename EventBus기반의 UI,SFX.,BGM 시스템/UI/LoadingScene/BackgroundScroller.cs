using UnityEngine;
using DG.Tweening;

public class UIBackgroundScroller : MonoBehaviour
{
    public RectTransform target;
    public float scrollDistance = 1920f; // 왼쪽으로 이동할 거리
    public float scrollDuration = 5f;    // 이동 시간

    void Start()
    {
        // anchoredPosition.x를 기준으로 왼쪽으로 이동 → 원래 위치로 리셋 반복
        target.DOAnchorPosX(-scrollDistance, scrollDuration)
              .SetEase(Ease.Linear)
              .SetLoops(-1, LoopType.Restart);
    }
}
