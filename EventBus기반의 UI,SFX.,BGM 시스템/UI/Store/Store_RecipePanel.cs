using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Store_RecipePanel : MonoBehaviour
{
    [SerializeField] private Image menuIcon;
    [SerializeField] private TMP_Text menuNameText;
    [SerializeField] private TMP_Text menuDescriptionText;
    [SerializeField] private TMP_Text saleText;
    [SerializeField] private Button buyButton;
    [SerializeField] private GameObject lockedOverlayPanel;
    [SerializeField] private TMP_Text unlockConditionText;
    
    private StoreItem currentItem;

    public void SetData(StoreItem item, Action onBuyClick, bool canBePurchased)
    {
        if (item == null) return;
        currentItem = item;

        menuIcon.sprite = item.Icon;
        menuNameText.text = item.DisplayName;
        saleText.text = $"{item.Cost} G";
        
        if (canBePurchased)
        {
            menuDescriptionText.text = item.Description.Replace("\\n", "\n");
        }

        buyButton.interactable = canBePurchased;
        SetLockedOverlayVisible(!canBePurchased);

        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(() => {
            EventBus.PlaySFX(SFXType.ButtonClick);
            onBuyClick?.Invoke();
        });
    }

    private void SetLockedOverlayVisible(bool isVisible)
    {
        if (lockedOverlayPanel == null) return;

        CanvasGroup group = lockedOverlayPanel.GetComponent<CanvasGroup>();
        if (group == null)
            lockedOverlayPanel.AddComponent<CanvasGroup>();

        if (isVisible)
        {
            unlockConditionText.text = StoreDataManager.Instance.GenerateUnlockDescription(currentItem.CsvData);
        
            lockedOverlayPanel.SetActive(true);
        }
        else
        {
            lockedOverlayPanel.SetActive(false);
        }
    }
}
