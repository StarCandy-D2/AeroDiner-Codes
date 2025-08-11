using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainSceneUIHandler : IUIEventHandler
{
    private readonly List<GameObject> sceneUIs;

    public MainSceneUIHandler(List<GameObject> sceneUIs)
    {
        this.sceneUIs = sceneUIs;
    }

    public bool Handle(UIEventType type, object payload)
    {
        switch (type)
        {


            case UIEventType.UpdateTodayEarnings:
                foreach (var ui in sceneUIs)
                {
                    var resultPanel = ui != null ? ui.GetComponentInChildren<ResultPanel>(true) : null;
                    if (resultPanel != null && resultPanel.TotalRevenueText != null)
                    {
                        resultPanel.TotalRevenueText.text = ((int)payload).ToString("N0");
                    }
                }
                return true;



            case UIEventType.ShowResultPanel:
                foreach (var ui in sceneUIs)
                {
                    var resultPanel = ui?.GetComponentInChildren<ResultPanel>(true);
                    if (resultPanel != null)
                    {
                        resultPanel.gameObject.SetActive(true);
                        resultPanel.Init();
                    }
                }
                return true;

            case UIEventType.HideResultPanel:
                foreach (var ui in sceneUIs)
                    ui?.GetComponentInChildren<ResultPanel>(true)?.gameObject.SetActive(false);
                return true;

            case UIEventType.ShowInventory:
                foreach (var ui in sceneUIs)
                    ui?.GetComponentInChildren<Inventory>(true)?.gameObject.SetActive(true);
                return true;

            case UIEventType.HideInventory:
                foreach (var ui in sceneUIs)
                    ui?.GetComponentInChildren<Inventory>(true)?.gameObject.SetActive(false);
                return true;

            case UIEventType.ShowRoundTimer:
            case UIEventType.HideRoundTimer:
                bool show = type == UIEventType.ShowRoundTimer;
                foreach (var ui in sceneUIs)
                    ui?.GetComponentInChildren<RoundTimerUI>(true)?.gameObject.SetActive(show);
                return true;

            case UIEventType.ShowOrderPanel:
                if (payload is Customer customerToShow)
                {
                    foreach (var ui in sceneUIs)
                        ui?.GetComponentInChildren<CustomerOrderPanel>(true)?.ShowOrderPanel(customerToShow);
                }
                return true;

            case UIEventType.HideOrderPanel:
                if (payload is Customer customerToHide)
                {
                    foreach (var ui in sceneUIs)
                        ui?.GetComponentInChildren<CustomerOrderPanel>(true)?.HideOrderPanel(customerToHide);
                }
                return true;
            case UIEventType.ShowIngredientWarn:
                    foreach (var ui in sceneUIs)
                        ui?.GetComponentInChildren<IngredientWarnPopup>(true)?.ShowPopup();
                
                return true;
        }

        return false;
    }
}
