using UnityEngine;
using UnityEngine.SceneManagement;

public class UIInputHandler : MonoBehaviour
{
    public void HandleEscapeLikeAction()
    {
        EventBus.PlaySFX(SFXType.ButtonClick);
        var tracker = UIRoot.Instance.uiTracker;
        
        if (!tracker) return;

        string currentScene = SceneManager.GetActiveScene().name;
        bool isRestrictedScene = currentScene == StringScene.START_SCENE || currentScene == StringScene.LOADING_SCENE || currentScene == "IntroScene";

        if (tracker.IsOptionOpen)
        {
            bool keyChanged = UIRoot.Instance.keyRebindManager?.HasUnsavedChanges() ?? false;
            bool volumeChanged = UIRoot.Instance.volumeHandler?.HasUnsavedChanges() ?? false;
            bool videoChanged = UIRoot.Instance.videoSettingPanel?.HasUnsavedChanges() ?? false;

            if (keyChanged || volumeChanged || videoChanged)
            {
                UIRoot.Instance.uiExitPopup?.Show(); // 변경 사항 있음 → 팝업 노출
            }
            else
            {
                EventBus.Raise(UIEventType.CloseOption);
                EventBus.Raise(UIEventType.CloseSound);
                EventBus.Raise(UIEventType.CloseVideo);
                EventBus.Raise(UIEventType.CloseControl);
            }
        }
        else if (tracker.IsInventoryOpen)
        {
            EventBus.Raise(UIEventType.HideInventory);
        }
        else if (tracker.IsStoreOpen)
        {
            EventBus.Raise(UIEventType.FadeOutStore);
        }
        else if (tracker.IsPauseOpen)
        {
            EventBus.Raise(UIEventType.ClosePause);
        }
        else
        {
            if (!isRestrictedScene)
            {
                EventBus.Raise(UIEventType.OpenPause);
            }
            else
            {
                //Debug.Log($"{currentScene}에서는 PausePanel을 열지 않습니다.");
            }
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            EventBus.PlaySFX(SFXType.ButtonClick);
            //Debug.Log("ESC 눌렸음");
            HandleEscapeLikeAction();
        }
    }
}
