using UnityEngine;

public class Tu7 : MonoBehaviour
{
    public void OnCloseBtn()
    {
        EventBus.PlaySFX(SFXType.ButtonClick);
        EventBus.Raise(UIEventType.HideInventory);
        UIEventCaller.CallUIEvent("tu8");
    }
}
