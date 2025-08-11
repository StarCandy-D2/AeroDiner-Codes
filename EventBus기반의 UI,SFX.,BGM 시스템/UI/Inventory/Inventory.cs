using DG.Tweening;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [Header("참조")]
    [SerializeField] public TabController tabController;
    [SerializeField] private CanvasGroup canvasGroup;
    
    [Header("Debug Info")]
    [SerializeField] private bool showDebugInfo;
    
    private void Awake()
    {
        if (tabController == null)
            tabController = GetComponentInChildren<TabController>();
    }
    
    #region 두트윈 메서드
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
        canvasGroup.DOFade(0f, 0.2f).SetEase(Ease.InQuad).SetUpdate(true).OnComplete(() =>
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            gameObject.SetActive(false);
        });
    }
    #endregion
    
    #region 버튼 메서드
    
    public void OnIngredientTabClick()
    {
        if (showDebugInfo)
            Debug.Log("버튼 클릭 됨");
        EventBus.PlaySFX(SFXType.ButtonClick);
        tabController.RequestSelectTab(0);
       // EventBus.Raise(UIEventType.ShowInventory);
    }
    public void OnRecipeTabClick()
    {
        if (showDebugInfo)
            Debug.Log("버튼 클릭 됨");
        EventBus.PlaySFX(SFXType.ButtonClick);
        tabController.RequestSelectTab(2);
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
    public void OnQuestTabClick()
    {
        if (showDebugInfo)
            Debug.Log("버튼 클릭 됨");
        EventBus.PlaySFX(SFXType.ButtonClick);
        tabController.RequestSelectTab(3);
      //  EventBus.Raise(UIEventType.ShowQuestPanel);
    }
    public void OnCloseButtonClick()
    {
        EventBus.PlaySFX(SFXType.ButtonClick);
        EventBus.Raise(UIEventType.HideInventory);
    }
    
    #endregion
}
