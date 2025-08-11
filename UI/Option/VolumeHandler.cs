using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VolumeHandler : MonoBehaviour
{


    [Header("BGM 설정")]
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private TMP_Text bgmPercentageText;

    [Header("SFX 설정")]
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private TMP_Text sfxPercentageText;

    private float originalBGMVolume;
    private float originalSFXVolume;
    private float pendingBGMVolume;
    private float pendingSFXVolume;

    private void Start()
    {
        var data = SaveLoadManager.LoadGame();

        if (data == null)
        {
            Debug.Log("[VolumeHandler] 저장 파일 없음 → 기본 볼륨 설정 적용 및 저장");
            ResetVolumes(); // 내부에서 SaveVolumes도 호출됨
            return;
        }

        originalBGMVolume = data.bgmVolume < 0 ? 0.5f : data.bgmVolume;
        originalSFXVolume = data.sfxVolume < 0 ? 0.5f : data.sfxVolume;

        pendingBGMVolume = originalBGMVolume;
        pendingSFXVolume = originalSFXVolume;

        bgmSlider.SetValueWithoutNotify(originalBGMVolume);
        sfxSlider.SetValueWithoutNotify(originalSFXVolume);

        UpdateBGMVolumeUI(originalBGMVolume);
        UpdateSFXVolumeUI(originalSFXVolume);

        bgmSlider.onValueChanged.AddListener((v) =>
        {
            pendingBGMVolume = v;
            UpdateBGMVolumeUI(v);
        });

        sfxSlider.onValueChanged.AddListener((v) =>
        {
            pendingSFXVolume = v;
            UpdateSFXVolumeUI(v);
        });
    }
    private void OnEnable()
    {
        UIRoot.Instance.tabButtonController.ApplyTabSelectionVisuals();
    }
    private void UpdateBGMVolumeUI(float value)
    {
        bgmPercentageText.text = $"{Mathf.RoundToInt(value * 100)}%";
        BGMManager.Instance.SetVolume(value);
    }

    private void UpdateSFXVolumeUI(float value)
    {
        sfxPercentageText.text = $"{Mathf.RoundToInt(value * 100)}%";
        SFXManager.Instance.SetVolume(value);
    }
    public bool HasUnsavedChanges()
    {
        bool bgmChanged = !Mathf.Approximately(pendingBGMVolume, originalBGMVolume);
        bool sfxChanged = !Mathf.Approximately(pendingSFXVolume, originalSFXVolume);

        if (bgmChanged || sfxChanged)
        {
            Debug.LogWarning("[VolumeHandler] 변경사항 감지됨:");
            if (bgmChanged)
                Debug.LogWarning($"  - BGM 변경됨: 현재={pendingBGMVolume}, 원본={originalBGMVolume}");
            if (sfxChanged)
                Debug.LogWarning($"  - SFX 변경됨: 현재={pendingSFXVolume}, 원본={originalSFXVolume}");
        }

        return bgmChanged || sfxChanged;
    }
    /// <summary> 저장 버튼 클릭 시 호출 </summary>
    public void SaveVolumes()
    {
        originalBGMVolume = pendingBGMVolume;
        originalSFXVolume = pendingSFXVolume;

        var data = SaveLoadManager.LoadGame() ?? new SaveData();
        data.bgmVolume = originalBGMVolume;
        data.sfxVolume = originalSFXVolume;
        SaveLoadManager.SaveGame(data);

        Debug.Log($"[VolumeHandler] 볼륨 저장됨: BGM {originalBGMVolume}, SFX {originalSFXVolume}");
    }

    /// <summary> 리셋 버튼 클릭 시 호출 </summary>
    public void ResetVolumes()
    {
        pendingBGMVolume = 0.5f;
        pendingSFXVolume = 0.5f;

        bgmSlider.SetValueWithoutNotify(0.5f);
        sfxSlider.SetValueWithoutNotify(0.5f);

        UpdateBGMVolumeUI(0.5f);
        UpdateSFXVolumeUI(0.5f);
        EventBus.PlaySFX(SFXType.ButtonClick);
        SaveVolumes();
    }

    /// <summary> ESC로 옵션 창 닫을 때 호출 — 원래 값으로 롤백 </summary>
    public void RollbackVolumes()
    {
        pendingBGMVolume = originalBGMVolume;
        pendingSFXVolume = originalSFXVolume;

        bgmSlider.SetValueWithoutNotify(originalBGMVolume);
        sfxSlider.SetValueWithoutNotify(originalSFXVolume);

        UpdateBGMVolumeUI(originalBGMVolume);
        UpdateSFXVolumeUI(originalSFXVolume);

        Debug.Log("[VolumeHandler] 볼륨 롤백 완료");
    }
}