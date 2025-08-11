using UnityEngine;
using TMPro;
using DG.Tweening;

public class WallPopupUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup panelGroup;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private float fadeDuration = 0.3f;

    private Tween currentTween;
    private bool isShowing = false;

    private void Awake()
    {
        if (panelGroup != null)
        {
            panelGroup.alpha = 0f;
            panelGroup.gameObject.SetActive(false);
        }
    }

    public void Show(string message)
    {
        if (!gameObject.activeInHierarchy || panelGroup == null) return;

        if (isShowing && messageText.text == message) return; // 같은 메시지일 때 무시

        isShowing = true;
        messageText.text = message;

        currentTween?.Kill();
        panelGroup.gameObject.SetActive(true);
        panelGroup.alpha = 0f;

        currentTween = panelGroup.DOFade(1f, fadeDuration);
    }

    public void Hide()
    {
        if (!isShowing || panelGroup == null) return;

        isShowing = false;
        currentTween?.Kill();

        currentTween = panelGroup.DOFade(0f, fadeDuration)
            .OnComplete(() =>
            {
                panelGroup.gameObject.SetActive(false);
            });
    }

    private void OnDisable()
    {
        currentTween?.Kill();
    }
}
