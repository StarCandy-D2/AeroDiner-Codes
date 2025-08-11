using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StationPanel : MonoBehaviour
{
    [SerializeField] private Image stationIconImage;
    [SerializeField] private TMP_Text stationNameText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text storedCountText;
    [SerializeField] private TMP_Text placedCountText;

    public void SetData(StationData data)
    {
        stationIconImage.sprite = data.stationIcon;
        stationNameText.text = data.displayName;
        descriptionText.text = data.description.Replace("\\n", "\n");

        storedCountText.text = StationManager.Instance.GetStationStoredCount(data.id).ToString();
        placedCountText.text = StationManager.Instance.GetStationPlacedCount(data.id).ToString();
    }
}
