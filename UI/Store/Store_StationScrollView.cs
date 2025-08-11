using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Store_StationScrollView : MonoBehaviour
{
    [Header("UI 연결")]
    [SerializeField] private RectTransform contentTransform;
    [SerializeField] private Store_StationPanel detailPanel;

    [Header("프리팹")]
    [SerializeField] private GameObject lockedSlotPrefab;
    [SerializeField] private GameObject unlockedSlotPrefab;

    [Header("참조")]
    [SerializeField] private Store store;

    private List<StoreItem> stationStoreItems = new();
    private StoreItem currentSelectedItem;

    private void Awake()
    {
        if (store == null) store = FindObjectOfType<Store>();
    }

    private void Start()
    {
        InitializeAndPopulate();
    }

    public void InitializeAndPopulate()
    {
        string previouslySelectedId = currentSelectedItem?.ID;
        stationStoreItems.Clear();

        var allStations = StationManager.Instance.StationDatabase;
        var storeDataMap = StoreDataManager.Instance.StoreItemMap;

        foreach (var station in allStations)
        {
            if (storeDataMap.TryGetValue(station.Key, out var csvData))
            {
                var storeItem = new StoreItem(station.Value.stationData, csvData);
                stationStoreItems.Add(storeItem);
            }
        }

        stationStoreItems = stationStoreItems.OrderBy(item => {
            string numPart = new string(item.ID.Where(char.IsDigit).ToArray());
            return int.TryParse(numPart, out int n) ? n : int.MaxValue;
        }).ToList();

        PopulateScrollView();
        
        // 구매 후 해당 항목 머무르기
        StoreItem itemToSelect = stationStoreItems.FirstOrDefault(item => item.ID == previouslySelectedId) 
                                 ?? stationStoreItems.FirstOrDefault();

        if (itemToSelect != null)
        {
            OnSlotSelected(itemToSelect, false);
        }
    }

    private void PopulateScrollView()
    {
        foreach (Transform child in contentTransform)
        {
            Destroy(child.gameObject);
        }

        foreach (var item in stationStoreItems)
        {
            bool conditionsMet = AreConditionsMet(item);
            GameObject prefabToUse = conditionsMet ? unlockedSlotPrefab : lockedSlotPrefab;

            var go = Instantiate(prefabToUse, contentTransform);
            var slotUI = go.GetComponent<Store_Station_Content>();
            slotUI.Init(item, selected => OnSlotSelected(selected, true), conditionsMet);
        }
    }

    private bool AreConditionsMet(StoreItem item)
    {
        if (item.CsvData.Type == UnlockType.None) return true;

        switch (item.CsvData.Type)
        {
            case UnlockType.Quest:
                // TODO: 퀘스트 완료 여부 확인
                return false;
            
            case UnlockType.Recipe:
                return item.CsvData.Conditions.Any(id => MenuManager.Instance.FindMenuById(id).isUnlocked);
        }
        return false;
    }

    private void OnSlotSelected(StoreItem item, bool playSFX = true)
    {
        if (playSFX)
            EventBus.PlaySFX(SFXType.ButtonClick);
        
        currentSelectedItem = item;
        bool canBePurchased = AreConditionsMet(item);
        detailPanel.SetData(item, () => store?.TryBuyItem(item), canBePurchased);
    }
}