using DG.Tweening;
using UnityEngine;
public class Tu1 : MonoBehaviour
{
    [SerializeField] private GameObject targetPanel;
    [SerializeField] private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup.alpha = 0f;
        targetPanel.SetActive(false);
    }
    private void OnEnable()
    {
        FadeInPanel();
    }
    public void OnOpenButtonClick()
    {
        EventBus.PlaySFX(SFXType.ButtonClick);
        UIEventCaller.CallUIEvent("tu2");
        EventBus.PlayBGM(BGMEventType.PlayRecipeChoice);
    }
    public void OnShopButtonClick()
    {
        EventBus.Raise(UIEventType.FadeInStore);
    }
    public void FadeInPanel(float duration = 0.5f)
    {
        targetPanel.SetActive(true);
        canvasGroup.alpha = 0f;
        canvasGroup.DOFade(1f, duration)
            .SetEase(Ease.OutQuad)
            .SetUpdate(true)
            .OnStart(() =>
            {
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            });
    }
}
