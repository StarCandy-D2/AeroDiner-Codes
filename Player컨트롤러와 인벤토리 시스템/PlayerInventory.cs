using UnityEngine;

/// <summary>
/// 플레이어가 음식, 설비를 들고 내려놓는 기능을 담당
/// 게임의 현재 상태에 따라 동작이 달라짐
/// </summary>
public class PlayerInventory : MonoBehaviour
{
    public Transform GetItemSlotTransform() => itemSlotTransform;

    [Header("참조")]
    [SerializeField] private Transform itemSlotTransform;
    [SerializeField] private TilemapController tilemapController;

    [Header("디버그")]
    [SerializeField] private bool showDebugInfo;

    // 현재 들고 있는 아이템 정보
    public FoodDisplay HoldingFood { get; private set; }
    public IMovableStation HoldingStation { get; private set; }

    public bool IsHoldingItem => HoldingFood != null || HoldingStation != null;

    private void Start()
    {
        if (tilemapController == null)
        {
            tilemapController = FindObjectOfType<TilemapController>();
        }
    }

    /// <summary>
    /// 대상 아이템을 들려고 시도합니다.
    /// </summary>
    public void TryPickup(IInteractable target)
    {
        if (IsHoldingItem || target == null) return;

        // 설비 들기 (편집 모드)
        if (GameManager.Instance.CurrentPhase == GamePhase.EditStation && target is IMovableStation movable)
        {
            HandleStationPickup(movable);
            return;
        }

        // 재료/음식 들기 (영업 페이즈)
        if (GameManager.Instance.CurrentPhase == GamePhase.Operation || GameManager.Instance.CurrentPhase == GamePhase.Closing)
        {
            // 재료 상자에서는 재료를 생성해서 바로 듦
            if (target is IngredientStation station)
            {
                station.Interact(this, InteractionType.Pickup);
                return;
            }

            // 바닥에 놓인 음식/재료를 듦
            if (target is FoodDisplay food && food.CanPickup())
            {
                HandleFoodPickup(food);
            }
        }
    }

    /// <summary>
    /// 들고 있는 아이템을 대상에 내려놓으려 시도합니다.
    /// </summary>
    public void DropItem(IInteractable target)
    {
        if (!IsHoldingItem || target == null)
        {
            if (showDebugInfo) Debug.Log("[Inventory] 아이템이 없거나 타겟이 없습니다.");
            return;
        }

        // 설비 내려놓기 (편집 모드)
        if (HoldingStation != null && GameManager.Instance.CurrentPhase == GamePhase.EditStation)
        {
            HandleStationDrop(target);
            return;
        }

        // 재료/음식 내려놓기 (영업 페이즈)
        if (HoldingFood != null && (GameManager.Instance.CurrentPhase == GamePhase.Operation || GameManager.Instance.CurrentPhase == GamePhase.Closing))
        {
            HandleFoodDrop(target);
            return;
        }

        if (showDebugInfo)
            Debug.Log($"[Inventory] 현재 게임 상태({GameManager.Instance.CurrentPhase})에서는 아이템을 내려놓을 수 없습니다.");
    }

    /// <summary>
    /// 외부에서 플레이어가 들 아이템을 직접 설정
    /// </summary>
    public void SetHeldItem(FoodDisplay item)
    {
        HoldingFood = item;
        EventBus.Raise(GameEventType.PlayerPickedUpItem, HoldingFood.foodData);
    }

    // --- Private Helper Methods ---

    #region 아이템 들기/놓기 처리
    private void HandleStationPickup(IMovableStation station)
    {
        HoldingStation = station;

        Transform tr = station.GetTransform();
        tr.SetParent(itemSlotTransform);
        tr.localPosition = Vector3.zero;

        // 물리 및 충돌 비활성화
        var rb = tr.GetComponent<Rigidbody2D>();
        if (rb) rb.simulated = false;

        var col = tr.GetComponent<Collider2D>();
        if (col) col.enabled = false;

        tilemapController?.ShowAllCells();
        EventBus.Raise(GameEventType.PlayerPickedUpItem, HoldingStation.StationData);
    }

    private void HandleFoodPickup(FoodDisplay food)
    {
        HoldingFood = food;

        food.transform.SetParent(itemSlotTransform);
        food.transform.localPosition = Vector3.zero;

        // 물리 및 충돌 비활성화
        var rb = food.GetComponent<Rigidbody2D>();
        if (rb) rb.simulated = false;

        var col = food.GetComponent<Collider2D>();
        if (col) col.enabled = false;

        food.originPlace?.OnPlayerPickup();
        EventBus.Raise(GameEventType.PlayerPickedUpItem, HoldingFood.foodData);
    }

    private void HandleStationDrop(IInteractable target)
    {
        if (target is GridCellStatus gridCell)
        {
            bool success = PlacementManager.Instance.TryPlaceStationAt(gridCell.gameObject, HoldingStation);
            if (success)
            {
                HoldingStation = null; // 성공 시에만 손에서 놓음
                tilemapController?.HideAllCells();
            }
        }
    }

    private void HandleFoodDrop(IInteractable target)
    {
        bool placedSuccessfully = false;

        switch (target)
        {
            // 놓을 수 없음 처리
            case IPlaceableStation when target is BaseStation station && !station.CanPlaceIngredient(HoldingFood.foodData) 
                                        || target is Shelf shelf && !shelf.CanPlaceIngredient(HoldingFood.foodData) 
                                        || target is Table table && !table.CanPlaceIngredient(HoldingFood.foodData):
            {
                if (showDebugInfo) Debug.Log($"[Inventory] {target.GetType().Name}에 재료({HoldingFood.foodData.foodName})를 놓을 수 없습니다.");
                return;
            }
            
            // 내려놓기 실행
            case IPlaceableStation placeable:
                placeable.PlaceObject(HoldingFood.foodData);
                placedSuccessfully = true;
                break;
            
            // IPlaceableStation을 구현하지 않은 특별한 케이스 (쓰레기통 등)
            case Trashcan:
                placedSuccessfully = true;
                break;
            
            case IngredientStation ingredientStation:
                placedSuccessfully = ingredientStation.PlaceIngredient(HoldingFood.foodData);
                break;
        }

        // 성공적으로 내려놓았다면 손에 든 아이템을 파괴하고 인벤토리 비우기
        if (placedSuccessfully)
        {
            ConsumeHeldItem();
        }
    }

    /// <summary>
    /// 손에 들고 있는 아이템 파괴
    /// </summary>
    private void ConsumeHeldItem()
    {
        if (HoldingFood != null)
        {
            Destroy(HoldingFood.gameObject);
            HoldingFood = null;
        }
    }
    #endregion
}