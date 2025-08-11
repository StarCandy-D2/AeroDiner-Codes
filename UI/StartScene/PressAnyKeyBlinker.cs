using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class PressAnyKeyBlinker : MonoBehaviour
{
    [SerializeField] private Image pressAnyKeyImage;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float blinkInterval = 0.5f;
    [SerializeField] private float fastBlinkMultiplier = 0.5f; // 두 배 빠르게
    [SerializeField] private float fastBlinkDuration = 1f;
    [SerializeField] private GameObject credditBtn;
    private bool inputDetected = false;
    private Tween blinkTween;

    private void Start()
    {

        if (pressAnyKeyImage == null || canvasGroup == null)
        {
            return;
        }

        canvasGroup.alpha = 1f;
        StartBlink(blinkInterval);
    }


    private void Update()
    {
        if (inputDetected) return;

        if (Input.anyKeyDown)
        {
            inputDetected = true;
            blinkTween.Kill(); // 기존 깜빡임 중단
            StartFastBlink();  // 빠른 깜빡임 시작
        }
    }

    private void StartBlink(float interval)
    {
        // 0 ↔ 1 반복 루프
        blinkTween = canvasGroup.DOFade(0f, interval)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

    private void StartFastBlink()
    {
        EventBus.PlaySFX(SFXType.PressAnyKey);
        // 빠르게 깜빡임
        canvasGroup.alpha = 2f;

        blinkTween = canvasGroup.DOFade(0f, blinkInterval * fastBlinkMultiplier)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);

        // 1초 후 종료 및 전환
        DOVirtual.DelayedCall(fastBlinkDuration, () =>
        {
            blinkTween.Kill();
            canvasGroup.alpha = 0f;
            RaiseStartMenuEvent();
        });
    }

    private void RaiseStartMenuEvent()
    {
        credditBtn.gameObject.SetActive(true); 

        if (HasSavedGame())
            EventBus.Raise(UIEventType.ShowStartMenuWithSave);
        else
            EventBus.Raise(UIEventType.ShowStartMenuNoSave);
    }

    private bool HasSavedGame()
    {
        var data = SaveLoadManager.LoadGame();
        return data != null && data.currentDay > 1;
    }
}