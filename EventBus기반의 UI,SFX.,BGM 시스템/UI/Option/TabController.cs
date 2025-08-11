using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabController : MonoBehaviour
{
    [SerializeField] public List<Button> tabButtons;
    [SerializeField] public List<Image> tabImages;

    [SerializeField] private List<RectTransform> tabContents;

    [SerializeField] private Color selectedColor = Color.white;
    [SerializeField] private Color unselectedColor = Color.gray;

    [SerializeField] private float animationDuration = 0.3f;
    [SerializeField] private float slideDistance = 1920f;

    private int currentSelectedIndex = -1;
    public int CurrentIndex => currentSelectedIndex;



    private void Start()
    {
        RequestSelectTab(0, true);              // 0번 탭을 예약하고
        ApplyTabSelectionVisuals();       // 실제로 색상을 반영
    }

    public void RequestSelectTab(int index, bool instant = false)
    {
        if (index == currentSelectedIndex && currentSelectedIndex != -1) return;

        int previousIndex = currentSelectedIndex; // 순서 수정
        currentSelectedIndex = index;

        ApplyTabSelectionVisuals();

        if (instant)
        {
            for (int i = 0; i < tabContents.Count; i++)
            {
                tabContents[i].gameObject.SetActive(i == currentSelectedIndex);
                tabContents[i].anchoredPosition = Vector2.zero;
            }
            return;
        }

        int direction = index > previousIndex ? 1 : -1;
        Vector2 exitPos = new Vector2(-direction * slideDistance, 0);
        Vector2 enterFrom = new Vector2(direction * slideDistance, 0);

        if (previousIndex >= 0 && previousIndex < tabContents.Count)
        {
            var prevPanel = tabContents[previousIndex];
            prevPanel.DOKill(); // 충돌 방지
            prevPanel.DOAnchorPos(exitPos, animationDuration).SetEase(Ease.InOutQuad)
                .SetUpdate(true)
                .OnComplete(() => prevPanel.gameObject.SetActive(false));
        }

        var newPanel = tabContents[index];
        newPanel.DOKill(); // 충돌 방지
        newPanel.gameObject.SetActive(true);
        newPanel.anchoredPosition = enterFrom;
        newPanel.DOAnchorPos(Vector2.zero, animationDuration).SetEase(Ease.InOutQuad)
        .SetUpdate(true);
    }

    // 실제 탭이 보여질 때 호출
    public  void ApplyTabSelectionVisuals()
    {
        for (int i = 0; i < tabImages.Count; i++)
        {
            tabImages[i].color = (i == currentSelectedIndex) ? selectedColor : unselectedColor;
        }
    }

}