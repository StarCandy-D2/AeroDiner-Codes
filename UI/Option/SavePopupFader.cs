using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SavePopupFader : MonoBehaviour
{
    [SerializeField] private CanvasGroup popupGroup; // 페이드 제어용
    [SerializeField] private float showDuration = 1.5f;
    [SerializeField] private float fadeDuration = 1f;

    private Coroutine currentRoutine;

    public void ShowPopup(string message)
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(ShowAndFade(message));
    }

    private IEnumerator ShowAndFade(string message)
    {
        popupGroup.gameObject.SetActive(true);

        TMP_Text text = popupGroup.GetComponentInChildren<TMP_Text>();
        if (text != null) text.text = message;

        popupGroup.alpha = 1f;

        //타임스케일 무시하고 대기
        yield return new WaitForSecondsRealtime(showDuration);

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime; // 타임스케일 무시
            popupGroup.alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
            yield return null;
        }

        popupGroup.alpha = 0f;
        popupGroup.gameObject.SetActive(false);
        currentRoutine = null;
    }
}
