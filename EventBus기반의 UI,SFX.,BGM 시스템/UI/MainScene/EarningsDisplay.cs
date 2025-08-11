using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Serialization;

public class EarningsDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI earningText;
    [SerializeField] private float animateDuration = 0.5f;
    [SerializeField] private Color flashColor = Color.yellow;

    private int currentDisplayAmount;
    private Color originalColor;

    private void Awake()
    {
        if (!earningText)
            earningText = transform.FindChild<TextMeshProUGUI>("Tmp_Earnings");

        originalColor = earningText.color;
        InitializeEarnings();
    }

    public void InitializeEarnings()
    {
        currentDisplayAmount = GameManager.Instance.TotalEarnings;
        earningText.text = $"{Mathf.RoundToInt(currentDisplayAmount):N0} G";
    }

    public void AnimateEarnings(int newAmount)
    {
        DOTween.Kill(earningText);
        DOTween.Kill(earningText.transform);

        //현재 UI에 표시된 금액을 기준으로 애니메이션 시작
        int fromAmount = currentDisplayAmount;
        
        //애니메이션: 현재 표시값 → 새로운 수치
        DOVirtual.Int(fromAmount, newAmount, animateDuration, value =>
        {
            currentDisplayAmount = value;
            earningText.text = $"{value:N0} G";
        }).SetEase(Ease.OutCubic);

        //시각 효과
        var seq = DOTween.Sequence();
        seq.Append(earningText.DOColor(flashColor, 0.2f));
        seq.Join(earningText.transform.DOScale(1.2f, 0.2f));
        seq.AppendInterval(0.1f);
        seq.Append(earningText.DOColor(originalColor, 0.2f));
        seq.Join(earningText.transform.DOScale(1.0f, 0.2f));
    }
}