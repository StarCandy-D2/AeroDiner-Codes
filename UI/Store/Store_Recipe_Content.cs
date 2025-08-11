using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class Store_Recipe_Content : BaseScrollViewItem
{
    [SerializeField] private TMP_Text menuNameText;
    [SerializeField] private Button selectButton;
    [SerializeField] private Image resultIconImage;

    private StoreItem currentItem;

    public void Init(StoreItem item, Action<StoreItem> onClick)
    {
        currentItem = item;
        menuNameText.text = item.DisplayName;
        resultIconImage.sprite = item.Icon;

        selectButton.onClick.RemoveAllListeners();
        selectButton.onClick.AddListener(() => onClick?.Invoke(currentItem));
    }
}