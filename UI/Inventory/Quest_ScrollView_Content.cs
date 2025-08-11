using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Quest_ScrollView_Content : BaseScrollViewItem
{
    [SerializeField] private TMP_Text titleText;

    public Action onClicked;

    protected override void Awake()
    {
        base.Awake();
        GetComponent<Button>().onClick.AddListener(() => onClicked?.Invoke());
    }

    public void SetData(QuestData data)
    {
        titleText.text = data.questName;
    }
}
