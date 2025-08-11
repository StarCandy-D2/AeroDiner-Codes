using DG.Tweening;
using UnityEngine;
using System.Collections;
using System.Threading.Tasks;

public class Tu2 : MonoBehaviour
{
    [Header("참조")]
    [SerializeField] private GameObject menuPanelContent;      // Content 프리팹
    [SerializeField] public RectTransform menuPanelTransform;
    [SerializeField] private Transform contentTransform;      // ScrollView의 Content
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private GameObject deleteWarningPopup;
    [SerializeField] private GameObject warningPopup; // 팝업 루트
    [SerializeField] private CanvasGroup warningPopupCanvas; // 팝업의 CanvasGroup

    [Header("DOTween 설정")]
    [SerializeField] private float popupFadeDuration = 0.5f;
    [SerializeField] private float popupVisibleTime = 2f;

    [Header("Debug Info")]
    [SerializeField] private bool showDebugInfo;

    [SerializeField] private CanvasGroup smallcanvasGroup;
    [SerializeField] private GameObject targetPanel;
    [SerializeField] private GameObject pointer1;
    [SerializeField] private GameObject pointer2;
    [SerializeField] private RectTransform pointer2TargetTransform;

    private Vector2 originalPos;

    private void Awake()
    {
        menuPanelContent = Resources.Load<GameObject>("Prefabs/UI/ScrollViewContent/MenuPanelContent");
        contentTransform = transform.FindChild<RectTransform>("Content");
        canvasGroup = GetComponent<CanvasGroup>();
        warningPopup = transform.FindChild<CanvasGroup>("WarningPopup").gameObject;
        warningPopupCanvas = warningPopup.GetComponent<CanvasGroup>();
    }
    private void Start()
    {
        if (pointer1 != null) pointer1.SetActive(true);
        if (pointer2 != null) pointer2.SetActive(false);
    }
    private void OnEnable()
    {
        foreach (var menu in MenuManager.Instance.GetUnlockedMenus())
        {
            MenuManager.Instance.SetMenuSelection(menu.foodData.id, false);
        }

        GenerateFoodList();

        if (menuPanelTransform != null)
        {
            menuPanelTransform.anchoredPosition = Vector2.zero;
            originalPos = Vector2.zero;

            menuPanelTransform.anchoredPosition += new Vector2(0, 1600f);
        }

        if (canvasGroup != null)
            canvasGroup.alpha = 0f;

        StartCoroutine(DelayedAnimateEntrance());
    }

    public void CheckMenuSelectionAndSwitchPointer(string menuId)
    {
        if (MenuManager.Instance.IsMenuSelected(menuId))
        {
            if (pointer1 != null) pointer1.SetActive(false);
            if (pointer2 != null)
            {
                pointer2.SetActive(true);

                // pointer2를 새로운 위치로 이동
                pointer2.transform.position = pointer2TargetTransform.position;

                // 필요시 부드럽게 이동 (DOTween 사용)
                pointer2.transform.DOMove(pointer2TargetTransform.position, 0.5f).SetEase(Ease.OutQuad);
            }
        }
    }
    public void UncheckMenu()
    {
        pointer2.SetActive(false);
        pointer1.SetActive(true);
    }
    public void GenerateFoodList()
    {
        foreach (Transform child in contentTransform)
        {
            Destroy(child.gameObject);
        }

        var menuList = MenuManager.Instance.GetUnlockedMenus();
        if (menuList == null)
        {
            if (showDebugInfo) Debug.LogWarning("TodayMenus 가 null입니다!");
            return;
        }

        //Debug.Log($" 메뉴 수: {menuList.Count}");


        foreach (var menu in menuList)
        {
            if (menu == null)
            {
                if (showDebugInfo) Debug.LogWarning("null인 Menu 발견");
                continue;
            }

            var go = Instantiate(menuPanelContent, contentTransform);
            var foodUI = go.GetComponent<MenuPanelContent>();
            foodUI.SetData(menu);

            #region 메뉴등장 애니메이션

            float delay = 1f;
            var cg = go.GetComponent<CanvasGroup>();
            if (!cg)
            {
                cg = go.AddComponent<CanvasGroup>();
            }

            RectTransform rt = go.GetComponent<RectTransform>();
            rt.anchoredPosition += new Vector2(0, 100f); // 약간 위에서 떨어짐
            cg.alpha = 0f;
            rt.localScale = Vector3.one * 0.8f;

            DOTween.Sequence()
                .AppendInterval(delay)
                .Append(cg.DOFade(1f, 0.3f))
                .Join(rt.DOAnchorPosY(rt.anchoredPosition.y - 100f, 0.3f).SetRelative().SetEase(Ease.OutQuad))
                .Join(rt.DOScale(1f, 0.3f).SetEase(Ease.OutBack));

            delay += 0.05f; // 순차적으로 나옴

            #endregion
        }
    }

