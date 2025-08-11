using UnityEngine;
using System.Linq;

public class StationPanel_ScrollView : MonoBehaviour
{
    [SerializeField] private RectTransform contentTransform;
    [SerializeField] private StationPanel detailPanel;
    [SerializeField] private GameObject noItemPanel;
    [SerializeField] private GameObject stationSlotPrefab;
    
    private void OnEnable()
    {
        if (StationManager.Instance != null)
        {
            StationManager.Instance.CalculateStationCounts(); 
        }
        
        PopulateStationList();
    }

    private void PopulateStationList()
    {
        foreach (Transform child in contentTransform)
        {
            Destroy(child.gameObject);
        }
        
        var stations = StationManager.Instance.GetUnlockedStations()
            .OrderBy(s =>
            {
                string numPart = new string(s.stationData.id.Where(char.IsDigit).ToArray());
                return int.TryParse(numPart, out int n) ? n : int.MaxValue;
            })
            .Select(s => s.stationData)
            .ToList();
        
        // 아무 설비도 없음
        if (!stations.Any())
        {
            detailPanel.gameObject.SetActive(false);
            noItemPanel?.SetActive(true);
            return;
        }
        
        // 설비 있음
        detailPanel.gameObject.SetActive(true);
        noItemPanel?.SetActive(false);
        
        foreach (var station in stations)
        {
            var go = Instantiate(stationSlotPrefab, contentTransform);
            var slotUI = go.GetComponent<StationPanel_ScrollView_Content>();
            slotUI.Init(station, OnSlotSelected);
        }

        OnSlotSelected(stations.First());
    }
    
    private void OnSlotSelected(StationData data)
    {
        detailPanel.SetData(data);
        EventBus.PlaySFX(SFXType.ButtonClick);
    }
}
