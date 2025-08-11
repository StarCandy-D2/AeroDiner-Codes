using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections.Generic;

public class DialogueUI : MonoBehaviour
{
    [Header("UI 참조")]
    [SerializeField] private GameObject rootPanel;
    [SerializeField] private TMP_Text speakerNameText;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private Image leftPortraitImage;
    [SerializeField] private Image rightPortraitImage;
    [SerializeField] private GameObject nextButton;          
    [SerializeField] private CanvasGroup nextButtonGroup;

    private string currentLeftSpeakerId;
    private string currentRightSpeakerId;

    private Tween typingTween;
    private Tween blinkingTween;
    private HashSet<string> appearedSpeakers = new();
    private string fullCurrentText;

    private void OnEnable()
    {
        EventBus.OnUIEvent += HandleUIEvent;
        
        appearedSpeakers.Clear();
        currentLeftSpeakerId = null;
        currentRightSpeakerId = null;
        SetAlpha(leftPortraitImage, 0f);
        SetAlpha(rightPortraitImage, 0f);
        nextButton.SetActive(false);
    }

    private void OnDisable()
    {
        EventBus.OnUIEvent -= HandleUIEvent;
    }

    private void Update()
    {
        if (!rootPanel.activeInHierarchy) return;
        if (Input.anyKeyDown || Input.GetMouseButtonDown(0))
        {
            OnClickNext();
        }
    }

    public void OnClickNext()
    {
        if (typingTween != null && typingTween.IsActive() && typingTween.IsPlaying())
        {
            typingTween.Complete();

            dialogueText.text = fullCurrentText;

            return;
        }

        StopBlinkingNextButton();
        DialogueManager.Instance.RequestNextLine();
    }

    public void OnClickSkip()
    {
        typingTween?.Kill();
        blinkingTween?.Kill();
        nextButton.SetActive(false);

        DialogueManager.Instance.SkipDialogue();
        rootPanel.SetActive(false);
    }

    private void HandleUIEvent(UIEventType type, object payload)
    {
        switch (type)
        {
            case UIEventType.ShowDialogueLine:
                if (payload is DialogueLine line) DisplayLine(line);
                break;
            case UIEventType.HideDialoguePanel:
                rootPanel.SetActive(false);
                break;
        }
    }

    private void DisplayLine(DialogueLine line)
    {
        rootPanel.SetActive(true);

        var speaker = DialogueManager.Instance.FindSpeakerById(line.speakerId);
        speakerNameText.text = speaker?.speakerName ?? line.speakerId;
        
        var activePortrait = UpdatePortraits(line, speaker);
        AnimateSpeakerIntroduction(activePortrait, line.speakerId);
        AnimateTextTyping(line.text);
    }

    // 초상화 업데이트
    private Image UpdatePortraits(DialogueLine line, SpeakerData speaker)
    {
        Image activePortrait;
        Image inactivePortrait;

        if (line.position == DialoguePosition.Left)
        {
            currentLeftSpeakerId = line.speakerId;
            activePortrait = leftPortraitImage;
            inactivePortrait = rightPortraitImage;
            if (string.IsNullOrEmpty(currentRightSpeakerId)) SetAlpha(inactivePortrait, 0f);
        }
        else
        {
            currentRightSpeakerId = line.speakerId;
            activePortrait = rightPortraitImage;
            inactivePortrait = leftPortraitImage;
            if (string.IsNullOrEmpty(currentLeftSpeakerId)) SetAlpha(inactivePortrait, 0f);
        }

        activePortrait.sprite = speaker?.GetPortraitByExpression(line.expression);
        SetAlpha(activePortrait, 1f);
        if (inactivePortrait.sprite)
        {
            SetAlpha(inactivePortrait, 0.3f);
        }

        return activePortrait;
    }

    #region DOTween 연출

    // 화자 등장 연출
    private void AnimateSpeakerIntroduction(Image activePortrait, string speakerId)
    {
        if (activePortrait && appearedSpeakers.Add(speakerId))
        {
            activePortrait.rectTransform.localScale = Vector3.one;
            activePortrait.rectTransform.DOScale(1.1f, 0.15f).SetLoops(2, LoopType.Yoyo).SetEase(Ease.OutQuad);
        }
    }

    // 텍스트 타이핑 연출
    private void AnimateTextTyping(string text)
    {
        typingTween?.Kill();
        dialogueText.text = "";
        fullCurrentText = text; // 현재 전체 문장 저장

        typingTween = DOTween.To(() => "", x => dialogueText.text = x, text, 0.03f * text.Length)
            .SetEase(Ease.Linear)
            .OnComplete(StartBlinkingNextButton);
    }

    private void SetAlpha(Image image, float alpha)
    {
        if (!image) return;
        var c = image.color;
        c.a = alpha;
        image.color = c;
    }
    
    private void StartBlinkingNextButton()
    {
        nextButton.SetActive(true);
        nextButtonGroup.alpha = 1f;

        blinkingTween?.Kill();
        blinkingTween = nextButtonGroup.DOFade(0f, 0.5f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

    private void StopBlinkingNextButton()
    {
        blinkingTween?.Kill();
        nextButton.SetActive(false);
    }

    #endregion
}