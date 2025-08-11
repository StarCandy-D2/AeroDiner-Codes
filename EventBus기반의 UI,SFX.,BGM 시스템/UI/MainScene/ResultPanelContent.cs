using System;
using UnityEngine;
using TMPro;

public class ResultPanelContent : BaseScrollViewItem
{
    [SerializeField] private TextMeshProUGUI menuName;
    [SerializeField] private TextMeshProUGUI soldCount;
    [SerializeField] private TextMeshProUGUI menuRevenue;

    private void Reset()
    {
        menuName = transform.FindChild<TextMeshProUGUI>("Tmp_MenuName");
        soldCount = transform.FindChild<TextMeshProUGUI>("Tmp_SoldCount");
        menuRevenue = transform.FindChild<TextMeshProUGUI>("Tmp_MenuRevenue");
    }

    /// <summary>
    /// Menu 데이터로 UI 설정
    /// </summary>
    public void SetData(MenuSalesData menu)
    {
        menuName.text = menu.MenuName;
        soldCount.text = menu.SoldCount.ToString();
        menuRevenue.text = menu.TotalRevenue.ToString();
    }
}
