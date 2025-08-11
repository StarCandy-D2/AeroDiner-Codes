using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

public class Intro : MonoBehaviour
{
    [SerializeField] private Image[] introImages;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private float[] displayTimes;
    
    private BGMEventType[][] bgmEventsPerImage; // 각 이미지마다 여러 BGM 이벤트
    private Coroutine fadeCoroutine;
    
    private void Awake()
    {
        displayTimes = new[] { 4f, 3f, 3f, 4f, 3f, 3f };
        bgmEventsPerImage = new BGMEventType[6][];
        bgmEventsPerImage[0] = new[] { BGMEventType.Intro1 };
        bgmEventsPerImage[1] = new[] { BGMEventType.Intro2 };
        bgmEventsPerImage[2] = new BGMEventType[] { };
        bgmEventsPerImage[3] = new BGMEventType[] { };
        bgmEventsPerImage[4] = new[] { BGMEventType.Intro3 };
        bgmEventsPerImage[5] = new BGMEventType[] {};
    }
    
    private void Start()
    {
        EventBus.RaiseFadeEvent(FadeEventType.FadeIn);
        foreach (var img in introImages)
        {
            var color = img.color;
            color.a = 0f;
            img.color = color;
            img.gameObject.SetActive(true);
        }
        fadeCoroutine = StartCoroutine(PlayFadeSequence());
        StartCoroutine(PlayFadeSequence());
    }

    private IEnumerator PlayFadeSequence()
    {
        for (int i = 0; i < introImages.Length; i++)
        {
            switch (i)
            {
                case 2:
                    EventBus.PlayLoopSFX(SFXType.Rain);
                    break;
                case 3:
                    EventBus.PlaySFX(SFXType.Sunder);
                    break;
                case 4:
                    EventBus.PlaySFX(SFXType.Crash1);
                    break;
                case 5:
                    EventBus.PlaySFX (SFXType.Crash2); ;
                    break;
            }

            Image img = introImages[i];

            // BGM 호출
            if (bgmEventsPerImage != null && i < bgmEventsPerImage.Length)
            {
                foreach (var bgmEvent in bgmEventsPerImage[i])
                    EventBus.PlayBGM(bgmEvent);
            }

            // 페이드 인
            yield return img.DOFade(1f, fadeDuration).SetEase(Ease.InOutQuad).WaitForCompletion();

            // 각 이미지에 설정된 표시 시간 (없으면 기본값)
            float waitTime = (displayTimes != null && i < displayTimes.Length) ? displayTimes[i] : 1.5f;
            yield return new WaitForSeconds(waitTime);

            // 페이드 아웃
            yield return img.DOFade(0f, fadeDuration).SetEase(Ease.InOutQuad).WaitForCompletion();
        }

        OnIntroFinished();
    }

    public void OnIntroFinished()
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);
        EventBus.PlayBGM(BGMEventType.StopBGM);
        EventBus.StopLoopSFX(SFXType.Rain);
        EventBus.Raise(UIEventType.LoadDayScene);
    }
}