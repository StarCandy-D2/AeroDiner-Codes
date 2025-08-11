using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VideoSettingPanel : MonoBehaviour
{

    public TMP_Text screenModeText;
    public TMP_Text resolutionText;

    private int screenModeIndex = 0;
    private int resolutionIndex = 0;

    private int originalScreenModeIndex = 0;
    private int originalResolutionIndex = 0;

    private readonly string[] screenModes = { "창모드", "테두리 없는 창모드", "전체 화면" };
    private readonly FullScreenMode[] fullScreenModes =
    {
        FullScreenMode.Windowed,
        FullScreenMode.FullScreenWindow,
        FullScreenMode.ExclusiveFullScreen
    };

    private readonly List<Vector2Int> resolutions = new List<Vector2Int>
    {
        new Vector2Int(1024, 768),
        new Vector2Int(1152, 864),
        new Vector2Int(1176, 664),
        new Vector2Int(1280, 720),
        new Vector2Int(1280, 768),
        new Vector2Int(1280, 800),
        new Vector2Int(1280, 960),
        new Vector2Int(1280, 1024),
        new Vector2Int(1360, 768),
        new Vector2Int(1366, 768),
        new Vector2Int(1440, 900),
        new Vector2Int(1440, 1080),
        new Vector2Int(1600, 900),
        new Vector2Int(1600, 1024),
        new Vector2Int(1680, 1050),
        new Vector2Int(1920, 1080)
    };


    private void OnEnable()
    {
        var data = SaveLoadManager.LoadGame();

        if (data == null)
        {
            OnClickReset(); // 리셋 → 내부적으로 OnClickSave도 실행됨
            return;
        }

        // 기존 저장값 로딩
        originalScreenModeIndex = data.screenModeIndex;
        originalResolutionIndex = data.resolutionIndex;

        screenModeIndex = originalScreenModeIndex;
        resolutionIndex = originalResolutionIndex;

        // 혹시라도 범위 바깥값이면 안전하게 보정
        if (resolutionIndex < 0 || resolutionIndex >= resolutions.Count)
        {
            var fallbackRes = new Vector2Int(Screen.width, Screen.height);
            if (!resolutions.Exists(r => r.x == fallbackRes.x && r.y == fallbackRes.y))
                resolutions.Insert(0, fallbackRes);

            resolutionIndex = 0;
            originalResolutionIndex = 0;
        }

        UpdateUI();
        UIRoot.Instance.tabButtonController.ApplyTabSelectionVisuals();
    }

    public void OnClickLeft_ScreenMode()
    {
        EventBus.PlaySFX(SFXType.ButtonClick);
        screenModeIndex = (screenModeIndex - 1 + screenModes.Length) % screenModes.Length;
        UpdateUI();
    }

    public void OnClickRight_ScreenMode()
    {
        EventBus.PlaySFX(SFXType.ButtonClick);
        screenModeIndex = (screenModeIndex + 1) % screenModes.Length;
        UpdateUI();
    }

    public void OnClickLeft_Resolution()
    {
        EventBus.PlaySFX(SFXType.ButtonClick);
        resolutionIndex = (resolutionIndex - 1 + resolutions.Count) % resolutions.Count;
        UpdateUI();
    }

    public void OnClickRight_Resolution()
    {
        EventBus.PlaySFX(SFXType.ButtonClick);
        resolutionIndex = (resolutionIndex + 1) % resolutions.Count;
        UpdateUI();
    }

    public void OnClickSave()
    {
        ApplyPending();

        originalScreenModeIndex = screenModeIndex;
        originalResolutionIndex = resolutionIndex;

        var data = SaveLoadManager.LoadGame() ?? new SaveData();
        data.screenModeIndex = screenModeIndex;
        data.resolutionIndex = resolutionIndex;
        SaveLoadManager.SaveGame(data);

    }

    public void OnClickReset()
    {
        screenModeIndex = 2; // 전체화면
        resolutionIndex = resolutions.FindIndex(r => r.x == 1920 && r.y == 1080);
        if (resolutionIndex < 0) resolutionIndex = 0;

        OnClickSave();
        UpdateUI();
    }

    /// <summary>
    /// ESC 등으로 변경 사항 무시 시 호출
    /// </summary>
    public void RollbackPending()
    {
        screenModeIndex = originalScreenModeIndex;
        resolutionIndex = originalResolutionIndex;
        UpdateUI();

    }
    public bool HasUnsavedChanges()
    {
        bool screenChanged = screenModeIndex != originalScreenModeIndex;
        bool resolutionChanged = resolutionIndex != originalResolutionIndex;

        if (screenChanged || resolutionChanged)
        {
            Debug.LogWarning("[VideoSettingPanel] 변경사항 감지됨:");
            if (screenChanged)
                Debug.LogWarning($"  - ScreenMode 변경됨: 현재={screenModeIndex}, 원본={originalScreenModeIndex}");
            if (resolutionChanged)
                Debug.LogWarning($"  - Resolution 변경됨: 현재={resolutionIndex}, 원본={originalResolutionIndex}");
        }

        return screenChanged || resolutionChanged;
    }
    /// <summary>
    /// 실제 해상도 적용
    /// </summary>
    private void ApplyPending()
    {
        Vector2Int res = resolutions[resolutionIndex];
        FullScreenMode mode = fullScreenModes[screenModeIndex];

        Screen.SetResolution(res.x, res.y, mode);
    }

    private void UpdateUI()
    {
        screenModeText.text = screenModes[screenModeIndex];
        Vector2Int res = resolutions[resolutionIndex];
        resolutionText.text = $"{res.x} * {res.y}";
    }
}