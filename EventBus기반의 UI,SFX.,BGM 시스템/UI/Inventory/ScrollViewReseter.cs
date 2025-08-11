using UnityEngine;
using UnityEngine.UI;

public class ScrollViewReseter : MonoBehaviour
{
    [SerializeField] private ScrollRect scrollRect;

    void OnEnable()
    {
        scrollRect.verticalNormalizedPosition = 1f;
    }
}
