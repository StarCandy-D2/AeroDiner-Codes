using UnityEngine.EventSystems;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IngredientPanel_ScrollView_Content : BaseScrollViewItem
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private Image typeIconImage;
    [SerializeField] private Button detailButton; // 버튼 참조

    private FoodData myData;
    [SerializeField] private FoodTypeIconSet iconSet;
    [SerializeField] private IngredientPanel detailPanel;
    public FoodData FoodInfo => myData;

    public void Init(FoodData data, FoodTypeIconSet iconSet, IngredientPanel panel)
    {
        myData = data;
        this.iconSet = iconSet;
        detailPanel = panel;


        // UI 표시
        if (iconImage != null)
            iconImage.sprite = data.foodIcon;
        else
            Debug.LogWarning("iconImage is not assigned!");

        nameText.text = data.displayName;


        if (typeIconImage != null && iconSet != null)
            typeIconImage.sprite = iconSet.GetIcon(data.foodType);

        if (detailButton != null)
        {
            detailButton.onClick.RemoveAllListeners();
            detailButton.onClick.AddListener(() =>
            {
                EventBus.PlaySFX(SFXType.ButtonClick);
                detailPanel.SetData(myData, iconSet.GetIcon(myData.foodType));
            });
        }
    }
}
