using UnityEngine;

public static class UIEventCaller
{
    /// <summary>
    /// 문자열로 UIEventType을 찾아서 Raise 호출
    /// </summary>
    public static void CallUIEvent(string eventName, object payload = null)
    {
        if (string.IsNullOrEmpty(eventName))
        {
            //Debug.LogWarning("[UIEventCaller] 이벤트 이름이 비어있습니다.");
            return;
        }

        if (System.Enum.TryParse<UIEventType>(eventName, out var result))
        {
            EventBus.Raise(result, payload);
            //Debug.Log($"[UIEventCaller] 이벤트 호출됨: {result} / Payload: {payload}");
        }
        else
        {
            //Debug.LogError($"[UIEventCaller] 유효하지 않은 UIEventType 이름: {eventName}");
        }
    }
}
