using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class Store : MonoBehaviour
{
    [Header("참조")]
    [SerializeField] protected TabController tabController;
    [SerializeField] private Store_RecipeScrollView recipeScrollView;
    [SerializeField] private Store_StationScrollView stationScrollView;
    
    [Header("UI")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI currentMoney;
    [SerializeField] private GameObject insufficientMoneyPanel;
    [SerializeField] private GameObject noPlacePanel;
    [SerializeField] private GameObject noContentPanel;

    [Header("DOTween 설정")]
    [SerializeField] private float animateDuration = 0.5f;
    [SerializeField] private Color flashColor = Color.yellow;
    
    [Header("Debug Info")]
    [SerializeField] private bool showDebugInfo;
    
    private int currentDisplayAmount;
    private Color originalColor;

    private void Awake()
    {
        if (tabController == null)
            tabController = GetComponentInChildren<TabController>();

        currentDisplayAmount = GameManager.Instance.TotalEarnings;
        currentMoney.text = $"{currentDisplayAmount:N0} G";
        originalColor = currentMoney.color;
    }
    
    private void OnEnable()
    {
        currentDisplayAmount = GameManager.Instance.TotalEarnings;
        currentMoney.text = $"{currentDisplayAmount:N0} G";
        EventBus.Raise(UIEventType.UpdateTotalEarnings, GameManager.Instance.TotalEarnings);
    }
    
    public void TryBuyItem(StoreItem item)
    {
        if (item == null) return;
        if (item.BaseData is FoodData && item.IsPurchased) return;

        if (GameManager.Instance.TotalEarnings >= item.Cost)
        {
            bool purchaseSucceeded = false;
            
            switch (item.BaseData)
            {
                case FoodData:
                    MenuManager.Instance.UnlockMenu(item.TargetID);
                    purchaseSucceeded = true;
                    break;
                
                case StationData:
                    if (StationManager.Instance.CreateStationInStorage(item.TargetID))
                    {
                        StationManager.Instance.UnlockStation(item.TargetID);
                        purchaseSucceeded = true;
                        if (showDebugInfo) Debug.Log($"구매 성공: {item.DisplayName}");
                    }
                    else
                    {
                        AnimateStoreMoney(GameManager.Instance.TotalEarnings);
                        EventBus.Raise(UIEventType.UpdateTotalEarnings, GameManager.Instance.TotalEarnings);
                        ShowNoPlacePanel();
                        if (showDebugInfo) Debug.LogWarning($"구매 실패: {item.DisplayName} - 보관 공간 부족");
                    }
                    break;
            }

            if (purchaseSucceeded)
            {
                GameManager.Instance.AddMoney(-item.Cost);
                AnimateStoreMoney(GameManager.Instance.TotalEarnings);
                EventBus.Raise(UIEventType.UpdateTotalEarnings, GameManager.Instance.TotalEarnings);
                
                if (showDebugInfo) Debug.Log($"구매 성공 처리 완료, UI 갱신 시작: {item.DisplayName}");
                
                switch (item.BaseData)
                {
                    case FoodData:
                        recipeScrollView.InitializeAndPopulate();
                        break;
                    case StationData:
                        stationScrollView.InitializeAndPopulate();
                        break;
                }
            }
        }
        else
        {
            ShowInsufficientMoneyPanel();
        }
    }
    
    #region 버튼 메서드
    
    public void OnIngredientTabClick()
    {
        if (showDebugInfo) Debug.Log("버튼 클릭 됨");
        EventBus.PlaySFX(SFXType.ButtonClick);
        tabController.RequestSelectTab(2);
        // EventBus.Raise(UIEventType.ShowInventory);
        // TODO: 아직 해금 안 됨 경고 팝업
    }
    
    public void OnRecipeTabClick()
    {
        if (showDebugInfo) Debug.Log("버튼 클릭 됨");
        EventBus.PlaySFX(SFXType.ButtonClick);
        recipeScrollView.InitializeAndPopulate();
        tabController.RequestSelectTab(0);
        // EventBus.Raise(UIEventType.ShowRecipeBook);
    }
    
    public void OnStationTabClick()
    {
        if (showDebugInfo)
            Debug.Log("버튼 클릭 됨");
        EventBus.PlaySFX(SFXType.ButtonClick);
        stationScrollView.InitializeAndPopulate();
        tabController.RequestSelectTab(1);
        // EventBus.Raise(UIEventType.ShowStationPanel);
    }
    
    public void OnCloseButtonClick()
    {
        EventBus.PlaySFX(SFXType.ButtonClick);
        EventBus.Raise(UIEventType.FadeOutStore);
    }
    
    #endregion
    
    #region 두트윈 메서드
    
    private void ShowInsufficientMoneyPanel()
    {
        var group = insufficientMoneyPanel.GetComponent<CanvasGroup>();
        if (group == null)
            group = insufficientMoneyPanel.AddComponent<CanvasGroup>();

        group.alpha = 0;
        insufficientMoneyPanel.SetActive(true);

        Sequence seq = DOTween.Sequence();
        seq.Append(group.DOFade(1, 0.5f))
            .AppendInterval(1.2f)
            .Append(group.DOFade(0, 0.5f))
            .OnComplete(() => insufficientMoneyPanel.SetActive(false));
    }
    
    private void ShowNoPlacePanel()
    {
        var group = noPlacePanel.GetComponent<CanvasGroup>();
        if (group == null)
            group = noPlacePanel.AddComponent<CanvasGroup>();

        group.alpha = 0;
        noPlacePanel.SetActive(true);

        Sequence seq = DOTween.Sequence();
        seq.Append(group.DOFade(1, 0.5f))
            .AppendInterval(1.2f)
            .Append(group.DOFade(0, 0.5f))
            .OnComplete(() => noPlacePanel.SetActive(false));
    }
    public void ShowNoContentPanel()
    {
        var group = noContentPanel.GetComponent<CanvasGroup>();
        if (group == null)
            group = noContentPanel.AddComponent<CanvasGroup>();

        group.alpha = 0;
        noContentPanel.SetActive(true);

        Sequence seq = DOTween.Sequence();
        seq.Append(group.DOFade(1, 0.5f))
            .AppendInterval(1.2f)
            .Append(group.DOFade(0, 0.5f))
            .OnComplete(() => noContentPanel.SetActive(false));
    }
    private void AnimateStoreMoney(int newAmount)
    {
        DOTween.Kill(currentMoney);
        DOTween.Kill(currentMoney.transform);

        int fromAmount = currentDisplayAmount;

        DOVirtual.Int(fromAmount, newAmount, animateDuration, value =>
        {
            currentDisplayAmount = value;
            currentMoney.text = $"{value:N0} G";
        }).SetEase(Ease.OutCubic);

        var seq = DOTween.Sequence();
        seq.Append(currentMoney.DOColor(flashColor, 0.2f));
        seq.Join(currentMoney.transform.DOScale(1.2f, 0.2f));
        seq.AppendInterval(0.1f);
        seq.Append(currentMoney.DOColor(originalColor, 0.2f));
        seq.Join(currentMoney.transform.DOScale(1.0f, 0.2f));
    }
    
    public void Show()
    {
        gameObject.SetActive(true);
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        canvasGroup.DOFade(1f, 0.3f).SetEase(Ease.OutQuad);
    }
    
    public void Hide()
    {
        canvasGroup.DOFade(0f, 0.2f).SetEase(Ease.InQuad).OnComplete(() =>
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            gameObject.SetActive(false);
        });
    }
    #endregion
}