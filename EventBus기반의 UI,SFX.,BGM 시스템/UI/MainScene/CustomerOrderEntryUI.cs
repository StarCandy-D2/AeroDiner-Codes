using System;
using UnityEngine;
using UnityEngine.UI;

public class CustomerOrderEntryUI : MonoBehaviour
{
    [SerializeField] private Image foodIcon;
    [SerializeField] private Image patienceTimer;
    [SerializeField] private Sprite[] timerSprites; // green: 1084, yellow: 1087, red: 1055

    private void Reset()
    {
        foodIcon = transform.FindChild<Image>("Img_FoodIcon");
        patienceTimer = transform.FindChild<Image>("Img_PatienceTimer");
    }

    public void Init(Sprite orderSprite)
    {
        foodIcon.sprite = orderSprite;
    }

    public void UpdatePatienceColor(float ratio)
    {
        patienceTimer.fillAmount = ratio;
        int index = Util.ChangeIndexByRatio(ratio);
        patienceTimer.sprite = timerSprites[index];
    }
}