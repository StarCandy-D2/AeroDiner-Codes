using UnityEngine;
public class Tu4 : MonoBehaviour
{

    public void OnOpenButtonClick()
    {
        EventBus.PlaySFX(SFXType.ButtonClick);
        UIEventCaller.CallUIEvent("tu2");
        EventBus.PlayBGM(BGMEventType.PlayRecipeChoice);
    }
    public void OnShopButtonClick()
    {
        UIEventCaller.CallUIEvent("tu5");
    }
}
