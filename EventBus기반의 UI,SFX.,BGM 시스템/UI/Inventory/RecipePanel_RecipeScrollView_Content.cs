using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class RecipePanel_RecipeScrollView_Content : BaseScrollViewItem
{
    [SerializeField] private TMP_Text menuNameText;
    [SerializeField] private Button selectButton;
    [SerializeField] private Image resultIconImage;

    private FoodData menuData;

    public void Init(FoodData data, Action<FoodData> onClick)
    {
        menuData = data;

        menuNameText.text = data.displayName;

        if (resultIconImage != null)
            resultIconImage.sprite = data.foodIcon;  // 결과물 이미지로 설정

        selectButton.onClick.RemoveAllListeners();
        
        selectButton.onClick.AddListener(() => onClick?.Invoke(menuData));
    }
}
