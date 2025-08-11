using System.Collections.Generic;
using UnityEngine;

public class StartSceneUIHandler : IUIEventHandler
{
    private readonly List<GameObject> sceneUIs;
    public StartSceneUIHandler(List<GameObject> sceneUIs)
    {
        this.sceneUIs = sceneUIs;
    }
    public bool Handle(UIEventType type, object payload)
    {
        switch (type)
        {
            case UIEventType.ShowStartMenuWithSave:
                foreach (var ui in sceneUIs)
                    ui?.GetComponentInChildren<MenuPanel4>(true)?.gameObject.SetActive(true);
                break;
            case UIEventType.ShowStartMenuNoSave:
                foreach (var ui in sceneUIs)
                    ui?.GetComponentInChildren<MenuPanel3>(true)?.gameObject.SetActive(true);
                break;
            case UIEventType.OnClickNewGame:
                SaveLoadManager.ResetProgressOnly(); // 옵션빼고 저장 삭제.
                GameManager.Instance.ResetGameData();
                EventBus.Raise(UIEventType.LoadIntroScene);
                break;
            case UIEventType.LoadIntroScene:
                EventBus.RaiseFadeEvent(FadeEventType.FadeOutAndLoadScene, new FadeEventPayload(alpha: 1f, duration: 1f, scene: "IntroScene"));
                break;
        }
        return false;
    }
}
