using UnityEngine;
using UnityEngine.UI;

public class RoundTimerUI : MonoBehaviour
{
    [SerializeField] private Image timerImage;

    private void Update()
    {
        if (!RestaurantManager.Instance || !timerImage) return;

        float time = RestaurantManager.Instance.CurrentRoundTime;
        float limit = RestaurantManager.Instance.RoundTimeLimit;

        float percent = Mathf.Clamp01(time / limit);
        timerImage.fillAmount = 1f - percent;
    }
}
