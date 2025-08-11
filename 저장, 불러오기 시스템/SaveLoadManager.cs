using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static SaveData;

public class SaveLoadManager : Singleton<SaveLoadManager>
{
    [Header("Debug")]
    [SerializeField] private bool showDebugInfo;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    private static string savePath => Path.Combine(Application.persistentDataPath, StringPath.SAVE_PATH);
    private static string stationSavePath => Path.Combine(Application.persistentDataPath, StringPath.STATION_PATH);

    // 저장
    public static void SaveGame(SaveData data)
    {
        try
        {
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(savePath, json);
            if (Instance?.showDebugInfo == true)
                Debug.Log($"[SaveLoadManager] 게임 저장 완료: {savePath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[SaveLoadManager] 저장 실패: {e.Message}");
        }
    }

    // 불러오기
    public static SaveData LoadGame()
    {
        if (!File.Exists(savePath))
        {
            if (Instance?.showDebugInfo == true)
                Debug.LogWarning("[SaveLoadManager] 저장 파일이 없습니다.");

            return null;
        }
        try
        {
            string json = File.ReadAllText(savePath);
            SaveData data = JsonConvert.DeserializeObject<SaveData>(json);

            if (Instance?.showDebugInfo == true)
                Debug.Log("[SaveLoadManager] 게임 불러오기 완료");

            return data;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[SaveLoadManager] 불러오기 실패: {e.Message}");
            return null;
        }
    }

    // 저장 파일 존재 여부
    public static bool HasSaveData()
    {
        bool exists = File.Exists(savePath);

        if (Instance?.showDebugInfo == true)
            Debug.Log($"[SaveLoadManager] 저장 파일 존재 여부: {exists}");

        return exists;
    }
    
    // 옵션 제외 게임데이터 초기화 (New Game 시)
    public static void ResetProgressOnly()
    {
        var data = LoadGame();
        if (data == null)
        {
            data = new SaveData();
            if (Instance?.showDebugInfo == true)
                Debug.LogWarning("[SaveLoadManager] 기존 세이브 없음 → 새 SaveData로 초기화함");
        }

        // 현재 바인딩 정보 저장
        var preservedKeyBindings = data.keyBindings != null
            ? new Dictionary<string, string>(data.keyBindings)
            : new Dictionary<string, string>();

        // 진행 정보 초기화
        data.currentDay = 1;
        data.totalEarnings = 0;
        data.menuDatabase.Clear();

        // 다시 보존한 키 바인딩 할당
        data.keyBindings = preservedKeyBindings;

        SaveGame(data);
        
        if (File.Exists(stationSavePath))
        {
            File.Delete(stationSavePath);
        }

        if (Instance?.showDebugInfo == true)
            Debug.Log("[SaveLoadManager] 진행 정보만 초기화됨 (옵션 및 키 바인딩 유지)");
    }

    public static void SaveStationData(List<StationSaveInfo> infos)
    {
        string path = stationSavePath;

        try
        {
            string json = JsonConvert.SerializeObject(infos, Formatting.Indented);
            File.WriteAllText(path, json);

            if (Instance?.showDebugInfo == true)
                Debug.Log($"[SaveLoadManager] 스테이션 저장 완료: {path}");
        }
        catch (System.Exception e)
        {
            if (Instance?.showDebugInfo == true)
                Debug.LogError($"[SaveLoadManager] 스테이션 저장 실패: {e.Message}");
        }
    }

    public static List<StationSaveInfo> LoadStationData()
    {
        string path = stationSavePath;

        try
        {
            if (!File.Exists(path))
            {
                if (Instance?.showDebugInfo == true)
                    Debug.LogWarning($"[SaveLoadManager] station.json 파일이 존재하지 않습니다 → {path}");

                // 현재 상태 저장
                if (StationManager.Instance)
                {
                    StationManager.Instance.Save();

                    // 다시 불러오기
                    string jsonAfterSave = File.ReadAllText(path);
                    var infosAfterSave = JsonConvert.DeserializeObject<List<StationSaveInfo>>(jsonAfterSave);

                    if (Instance?.showDebugInfo == true)
                        Debug.Log($"[SaveLoadManager] station.json 저장 후 로드 완료: {infosAfterSave.Count}개 항목");

                    return infosAfterSave;
                }
            }

            string json = File.ReadAllText(path);
            var infos = JsonConvert.DeserializeObject<List<StationSaveInfo>>(json);
            if (Instance?.showDebugInfo == true)
                Debug.Log($"[SaveLoadManager] station.json 로드 완료: {infos.Count}개 항목");

            return infos;
        }
        catch (System.Exception e)
        {
            if (Instance?.showDebugInfo == true)
                Debug.LogError($"[SaveLoadManager] station.json 로드 실패: {e.Message}");
            return null;
        }
    }
    
    /// <summary>
    /// Resources 폴더에서 튜토리얼용 기본 설비 배치 파일 불러오기
    /// </summary>
    public static List<StationSaveInfo> LoadTutorialStationData()
    {
        string path = StringPath.STATION_DEFAULT_SAVE_PATH;
        TextAsset jsonFile = Resources.Load<TextAsset>(path);

        if (!jsonFile)
        {
            if (Instance?.showDebugInfo == true) Debug.LogError($"[SaveLoadManager] 튜토리얼 설비 파일을 찾을 수 없음: Resources/{path}.json");
            return null;
        }

        try
        {
            var infos = JsonConvert.DeserializeObject<List<StationSaveInfo>>(jsonFile.text);
            if (Instance?.showDebugInfo == true)
                Debug.Log($"[SaveLoadManager] 튜토리얼 설비 파일 로드 완료: {infos.Count}개 항목");
            return infos;
        }
        catch (System.Exception e)
        {
            if (Instance?.showDebugInfo == true) Debug.LogError($"[SaveLoadManager] 튜토리얼 설비 파일 파싱 실패: {e.Message}");
            return null;
        }
    }

    public static void RestoreStationState(GamePhase currentPhase)
    {
        var infos = LoadStationData(); // station.json 불러오기
        if (infos == null || infos.Count == 0)
        {
            if (Instance?.showDebugInfo == true)
                Debug.LogWarning("[SaveLoadManager] station.json 로드 실패 또는 내용 없음");
            return;
        }

        StationManager.Instance.RestoreStations(infos, currentPhase); // StationManager에게 전달
    }
}
