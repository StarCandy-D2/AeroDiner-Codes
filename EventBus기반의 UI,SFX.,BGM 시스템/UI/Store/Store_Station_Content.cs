using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Store_Station_Content : BaseScrollViewItem
{
    [SerializeField] private TMP_Text stationNameText;
    [SerializeField] private Image stationIcon;
    [SerializeField] private Button selectButton;
    [SerializeField] private GameObject lockIcon;

    private StoreItem currentItem;

    public void Init(StoreItem item, Action<StoreItem> onClick, bool conditionsMet)
    {
        currentItem = item;
        stationNameText.text = item.DisplayName;
        stationIcon.sprite = item.Icon;

        if (lockIcon != null)
        {
            lockIcon.SetActive(!conditionsMet);
        }

        selectButton.onClick.RemoveAllListeners();
        selectButton.onClick.AddListener(() => onClick?.Invoke(currentItem));
    }
}