using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TutorialCursorController : MonoBehaviour
{
    [Header("커서")]
    [SerializeField] private Image sprite1Cursor;
    [SerializeField] private Sprite sprite1A;
    [SerializeField] private Sprite sprite1B;

    [SerializeField] private Image sprite2Cursor;
    [SerializeField] private Sprite sprite2A;
    [SerializeField] private Sprite sprite2B;

    [SerializeField] private float interval = 0.5f;

    private Tween sprite1Tween;
    private Tween sprite2Tween;
    private bool sprite1Toggle = true;
    private bool sprite2Toggle = true;

    private void Start()
    {
        // sprite1 시작
        StartSprite1Loop();

        // sprite2는 기본 비활성화
        sprite2Cursor.gameObject.SetActive(false);
    }

    public void OnTutorialTriggerNextStep() 
    {
        StopSprite1Loop();
        StartSprite2Loop();
    }

    private void StartSprite1Loop()
    {
        sprite1Cursor.gameObject.SetActive(true);
        sprite1Tween = DOTween.Sequence()
            .AppendCallback(() =>
            {
                sprite1Cursor.sprite = sprite1Toggle ? sprite1A : sprite1B;
                sprite1Toggle = !sprite1Toggle;
            })
            .AppendInterval(interval)
            .SetLoops(-1)
            .SetUpdate(true);
    }

    private void StopSprite1Loop()
    {
        sprite1Tween.Kill();
        sprite1Cursor.gameObject.SetActive(false);
    }

    private void StartSprite2Loop()
    {
        sprite2Cursor.gameObject.SetActive(true);
        sprite2Tween = DOTween.Sequence()
            .AppendCallback(() =>
            {
                sprite2Cursor.sprite = sprite2Toggle ? sprite2A : sprite2B;
                sprite2Toggle = !sprite2Toggle;
            })
            .AppendInterval(interval)
            .SetLoops(-1)
            .SetUpdate(true);
    }

    private void StopSprite2Loop()
    {
        sprite2Tween.Kill();
        sprite2Cursor.gameObject.SetActive(false);
    }
}