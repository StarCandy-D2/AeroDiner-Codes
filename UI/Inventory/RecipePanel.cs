using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RecipePanel : MonoBehaviour
{
    [SerializeField] private Image menuIcon;
    [SerializeField] private TMP_Text menuNameText;
    [SerializeField] private TMP_Text menuDescriptionText;
    [SerializeField] private TMP_Text recipeDescriptionText;
    

    public void SetData(FoodData data)
    {
        if (data == null) return;

        menuIcon.sprite = data.foodIcon;
        menuNameText.text = data.displayName;
        menuDescriptionText.text = data.description.Replace("\\n", "\n");
        recipeDescriptionText.text = data.recipeDescription;
    }
}
