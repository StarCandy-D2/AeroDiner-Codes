using System.Collections.Generic;
using UnityEngine;

public class TutorialUIHandler : IUIEventHandler
{
    private readonly List<GameObject> sceneUIs;
    public TutorialUIHandler(List<GameObject> sceneUIs)
    {
        this.sceneUIs = sceneUIs;
    }
    public bool Handle(UIEventType type, object payload)
    {
        switch (type)
        {
            case UIEventType.tu1:
                foreach (var ui in sceneUIs)
                {

                    ui?.GetComponentInChildren<Tu1>(true)?.gameObject.SetActive(true);
                }
                return true;
            case UIEventType.tu2:
                foreach (var ui in sceneUIs)
                {
                    ui?.GetComponentInChildren<Tu1>(true)?.gameObject.SetActive(false);
                    ui?.GetComponentInChildren<Tu2>(true)?.gameObject.SetActive(true);
                }
                return true;
            case UIEventType.tu3:
                foreach (var ui in sceneUIs)
                {

                    ui?.GetComponentInChildren<Tu3>(true)?.gameObject.SetActive(true);
                }
                return true;
            case UIEventType.tu3_step2:
                foreach (var ui in sceneUIs)
                    ui?.GetComponentInChildren<Tu3>(true)?.ShowTutorialStep2();
                return true;
            case UIEventType.tu3_step3:
                foreach (var ui in sceneUIs)
                    ui?.GetComponentInChildren<Tu3>(true)?.ShowTutorialStep3();
                return true;
            case UIEventType.tu3_step4:
                foreach (var ui in sceneUIs)
                    ui?.GetComponentInChildren<Tu3>(true)?.ShowTutorialStep4();
                return true;
            case UIEventType.tu3_step5:
                foreach (var ui in sceneUIs)
                    ui?.GetComponentInChildren<Tu3>(true)?.ShowTutorialStep5();
                return true;
            case UIEventType.tu3_step6:
                foreach (var ui in sceneUIs)
                    ui?.GetComponentInChildren<Tu3>(true)?.ShowTutorialStep6();
                return true;
            case UIEventType.tu3_step7:
                foreach (var ui in sceneUIs)
                    ui?.GetComponentInChildren<Tu3>(true)?.ShowTutorialStep7();
                return true;
            case UIEventType.tu3_stop:
                foreach (var ui in sceneUIs)
                    ui?.GetComponentInChildren<Tu3>(true)?.gameObject.SetActive(false);
                return true;
            case UIEventType.tu4:
                foreach (var ui in sceneUIs)
                {

                    ui?.GetComponentInChildren<Tu4>(true)?.gameObject.SetActive(true);
                }
                return true;
            case UIEventType.tu5:
                foreach (var ui in sceneUIs)
                {
                    ui?.GetComponentInChildren<Tu4>(true)?.gameObject.SetActive(false);
                    ui?.GetComponentInChildren<Tu5>(true)?.gameObject.SetActive(true);
                }
                return true;
            case UIEventType.tu6:
                foreach (var ui in sceneUIs)
                {
                    ui?.GetComponentInChildren<Tu6>(true)?.gameObject.SetActive(true);
                }
                return true;
            case UIEventType.tu7:
                foreach (var ui in sceneUIs)
                {
                    ui?.GetComponentInChildren<Tu6>(true)?.gameObject.SetActive(false);
                    var inventory = ui?.GetComponentInChildren<Inventory>(true);
                    var tab = inventory?.GetComponentInChildren<TabController>(true);

                    inventory?.Show();
                    ui?.GetComponentInChildren<RecipePanel>(true)?.gameObject.SetActive(true);
                    ui?.GetComponentInChildren<StationPanel>(true)?.gameObject.SetActive(false);
                    ui?.GetComponentInChildren<IngredientPanel>(true)?.gameObject.SetActive(false);
                    ui?.GetComponentInChildren<QuestPanel>(true)?.gameObject.SetActive(false);
                    ui?.GetComponentInChildren<Tu7>(true)?.gameObject.SetActive(true);

                    tab?.RequestSelectTab(2);
                    tab?.ApplyTabSelectionVisuals();
                }
                return true;
            case UIEventType.tu8:
                foreach (var ui in sceneUIs)
                {
                    ui?.GetComponentInChildren<Tu7>(true)?.gameObject.SetActive(false);
                    ui?.GetComponentInChildren<Tu8>(true)?.gameObject.SetActive(true);
                }
                return true;
            case UIEventType.tu8_stop:
                foreach (var ui in sceneUIs)
                {
                    ui?.GetComponentInChildren<Tu8>(true)?.gameObject.SetActive(false);
                }
                return true;
            case UIEventType.tu9:
                foreach (var ui in sceneUIs)
                {
                    ui?.GetComponentInChildren<Tu8>(true)?.gameObject.SetActive(false);
                    var inventory = ui?.GetComponentInChildren<Inventory>(true);
                    var tab = inventory?.GetComponentInChildren<TabController>(true);

                    inventory?.Show();
                    ui?.GetComponentInChildren<RecipePanel>(true)?.gameObject.SetActive(false);
                    ui?.GetComponentInChildren<StationPanel>(true)?.gameObject.SetActive(false);
                    ui?.GetComponentInChildren<IngredientPanel>(true)?.gameObject.SetActive(false);
                    ui?.GetComponentInChildren<QuestPanel>(true)?.gameObject.SetActive(true);

                    tab?.RequestSelectTab(3);
                    tab?.ApplyTabSelectionVisuals();
                }
                return true;
        }
        
        return false;
    }

}
