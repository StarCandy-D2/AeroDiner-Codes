using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class RecipePanel_IngredientScrollView : MonoBehaviour
{
    [SerializeField] private RectTransform contentTransform;
    [SerializeField] private GameObject ingredientSlotPrefab;

    public void PopulateIngredients(FoodData menu)
    {
        // 기존 자식 삭제
        foreach (Transform child in contentTransform)
        {
            Destroy(child.gameObject);
        }

        string[] ingredients = menu.ingredients;
        foreach (var ingredient in ingredients)
        {
            var go = Instantiate(ingredientSlotPrefab, contentTransform);
            var slot = go.GetComponent<RecipePanel_IngredientScrollView_Content>();
            slot.SetData(RecipeManager.Instance.FindFoodDataById(ingredient));
        }
    }
}
