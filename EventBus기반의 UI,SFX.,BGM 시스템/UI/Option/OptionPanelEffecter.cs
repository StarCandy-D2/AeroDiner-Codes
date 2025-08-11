using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionPanelEffecter : MonoBehaviour
{
    [SerializeField] private RectTransform panelTransform;
    [SerializeField] private CanvasGroup canvasGroup;

    public void PlayFadeIn()
    {
        gameObject.SetActive(true);
        canvasGroup.alpha = 0f;

        DOTween.To(() => canvasGroup.alpha, x => canvasGroup.alpha = x, 1f, 0.3f)
               .SetUpdate(true);
    }

    public void PlayFadeOut()
    {
        DOTween.To(() => canvasGroup.alpha, x => canvasGroup.alpha = x, 0f, 0.3f)
               .SetUpdate(true)
               .OnComplete(() =>
               {
                   gameObject.SetActive(false);
               });
    }
}