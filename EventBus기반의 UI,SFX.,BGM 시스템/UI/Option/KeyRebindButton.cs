using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class KeyRebindButton : MonoBehaviour
{
    [Header("UI 참조")]
    [SerializeField] private TMP_Text keyText;

    [Header("바인딩 설정")]
    public InputActionReference actionRef;
    public int bindingIndex;

    private Button button;
    private bool waitingForKey = false;

    public string BindingSaveKey => $"{actionRef.action.name}_binding_{bindingIndex}";
    public string GetCurrentPath() => actionRef.action.bindings[bindingIndex].effectivePath;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(StartRebind);
    }

    private void Start()
    {
        // SaveData 기준으로 Override 적용 → 표시도 일치
        var data = SaveLoadManager.LoadGame();
        if (data != null && data.keyBindings.TryGetValue(BindingSaveKey, out string savedPath))
        {
            actionRef.action.ApplyBindingOverride(bindingIndex, savedPath);
        }

        UpdateKeyText();
    }

    private void StartRebind()
    {
        if (waitingForKey) return;

        EventBus.PlaySFX(SFXType.ButtonClick);
        keyText.text = "Press any key...";
        waitingForKey = true;

        actionRef.action.Disable();

        actionRef.action.PerformInteractiveRebinding(bindingIndex)
            .WithControlsExcluding("Mouse")
            .WithCancelingThrough("<Keyboard>/escape")
            .OnComplete(op =>
            {
                op.Dispose();
                waitingForKey = false;
                actionRef.action.Enable();
                UpdateKeyText();
            })
            .OnCancel(op =>
            {
                op.Dispose();
                waitingForKey = false;
                actionRef.action.Enable();
                UpdateKeyText();
            })
            .Start();
    }

    public void UpdateKeyText()
    {
        string displayName = InputControlPath.ToHumanReadableString(
            GetCurrentPath(),
            InputControlPath.HumanReadableStringOptions.OmitDevice
        );
        keyText.text = displayName;
    }

    public string actionName => actionRef.action.name;

    public string GetCurrentBinding() => GetCurrentPath();
}
