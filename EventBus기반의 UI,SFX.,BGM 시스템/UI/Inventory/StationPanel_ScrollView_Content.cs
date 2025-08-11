using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class StationPanel_ScrollView_Content : BaseScrollViewItem
{
    [SerializeField] private Image stationIconImage;
    [SerializeField] private TMP_Text stationNameText;
    [SerializeField] private Button selectButton;
    [SerializeField] private TMP_Text placedCountText;
    [SerializeField] private TMP_Text storedCountText;

    private StationData myData;
    public StationData GetStationData() => myData;
    
    public void Init(StationData data, Action<StationData> onClick)
    {
        myData = data;

        stationNameText.text = data.displayName;
        stationIconImage.sprite = data.stationIcon;

        int placed = StationManager.Instance.GetStationPlacedCount(data.id);
        int stored = StationManager.Instance.GetStationStoredCount(data.id);

        placedCountText.text = placed.ToString();
        storedCountText.text = stored.ToString();

        selectButton.onClick.RemoveAllListeners();
        selectButton.onClick.AddListener(() => onClick?.Invoke(myData));
    }
}
