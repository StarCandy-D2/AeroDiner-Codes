using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PausePanelEffecter : MonoBehaviour
{
    [SerializeField] private List<GameObject> pauseButtons;

    public void PlaySequentialIntro()
    {
        foreach (var btn in pauseButtons)
        {
            btn.SetActive(false); // 시작 시 모두 꺼두기
        }

        StartCoroutine(SequentialActivate());
    }

    private IEnumerator SequentialActivate()
    {
        foreach (var btn in pauseButtons)
        {
            yield return new WaitForSecondsRealtime(0.15f); // TimeScale=0에도 동작
            btn.SetActive(true);
            EventBus.PlaySFX(SFXType.OpenPause);
        }
    }
    public void HideWithPushEffect()
    {
        float duration = 0.5f;
        Sequence seq = DOTween.Sequence().SetUpdate(true);

        foreach (var btn in pauseButtons)
        {
            var rt = btn.GetComponent<RectTransform>();
            if (rt == null) continue;

            seq.Join(rt.DOAnchorPosY(rt.anchoredPosition.y + 500f, duration)
                      .SetEase(Ease.InBack));
        }
        EventBus.PlaySFX(SFXType.ClosePause);
        seq.OnComplete(() =>
        {
            foreach (var btn in pauseButtons)
            {
                btn.SetActive(false); // 버튼 숨김
            }
            gameObject.SetActive(false); // 패널 비활성화
        });
    }
}

