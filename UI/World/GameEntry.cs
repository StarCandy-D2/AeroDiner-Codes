using System.Collections.Generic;
using UnityEngine;

public class GameEntry : MonoBehaviour
{
    void Start()
    {
        if (SaveLoadManager.HasSaveData())
        {
            var data = SaveLoadManager.LoadGame();
            ApplySettingsFromSave(data);
        }
        else
        {
            var defaultData = CreateDefaultSaveData();
            SaveLoadManager.SaveGame(defaultData);
            ApplySettingsFromSave(defaultData);
        }
        EventBus.RaiseFadeEvent(FadeEventType.FadeIn, new FadeEventPayload(0f, 1f));
        EventBus.PlayBGM(BGMEventType.Title);
    }
    private void ApplySettingsFromSave(SaveData data)
    {
        BGMManager.Instance.SetVolume(data.bgmVolume);
        SFXManager.Instance.SetVolume(data.sfxVolume);
        UIRoot.Instance.keyRebindManager.ApplyKeyBindings(data.keyBindings);
        // 튜토리얼 상태도 원하면 추가
        // TutorialManager.Instance.SetCompleted(data.isTutorialCompleted);
    }
    private SaveData CreateDefaultSaveData()
    {
        return new SaveData
        {
            currentDay = 1,
            totalEarnings = 0,
            menuDatabase = new HashSet<string>(),

            bgmVolume = 0.5f,
            sfxVolume = 0.5f,

            keyBindings = new Dictionary<string, string>
            {
                { "MoveUp", "<Keyboard>/w" },
                { "MoveDown", "<Keyboard>/s" },
                { "MoveLeft", "<Keyboard>/a" },
                { "MoveRight", "<Keyboard>/d" },
                { "Interact", "<Keyboard>/e" },
                { "Drop", "<Keyboard>/q" }
            },

        };
    }
}

