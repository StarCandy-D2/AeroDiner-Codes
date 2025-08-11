using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RecipePanel_IngredientScrollView_Content : BaseScrollViewItem
{
    [SerializeField] private TMP_Text ingredientNameText;
    [SerializeField] private Image iconImage;

    public void SetData(FoodData ingredient)
    {
        ingredientNameText.text = ingredient.displayName;

        iconImage.sprite = ingredient.foodIcon;
    }
}
