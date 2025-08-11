using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;

public class QuestPanel : MonoBehaviour
{
    [Header("탭 및 패널")]
    [SerializeField] private TabController tabController;
    [SerializeField] private GameObject noQuestPanel;

    [Header("ScrollView Content 부모")]
    [SerializeField] private Transform doingContent;
    [SerializeField] private Transform completeContent;

    [Header("리스트 아이템 프리팹")]
    [SerializeField] private GameObject questListItemPrefab;

    [Header("퀘스트 상세정보")]
    [SerializeField] private TMP_Text questNameText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text objectiveText;
    [SerializeField] private TMP_Text rewardText;

    [Header("Debug Info")]
    [SerializeField] private bool showDebugInfo;

    private void OnEnable()
    {
        if (tabController == null)
            tabController = GetComponentInChildren<TabController>();

        tabController.RequestSelectTab(0, true); // 첫 번째 탭 선택
        tabController.ApplyTabSelectionVisuals();
        RefreshQuestLists();
    }
    
    #region 버튼메서드
    
    public void OnClickDoningBtn()
    {
        if (showDebugInfo)
            Debug.Log("버튼 클릭 됨");
        EventBus.PlaySFX(SFXType.ButtonClick);
        tabController.RequestSelectTab(0);
        tabController.ApplyTabSelectionVisuals();
        RefreshQuestLists();
    }
    
    public void OnClickCompleteBtn()
    {
        if (showDebugInfo)
            Debug.Log("버튼 클릭 됨");
        EventBus.PlaySFX(SFXType.ButtonClick);
        tabController.RequestSelectTab(1);
        tabController.ApplyTabSelectionVisuals();
        RefreshQuestLists();
    }
    
    #endregion
    
    private void RefreshQuestLists()
    {
        ClearChildren(doingContent);
        ClearChildren(completeContent);

        var doingQuests = QuestManager.Instance.GetInProgressQuests();
        var finishedQuests = QuestManager.Instance.GetFinishedQuests();
        foreach (var quest in doingQuests)
        {
            CreateQuestListItem(quest, doingContent);
        }

        foreach (var quest in finishedQuests)
        {
            CreateQuestListItem(quest, completeContent);
        }

        // 공용 퀘스트 없음 패널 처리
        bool hasNoQuests = doingQuests.Count == 0 && finishedQuests.Count == 0;

        if (noQuestPanel != null)
            noQuestPanel.SetActive(hasNoQuests);

        // 상세 패널 초기화
        ShowQuestDetail(null);
        if (tabController.CurrentIndex == 0 && doingQuests.Count > 0)
        {
            ShowQuestDetail(doingQuests[0]);
        }
    }
    
    private void CreateQuestListItem(QuestData questData, Transform parent)
    {
        GameObject go = Instantiate(questListItemPrefab, parent);
        var item = go.GetComponent<Quest_ScrollView_Content>();
        item.SetData(questData);
        item.onClicked = () => ShowQuestDetail(questData);
    }
    
    private void ShowQuestDetail(QuestData quest)
    {
        if (quest == null)
        {
            questNameText.text = "";
            descriptionText.text = "";
            rewardText.text = "";
            objectiveText.text = "";
            return;
        }

        // 퀘스트 기본 정보 표시
        questNameText.text = quest.questName;
        descriptionText.text = quest.description;

        // 보상 정보가 있을 경우에만 표시
        if (!string.IsNullOrEmpty(quest.rewardDescription) || quest.rewardMoney > 0)
        {
            rewardText.text = $"보상: {quest.rewardDescription}";
        }
        else
        {
            rewardText.text = "보상: 없음";
        }
        
        StringBuilder objectivesBuilder = new StringBuilder();
        QuestStatus status = QuestManager.Instance.GetQuestStatus(quest.id);

        foreach (var obj in quest.objectives)
        {
            if (status == QuestStatus.Completed || status == QuestStatus.Failed)
            {
                objectivesBuilder.AppendLine($"<s>• {obj.description}</s>");
            }
            else // InProgress 상태
            {
                int currentAmount;
                int requiredAmount;

                switch (obj.objectiveType)
                {
                    case QuestObjectiveType.EarnMoney:
                        currentAmount = GameManager.Instance.TotalEarnings;
                        int.TryParse(obj.targetId, out requiredAmount);
                        break;
            
                    default:
                        currentAmount = QuestManager.Instance.GetQuestObjectiveProgress(quest.id, obj.targetId);
                        int.TryParse(obj.requiredIds.FirstOrDefault(), out requiredAmount);
                        break;
                }
                objectivesBuilder.AppendLine($"• {obj.description} ({currentAmount} / {requiredAmount})");
            }
        }
        objectiveText.text = objectivesBuilder.ToString().TrimEnd();
    }
    
    private void ClearChildren(Transform container)
    {
        if (container == null) return;

        foreach (Transform child in container)
        {
            if (child == null) continue;

            UnityEngine.UI.Button btn = child.GetComponentInChildren<UnityEngine.UI.Button>();
            if (btn != null)
                btn.onClick.RemoveAllListeners();

            Destroy(child.gameObject);
        }
    }
}

