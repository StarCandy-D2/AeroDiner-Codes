using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Store_StationPanel : MonoBehaviour
{
    [Header("UI 요소")]
    [SerializeField] private Image stationIconImage;
    [SerializeField] private TMP_Text stationNameText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text costText;
    [SerializeField] private Button buyButton;
    [SerializeField] private GameObject lockedOverlayPanel;
    [SerializeField] private TMP_Text unlockConditionText;

    private StoreItem currentItem;

    public void SetData(StoreItem item, Action onBuyClick, bool canBePurchased)
    {
        if (item == null)
        {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);
        currentItem = item;

        stationIconImage.sprite = item.Icon;
        stationNameText.text = item.DisplayName;
        costText.text = $"{item.Cost} G";

        if (canBePurchased)
        {
            descriptionText.text = item.Description.Replace("\\n", "\n");
        }

        buyButton.interactable = canBePurchased;
        SetLockedOverlayVisible(!canBePurchased);

        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(() =>
        {
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