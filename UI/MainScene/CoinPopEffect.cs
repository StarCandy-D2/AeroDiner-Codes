using UnityEngine;
using DG.Tweening;

public class CoinPopEffect : MonoBehaviour
{
    public float moveUpDistance = 2f;
    public float duration = 1f;
    public float spinAmount = 360f;

    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void Play()
    {
        gameObject.SetActive(true);
        Vector3 targetPos = transform.position + Vector3.up * moveUpDistance;
        Sequence seq = DOTween.Sequence();

        seq.Join(transform.DOMoveY(targetPos.y, duration).SetEase(Ease.OutCubic))
           .Join(transform.DORotate(new Vector3(0, spinAmount,0), duration, RotateMode.FastBeyond360).SetEase(Ease.OutQuad))
           .Join(sr.DOFade(0f, duration))
           .OnComplete(() => gameObject.SetActive(false));
    }
}