using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class ResultPanel : MonoBehaviour
{
    [Header("Sales")] 
    [SerializeField] private GameObject contentPrefab;
    [SerializeField] Transform contentTransform;
    [SerializeField] TextMeshProUGUI totalSalesVolume;
    [SerializeField] TextMeshProUGUI totalRevenue;
    public TextMeshProUGUI TotalRevenueText => totalRevenue;
    
    [Header("Customer Result")]
    [SerializeField] TextMeshProUGUI allCustomer;
    [SerializeField] TextMeshProUGUI servedCustomer;
    [SerializeField] TextMeshProUGUI goneCustomer;

    [SerializeField] public CanvasGroup canvasGroup;
    [SerializeField] private RectTransform panelTransform;
    private Vector2 originalPos;
    private bool isShown = false;

    private void Awake()
    {
        if (panelTransform == null) panelTransform = GetComponent<RectTransform>();
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();

        originalPos = panelTransform.anchoredPosition;

        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        gameObject.SetActive(true);
    }

    private void OnEnable()
    {
        EventBus.OnUIEvent += HandleUIEvent;
    }

    private void OnDisable()
    {
        EventBus.OnUIEvent -= HandleUIEvent;
    }
    
    private void HandleUIEvent(UIEventType eventType, object payload)
    {
        if (eventType == UIEventType.ShowResultPanel && GameManager.Instance.CurrentPhase != GamePhase.Dialogue)
        {
            ShowPanel();
        }
    }
    
    private void ShowPanel()
    {
        if (isShown) return;
        isShown = true;

        panelTransform.anchoredPosition = originalPos + new Vector2(0, 800f);

        Init();
        StartCoroutine(DelayedAnimate());
    }

    public void OnNextButtonClick()
    {
        EventBus.PlaySFX(SFXType.ButtonClick);

        canvasGroup.interactable = false;
        EventBus.Raise(UIEventType.HideResultPanel);
        canvasGroup.interactable = false;
        canvasGroup.DOFade(0f, 0.3f);
    }
    
    #region 통계 연결 & 애니메이션
    public void Init()
    {
        totalSalesVolume.text = "0";
        totalRevenue.text = "0";
        allCustomer.text = "0";
        servedCustomer.text = "0";
        goneCustomer.text = "0";
        
        SetSalesResult();
        SetCustomerResult();
    }
    
    private IEnumerator DelayedAnimate()
    {
        yield return new WaitForSeconds(1f); 
        AnimateEntrance(); 
    }
    
    private void SetSalesResult()
    {
        foreach (Transform child in contentTransform)
        {
            Destroy(child.gameObject);
        }

        var salesResults = MenuManager.Instance.GetAllMenuSalesData();
        if (salesResults == null) return;

        float delay = 1.5f;
        foreach (var sales in salesResults)
        {
            var go = Instantiate(contentPrefab, contentTransform);
            var foodUI = go.GetComponent<ResultPanelContent>();
            foodUI.SetData(sales);
            
            var cg = go.GetComponent<CanvasGroup>();
            if (cg == null)
                cg = go.AddComponent<CanvasGroup>();

            RectTransform rt = go.GetComponent<RectTransform>();
            rt.anchoredPosition += new Vector2(0, 100f);
            cg.alpha = 0f;
            rt.localScale = Vector3.one * 0.8f;

            DOTween.Sequence()
                .AppendInterval(delay)
                .Append(cg.DOFade(1f, 0.3f))
                .Join(rt.DOAnchorPosY(rt.anchoredPosition.y - 100f, 0.3f).SetRelative().SetEase(Ease.OutQuad))
                .Join(rt.DOScale(1f, 0.3f).SetEase(Ease.OutBack));

            delay += 0.05f;
        }

        int totalSales = MenuManager.Instance.GetTotalSalesToday();
        int totalRev = MenuManager.Instance.GetTotalRevenueToday();

        // 가장 마지막에 등장하는 메뉴 아이템 애니메이션이 끝난 후 총합 애니메이션 시작
        AnimateNumber(totalSalesVolume, totalSales, delay);
        AnimateNumber(totalRevenue, totalRev, delay);
    }
    
    private void SetCustomerResult()
    {
        int all = RestaurantManager.Instance.CustomersVisited;
        int served = RestaurantManager.Instance.CustomersServed;
        int gone = all - served;

        float delay = 1.5f;
        AnimateNumber(allCustomer, all, delay);
        delay += 0.3f;
        AnimateNumber(servedCustomer, served, delay);
        delay += 0.3f;
        AnimateNumber(goneCustomer, gone, delay);
    }
    
    private void AnimateEntrance()
    {
        if (!canvasGroup || !panelTransform) return;

        DG.Tweening.Sequence seq = DOTween.Sequence();
        seq.Append(canvasGroup.DOFade(1f, 0.7f));
        seq.Join(panelTransform.DOAnchorPos(originalPos, 0.5f).SetEase(Ease.OutBack));
        seq.OnComplete(() => canvasGroup.interactable = true);
        EventBus.OnBGMRequested(BGMEventType.PlayResultTheme);

    }
    private void AnimateNumber(TextMeshProUGUI targetText, int finalValue, float delay = 0f, float duration = 1.2f)
    {
        DOVirtual.DelayedCall(delay, () =>
        {
            DOVirtual.Int(0, finalValue, duration, value =>
            {
                targetText.text = value.ToString();
            }).SetEase(Ease.OutQuad);
        });
    }
    #endregion
}