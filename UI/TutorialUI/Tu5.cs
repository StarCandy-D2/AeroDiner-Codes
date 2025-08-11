using DG.Tweening;
using TMPro;
using UnityEngine;

public class Tu5 : MonoBehaviour
{
    [Header("")]
    [SerializeField] private TabController tabController;
    [SerializeField] private Store_RecipeScrollView recipeScrollView;
    [SerializeField] private Store_StationScrollView stationScrollView;

    [Header("")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI currentMoney;
    [SerializeField] private GameObject insufficientMoneyPanel;

    [Header("DOTween 설정")]
    [SerializeField] private float animateDuration = 0.5f;
    [SerializeField] private Color flashColor = Color.yellow;
    private int currentDisplayAmount;
    private Color originalColor;
    
    [Header("Debug Info")]
    [SerializeField] private bool showDebugInfo;

    [Header("Tutorial UI")]
    [SerializeField] private GameObject step1Panel1;
    [SerializeField] private GameObject step1Panel2;
    [SerializeField] private GameObject step1Pointer;

    [SerializeField] private GameObject step2Panel1;
    [SerializeField] private GameObject step2Pointer;
    [SerializeField] private GameObject step2Outline;
    [SerializeField] private GameObject stationBtnPanel;
    [SerializeField] private GameObject recipeBtnPanel;

    [SerializeField] private GameObject step3Panel1;
    [SerializeField] private GameObject step3Panel2;
    [SerializeField] private GameObject step3Pointer;

    [SerializeField] private GameObject step4Panel;
    [SerializeField] private GameObject step4Pointer;

    [SerializeField] private GameObject xPanel;

    private const int TUTORIAL_GOLD = -50;
    
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
        Tu5Step1();
    }

    #region 두트윈 메서드
    
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

    #region 버튼 메서드

    public void OnRecipeTabClick()
    {
        if (showDebugInfo)
            Debug.Log("버튼 클릭 됨");
        EventBus.PlaySFX(SFXType.ButtonClick);
        tabController.RequestSelectTab(0);
        // EventBus.Raise(UIEventType.ShowRecipeBook);
    }

    public void OnStationTabClick()
    {
        if (showDebugInfo)
            Debug.Log("버튼 클릭 됨");
        EventBus.PlaySFX(SFXType.ButtonClick);
        tabController.RequestSelectTab(1);
        // EventBus.Raise(UIEventType.ShowStationPanel);
    }

    #endregion

    #region Tutorial

    private void Tu5Step1()
    {
        step1Panel1.SetActive(true);
        step1Panel2.SetActive(true);
        step1Pointer.SetActive(true);
        stationBtnPanel.SetActive(true);
        recipeBtnPanel.SetActive(true);
    }

    public void Tu5Step2()
    {
        Tu5BuyRecipe();
        step1Panel1.SetActive(false);
        step1Panel2.SetActive(false);
        step1Pointer.SetActive(false);
        stationBtnPanel.SetActive(false);
        step2Panel1.SetActive(true);
        step2Pointer.SetActive(true);
        step2Outline.SetActive(true);
    }
    
    public void Tu5Step3()
    {
        step2Panel1.SetActive(false);
        step2Pointer.SetActive(false);
        step2Outline.SetActive(false);
        step3Panel1.SetActive(true);
        step3Panel2.SetActive(true);
        step3Pointer.SetActive(true);
    }
    
    public void Tu5Step4()
    {
        Tu5BuyStation();
        step3Panel1.SetActive(false);
        step3Panel2.SetActive(false);
        step3Pointer.SetActive(false);
        xPanel.SetActive(false);
        step4Panel.SetActive(true);
        step4Pointer.SetActive(true);
    }
    
    public void Tu5Step5()
    {
        UIEventCaller.CallUIEvent("tu6");
    }

    private void Tu5BuyRecipe()
    {
        GameManager.Instance.AddMoney(TUTORIAL_GOLD);
        MenuManager.Instance.UnlockMenu(StringID.TUTORIAL_RECIPE_ID);
        ForcePurchase(StringID.TUTORIAL_SHOP_ID);
        AnimateStoreMoney(GameManager.Instance.TotalEarnings);
        EventBus.Raise(UIEventType.UpdateTotalEarnings, GameManager.Instance.TotalEarnings);
    }

    private void Tu5BuyStation()
    {
        GameManager.Instance.AddMoney(TUTORIAL_GOLD);
        StationManager.Instance.UnlockStation(StringID.TUTORIAL_STATION_ID);
        StationManager.Instance.CreateStationInStorage(StringID.TUTORIAL_STATION_ID);
        AnimateStoreMoney(GameManager.Instance.TotalEarnings);
        EventBus.Raise(UIEventType.UpdateTotalEarnings, GameManager.Instance.TotalEarnings);
    }

    private void ForcePurchase(string id) => recipeScrollView.ForcePurchaseItem(id);

    #endregion
}