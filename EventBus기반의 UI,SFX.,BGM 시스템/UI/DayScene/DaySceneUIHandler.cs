using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DaySceneUIHandler : IUIEventHandler
{
    private readonly List<GameObject> sceneUIs;

    public DaySceneUIHandler(List<GameObject> sceneUIs)
    {
        this.sceneUIs = sceneUIs;
    }
    public bool Handle(UIEventType type, object payload)
    {
        switch (type)
        {
            case UIEventType.ShowMenuPanel:
                foreach (var ui in sceneUIs)
                    ui?.GetComponentInChildren<MenuPanel>(true)?.gameObject.SetActive(true);

                return true;

            case UIEventType.UpdateMenuPanel:
                foreach (var ui in sceneUIs)
                    ui?.GetComponentInChildren<MenuPanel>(true)?.GenerateFoodList();
                return true;

            case UIEventType.HideMenuPanel:
                foreach (var ui in sceneUIs)
                    ui?.GetComponentInChildren<MenuPanel>(true)?.gameObject.SetActive(false);
                return true;

            case UIEventType.FadeInStore:
                foreach (var ui in sceneUIs)
                {
                    var store = ui?.GetComponentInChildren<Store>(true);
                    var tab = store?.GetComponentInChildren<TabController>(true);

                    store?.Show();
                    ui?.GetComponentInChildren<IngredientPanel>(true)?.gameObject.SetActive(false);
                    ui?.GetComponentInChildren<RecipePanel>(true)?.gameObject.SetActive(true);
                    ui?.GetComponentInChildren<StationPanel>(true)?.gameObject.SetActive(false);

                    tab?.RequestSelectTab(0);
                    tab?.ApplyTabSelectionVisuals();
                }
                return true;
            case UIEventType.FadeOutStore:
                foreach (var ui in sceneUIs)
                {
                    var store = ui?.GetComponentInChildren<Store>(true);
                    var tab = store?.GetComponentInChildren<TabController>(true);

                    store?.Hide();
                    ui?.GetComponentInChildren<IngredientPanel>(true)?.gameObject.SetActive(false);
                    ui?.GetComponentInChildren<RecipePanel>(true)?.gameObject.SetActive(true);
                    ui?.GetComponentInChildren<StationPanel>(true)?.gameObject.SetActive(false);

                    tab?.RequestSelectTab(0);
                    tab?.ApplyTabSelectionVisuals();
                }
                return true;
            case UIEventType.ShowDemoEnd:
                foreach (var ui in sceneUIs)
                    ui?.GetComponentInChildren<DemoEnd>(true)?.gameObject.SetActive(true);
                return true;
        }
        return false;
    }
}
