using UnityEngine;
using DG.Tweening;

public class IngredientWarnPopup : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeDuration = 0.3f;
    [SerializeField] private float visibleDuration = 1f;

    private void Reset()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void ShowPopup()
    {
        gameObject.SetActive(true);      
        canvasGroup.alpha = 0f;          

        Sequence seq = DOTween.Sequence();
        seq.Append(canvasGroup.DOFade(1f, fadeDuration))        
           .AppendInterval(visibleDuration)                      
           .Append(canvasGroup.DOFade(0f, fadeDuration))         
           .OnComplete(() => gameObject.SetActive(false));       
    }
}