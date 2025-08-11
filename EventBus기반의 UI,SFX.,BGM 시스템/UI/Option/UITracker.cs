using UnityEngine;

public class UITracker : MonoBehaviour
{
    public bool IsPauseOpen { get; private set; }
    public bool IsOptionOpen { get; private set; }
    public bool IsInventoryOpen { get; private set; }
    public bool IsStoreOpen { get; private set; }

    private void OnEnable()
    {
        EventBus.OnUIEvent += OnUIEvent;
    }

    private void OnDisable()
    {
        EventBus.OnUIEvent -= OnUIEvent;
    }

    private void OnUIEvent(UIEventType eventType, object payload)
    {
        switch (eventType)
        {
            case UIEventType.OpenPause: IsPauseOpen = true; break;
            case UIEventType.ClosePause: IsPauseOpen = false; break;

            case UIEventType.OpenOption: IsOptionOpen = true; break;
            case UIEventType.CloseOption: IsOptionOpen = false; break;

            case UIEventType.FadeInInventory: IsInventoryOpen = true; break;
            case UIEventType.FadeInRecipeBook: IsInventoryOpen = true; break;
            case UIEventType.HideInventory: IsInventoryOpen = false; break;

            case UIEventType.FadeInStore: IsStoreOpen = true; break;
            case UIEventType.FadeOutStore: IsStoreOpen = false; break;
        }
    }
}
