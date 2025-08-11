using UnityEngine;
using DG.Tweening;

public class FloatMotion : MonoBehaviour
{
    public float floatAmount = 0.2f;  // 위아래 이동량
    public float floatDuration = 1f;  // 한 방향 이동 시간 (왕복 시간 아님)
    public Ease easeType = Ease.InOutSine;

    private Tween floatTween;

    void Start()
    {
        Vector3 upPos = transform.localPosition + Vector3.up * floatAmount;
        Vector3 downPos = transform.localPosition - Vector3.up * floatAmount;

        // DOTween Sequence를 사용해서 무한 반복하는 부드러운 모션 생성
        floatTween = transform.DOLocalMove(upPos, floatDuration)
            .SetEase(easeType)
            .SetLoops(-1, LoopType.Yoyo);
    }

    void OnDisable()
    {
        floatTween?.Kill();
    }
}