using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class KeyRebindManager : MonoBehaviour
{
    [SerializeField] private List<KeyRebindButton> rebindButtons;

    private void Start()
    {
        var data = SaveLoadManager.LoadGame() ?? new SaveData();

        if (data.keyBindings == null || data.keyBindings.Count == 0)
        {
            ResetAll(); // SaveData가 비어있으면 e/f 기본값 저장
        }
        else
        {
            ApplyKeyBindings(data.keyBindings);   // 저장된 값 적용
            UpdateAllKeyTexts();                  // UI 표시 동기화
        }
    }

    public void SaveAll()
    {
        var data = SaveLoadManager.LoadGame() ?? new SaveData();

        foreach (var btn in rebindButtons)
        {
            string path = btn.GetCurrentPath();
            data.keyBindings[btn.BindingSaveKey] = path;
        }

        SaveLoadManager.SaveGame(data);
        Debug.Log("[KeyRebindManager] 키 바인딩 저장 완료");
    }

    public void ResetAll()
    {
        var data = SaveLoadManager.LoadGame() ?? new SaveData();

        foreach (var btn in rebindButtons)
        {
            string defaultPath = "";

            // 방향키 WASD (2DVector Composite의 하위 바인딩)
            if (btn.BindingSaveKey.Contains("Move_binding_1")) // up
                defaultPath = "<Keyboard>/w";
            else if (btn.BindingSaveKey.Contains("Move_binding_2")) // down
                defaultPath = "<Keyboard>/s";
            else if (btn.BindingSaveKey.Contains("Move_binding_3")) // left
                defaultPath = "<Keyboard>/a";
            else if (btn.BindingSaveKey.Contains("Move_binding_4")) // right
                defaultPath = "<Keyboard>/d";

            // 상호작용 J
            else if (btn.BindingSaveKey.Contains("Interact"))
                defaultPath = "<Keyboard>/j";

            // 들기/내려놓기 K
            else if (btn.BindingSaveKey.Contains("Pickup/down"))
                defaultPath = "<Keyboard>/k";

            // 바인딩 덮어쓰기 및 저장
            if (!string.IsNullOrEmpty(defaultPath))
            {
                btn.actionRef.action.ApplyBindingOverride(btn.bindingIndex, defaultPath);
                data.keyBindings[btn.BindingSaveKey] = defaultPath;
                btn.UpdateKeyText();
            }
            else
            {
                Debug.LogWarning($"[KeyRebindManager] 기본 경로 지정 실패: {btn.BindingSaveKey}");
            }
        }

        SaveLoadManager.SaveGame(data);
        Debug.Log("[KeyRebindManager] 기본 키(WASD, J, K)로 리셋 완료");
    }


    public void ApplyKeyBindings(Dictionary<string, string> bindings)
    {
        foreach (var btn in rebindButtons)
        {
            if (btn == null || btn.actionRef?.action == null)
            {
                Debug.LogWarning($"[KeyRebindManager] actionRef 누락: {btn?.name}");
                continue;
            }

            if (bindings.TryGetValue(btn.BindingSaveKey, out var path))
            {
                btn.actionRef.action.ApplyBindingOverride(btn.bindingIndex, path);
            }
        }
    }

    public void UpdateAllKeyTexts()
    {
        foreach (var btn in rebindButtons)
        {
            btn.UpdateKeyText();
        }
    }

    public bool HasUnsavedChanges()
    {
        var data = SaveLoadManager.LoadGame();
        if (data == null) return true;

        foreach (var btn in rebindButtons)
        {
            string path = btn.GetCurrentPath();
            if (!data.keyBindings.TryGetValue(btn.BindingSaveKey, out var savedPath) || path != savedPath)
            {
                Debug.LogWarning($"[KeyRebindManager] 변경 감지: {btn.BindingSaveKey} 현재={path}, 저장={savedPath}");
                return true;
            }
        }
        return false;
    }
    public void CancelAll()
    {
        var data = SaveLoadManager.LoadGame();
        if (data == null || data.keyBindings == null)
        {
            Debug.LogWarning("[KeyRebindManager] 취소 불가: 저장된 키 정보 없음");
            return;
        }

        foreach (var btn in rebindButtons)
        {
            if (btn == null || btn.actionRef?.action == null)
                continue;

            if (data.keyBindings.TryGetValue(btn.BindingSaveKey, out string savedPath))
            {
                btn.actionRef.action.ApplyBindingOverride(btn.bindingIndex, savedPath);
                btn.UpdateKeyText();
            }
        }

        Debug.Log("[KeyRebindManager] 변경사항 취소 완료 (SaveData 기준 복원)");
    }
    public Dictionary<string, string> GetCurrentKeyBindings()
    {
        return rebindButtons.ToDictionary(b => b.BindingSaveKey, b => b.GetCurrentBinding());
    }
}
