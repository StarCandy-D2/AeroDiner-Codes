using System.Collections;
using UnityEngine;

public class Fader : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(FadeInAfterDelay());
    }

    private IEnumerator FadeInAfterDelay()
    {
        // 1초 대기
        yield return new WaitForSeconds(1.0f);

        // 기존 로직 실행
        EventBus.RaiseFadeEvent(FadeEventType.FadeIn, new FadeEventPayload(alpha: 0f, duration: 1f));
        GameManager.Instance.ChangePhase(GamePhase.Opening);
        EventBus.PlayBGM(BGMEventType.PlayMainTheme);
    }
}