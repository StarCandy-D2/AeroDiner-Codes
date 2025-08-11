using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TuPointer : MonoBehaviour
{
    [SerializeField] private Image targetImage;
    [SerializeField] private Sprite spriteA;
    [SerializeField] private Sprite spriteB;
    [SerializeField] private float interval = 0.5f;

    private bool isA = true;

    private void Start()
    {
        if (targetImage == null)
            targetImage = GetComponent<Image>();

        DOTween.Sequence()
            .AppendCallback(SwitchSprite)
            .AppendInterval(interval)
            .SetLoops(-1)
            .SetUpdate(true);
    }

    private void SwitchSprite()
    {
        targetImage.sprite = isA ? spriteA : spriteB;
        isA = !isA;
    }
}
