using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StationLockedItemUI : MonoBehaviour
{
    [SerializeField] private TMP_Text stationNameText;
    [SerializeField] private Image stationIconImage;

    public void Init(StationData data)
    {
        stationNameText.text = "???";
        stationIconImage.sprite = data.stationIcon; // 또는 자물쇠 아이콘
    }
}