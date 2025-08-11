using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Tu6 : MonoBehaviour
{
    [Header("Date Display")]
    [SerializeField] private TextMeshProUGUI monthText;
    [SerializeField] private TextMeshProUGUI dayText;

    private void Update()
    {
        UpdateDate();
    }

    private void UpdateDate()
    {
        GameManager.Instance.GetCurrentDate(out int month, out int day);
        monthText.text = $"{month}월";
        dayText.text = $"{day}일";

    }
    public void OnClickPause()
    {
        EventBus.PlaySFX(SFXType.ButtonClick);
        EventBus.Raise(UIEventType.OpenPause);
    }
    public void OnClickInventory()
    {
        EventBus.PlaySFX(SFXType.OpenInventory);
        EventBus.Raise(UIEventType.FadeInInventory);
    }

    public void OnClickRecipeBook()
    {
        EventBus.PlaySFX(SFXType.OpenBook);
        UIEventCaller.CallUIEvent("tu7");
    }
}