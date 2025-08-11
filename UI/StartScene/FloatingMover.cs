using UnityEngine;
using DG.Tweening;

public class FloatingMover : MonoBehaviour
{
    public RectTransform target;        // 움직일 이미지
    public float moveDistanceX = 500f;  // 오른쪽 이동 거리
    public float moveDuration = 2f;     // 오른쪽으로 이동하는 데 걸리는 시간

    public float floatHeight = 20f;     // Y축 위아래 떠오르는 높이
    public float floatDuration = 1f;    // 위아래 떠다니는 속도

    private Vector2 startPos;
    private Tween floatTween;

    void Start()
    {
        startPos = target.anchoredPosition;

        StartFloating();
        StartHorizontalLoop();
    }

    void StartFloating()
    {
        floatTween = target.DOAnchorPosY(startPos.y + floatHeight, floatDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo); // 계속 위아래 반복
    }

    void StartHorizontalLoop()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(target.DOAnchorPosX(startPos.x + moveDistanceX, moveDuration).SetEase(Ease.Linear))
           .AppendCallback(() => {
               target.anchoredPosition = new Vector2(startPos.x, target.anchoredPosition.y);
           })
           .SetLoops(-1); // 무한 반복
    }

    void OnDestroy()
    {
        floatTween?.Kill();
    }
}