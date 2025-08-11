using UnityEngine;

public class DaySceneBtn : MonoBehaviour
{
    public void OnOpenButtonClick()
    {
        EventBus.PlaySFX(SFXType.ButtonClick);
        EventBus.Raise(UIEventType.ShowMenuPanel);
        GameManager.Instance.ChangePhase(GamePhase.SelectMenu);
        EventBus.PlayBGM(BGMEventType.PlayRecipeChoice);
    }
    public void OnShopButtonClick()
    {
        EventBus.PlaySFX(SFXType.ButtonClick);
        EventBus.Raise(UIEventType.FadeInStore);
    }
}
