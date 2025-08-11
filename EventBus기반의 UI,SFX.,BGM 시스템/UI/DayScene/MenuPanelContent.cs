using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuPanelContent : BaseScrollViewItem
{
    public Image iconImage;
    public TMP_Text nameText;
    public TMP_Text priceText;
    public Toggle toggle;

    private Menu currentMenu;

    /// <summary>
    /// Menu 데이터로 UI 설정
    /// </summary>
    public void SetData(Menu menu)
    {
        currentMenu = menu;

        iconImage.sprite = menu.foodData.foodIcon;
        nameText.text = menu.foodData.displayName ?? menu.foodData.foodName;
        priceText.text = $"{menu.foodData.foodCost}G";

        toggle.onValueChanged.RemoveAllListeners();

        toggle.isOn = menu.isSelected;

        toggle.onValueChanged.AddListener((isOn) =>
        {
            OnToggle(isOn);
            var tu2 = FindObjectOfType<Tu2>();
            // 체크되었을 때 Tu2의 포인터 위치 이동
            if (isOn)
            {
                if (tu2 != null)
                    tu2.CheckMenuSelectionAndSwitchPointer(menu.foodData.id);
            }
            else
            {
                if (tu2 != null)
                    tu2.UncheckMenu();
            }
        });
    }

    public void OnToggle(bool isOn)
    {
        MenuManager.Instance.SetMenuSelection(currentMenu.foodData.id, isOn);
    }
}
