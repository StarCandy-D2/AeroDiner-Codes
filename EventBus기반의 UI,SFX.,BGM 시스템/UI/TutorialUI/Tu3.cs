using UnityEngine;
using TMPro;
using System.Collections;

public class Tu3 : MonoBehaviour
{
    [Header("튜토리얼 텍스트")]
    [SerializeField] private GameObject textBoxObject;
    [SerializeField] private TMP_Text tutorialText;
    [SerializeField] private float typingSpeed = 0.04f;

    private Coroutine typingCoroutine;

    private void OnEnable()
    {
        ShowTutorialStep1();
    }

    private void ShowTextWithTyping(string message)
    {
        textBoxObject.SetActive(true);

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeText(message));
    }

    private IEnumerator TypeText(string message)
    {
        tutorialText.text = "";
        foreach (char c in message)
        {
            tutorialText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    //  튜토리얼 단계 메서드들
    public void ShowTutorialStep1()
    {
        ShowTextWithTyping("채소 상자에 가까이 다가가 채소를 들어올리세요!");
    }

    public void ShowTutorialStep2()
    {
        ShowTextWithTyping("들고 있는 채소를 도마에 가까이 다가가 내려놓으세요!");
    }

    public void ShowTutorialStep3()
    {
        ShowTextWithTyping("도마와 일정 시간 동안 상호작용해 채소를 손질해보세요!");
    }

    public void ShowTutorialStep4()
    {
        ShowTextWithTyping("이번엔 면을 삶아보죠!\n싱크대에서 물을, 노란 면다발에서 건조 면을 꺼내서\n냄비에 집어넣으면 알아서 끓을 거에요!");
    }

    public void ShowTutorialStep5()
    {
        ShowTextWithTyping("이제 파스타를 만들 시간입니다!\n우선 삶은 면, 토마토 소스, 손질한 채소를 모두 프라이팬에 넣어주세요!");
    }

    public void ShowTutorialStep6()
    {
        ShowTextWithTyping("이제 재료가 모인 프라이팬을 일정 시간 동안 상호작용해서\n파스타를 완성해주세요!");
    }

    public void ShowTutorialStep7()
    {
        ShowTextWithTyping("좋아요! 이제 그 파스타를 제게 서빙해주세요!");
    }
}
