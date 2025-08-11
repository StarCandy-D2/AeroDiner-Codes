using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class IngredientPanel : MonoBehaviour
{
    [SerializeField] private Image foodIcon;
    [SerializeField] private Image foodTypeIcon;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private TMP_Text descriptionText;

    public void SetData(FoodData data, Sprite typeIcon)
    {
        if (data == null) return;

        foodIcon.sprite = data.foodIcon;
        nameText.text = data.displayName;
        priceText.text = data.foodCost.ToString();
        descriptionText.text = data.description.Replace("\\n", "\n");
        foodTypeIcon.sprite = typeIcon;
    }
}