    public void OnClickBackBtn()
    {
        PlayExitAnimation();
        GameManager.Instance.ChangePhase(GamePhase.EditStation);
        EventBus.PlayBGM(BGMEventType.PlayLifeTheme);
    }

    /// <summary>
    /// 시작버튼 클릭
    /// </summary>
    public void OnClickDayStartBtn()
    {
        HandleDayStart();
    }

    private void HandleDayStart()
    {
        EventBus.PlaySFX(SFXType.ButtonClick);

        // 메뉴 선택 여부 확인
        bool anyToggled = false;
        foreach (Transform child in contentTransform)
        {
            var content = child.GetComponent<MenuPanelContent>();
            if (content != null && content.toggle.isOn)
            {
                anyToggled = true;
                break;
            }
        }

        if (!anyToggled)
        {
            Debug.LogWarning("[MenuPanel] 선택된 메뉴가 없습니다!");
            ShowNoMenuSelectedPopup();
            return;
        }

        //  삭제 셀 확인
        bool hasStationInDeleteGrid = false;
        foreach (var deleteCell in FindObjectsOfType<DeleteGridCell>())
        {
            if (deleteCell.HasStationToBeDeleted())
            {
                hasStationInDeleteGrid = true;
                break;
            }
        }

        if (hasStationInDeleteGrid)
        {
            ShowDeletePopup();
            return;
        }

        // 정상 전환
        ProceedToMainScene();
    }

    private void ShowDeletePopup()
    {
        if (deleteWarningPopup == null) return;

        deleteWarningPopup.SetActive(true);
    }

    public void HideDeleteConfirmationPopup()
    {
        deleteWarningPopup?.SetActive(false);
    }

    public void OnClickConfirmProceed()
    {
        deleteWarningPopup?.SetActive(false);
        ProceedToMainScene();
    }

    #region 애니메이션

    private void ProceedToMainScene()
    {
        PlayExitAnimation();
        EventBus.RaiseFadeEvent(FadeEventType.FadeOutAndLoadScene, new FadeEventPayload(1f, 1f, scene: "MainScene"));
    }

    private void ShowDeleteStationPopup()
    {
        if (deleteWarningPopup == null) return;

        var cg = deleteWarningPopup.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = deleteWarningPopup.AddComponent<CanvasGroup>();

        deleteWarningPopup.SetActive(true);
        cg.alpha = 0f;

        cg.DOFade(1f, popupFadeDuration)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                DOVirtual.DelayedCall(popupVisibleTime, () =>
                {
                    cg.DOFade(0f, popupFadeDuration)
                        .SetEase(Ease.InQuad)
                        .OnComplete(() => deleteWarningPopup.SetActive(false));
                });
            });
    }

    private void ShowNoMenuSelectedPopup()
    {
        if (warningPopup == null || warningPopupCanvas == null) return;

        warningPopup.SetActive(true);
        warningPopupCanvas.alpha = 0f;
        warningPopupCanvas.DOFade(1f, popupFadeDuration)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                DOVirtual.DelayedCall(popupVisibleTime, () =>
                {
                    warningPopupCanvas.DOFade(0f, popupFadeDuration)
                        .SetEase(Ease.InQuad)
                        .OnComplete(() => warningPopup.SetActive(false));
                });
            });
    }

    private IEnumerator DelayedAnimateEntrance()
    {
        yield return new WaitForSeconds(0.5f); // 약간의 연출 지연
        AnimateEntrance();
        FadeInPanel();
    }

    private void AnimateEntrance()
    {
        if (!canvasGroup || !menuPanelTransform) return;

        Sequence seq = DOTween.Sequence();
        seq.SetUpdate(true);
        seq.Append(canvasGroup.DOFade(1f, 0.7f));
        seq.Join(menuPanelTransform.DOAnchorPos(originalPos, 0.5f).SetEase(Ease.OutBack));
    }
    public void FadeInPanel(float duration = 0.5f)
    {
        targetPanel.SetActive(false);
        DOVirtual.DelayedCall(1f, () =>
        {
            targetPanel.SetActive(true);
            smallcanvasGroup.alpha = 0f;
            smallcanvasGroup.DOFade(1f, duration)
                .SetEase(Ease.OutQuad)
                .OnStart(() =>
                {
                    smallcanvasGroup.interactable = true;
                    smallcanvasGroup.blocksRaycasts = true;
                });
        });
    }
    private void PlayExitAnimation()
    {
        Vector2 originalPos = menuPanelTransform.anchoredPosition;

        Sequence exitSeq = DOTween.Sequence();
        exitSeq.Append(menuPanelTransform.DOAnchorPosY(originalPos.y - 50f, 0.3f).SetEase(Ease.InQuad))
               .Append(menuPanelTransform.DOAnchorPosY(originalPos.y + 1200f, 1f).SetEase(Ease.InBack))
               .OnComplete(() =>
               {
                   gameObject.SetActive(false);
                   menuPanelTransform.anchoredPosition = originalPos;

                   EventBus.Raise(UIEventType.HideMenuPanel);
               });
    }

    #endregion


   
}