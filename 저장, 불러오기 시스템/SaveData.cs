using System;
using System.Collections.Generic;

[Serializable]
public class SaveData
{
    //  날짜 및 진행 정보
    public int currentDay;
    public int totalEarnings;

    //  메뉴, 설비 관련 정보
    public HashSet<string> menuDatabase = new();         // 해금된 메뉴 ID들 (중복 방지용)
    public HashSet<string> stationDatabase = new();         // 해금된 스테이션 ID들 (중복 방지용)
    
    // 퀘스트 상태 저장
    public List<string> playerQuestStatusKeys = new();
    public List<QuestStatus> playerQuestStatusValues = new();

    // 퀘스트 진행도 저장
    public List<SerializableQuestProgress> playerQuestProgress = new();

    //  설정 정보 (기존 PlayerPrefs → 통합 저장)
    public float bgmVolume = 0.5f;
    public float sfxVolume = 0.5f;

    //  키 바인딩 (Newtonsoft.Json은 Dictionary도 가능)
    public Dictionary<string, string> keyBindings = new();   // ex: "MoveUp" → "W"

    // 화면 옵션
    public int screenModeIndex = 2;        // 기본값: 전체화면
    public int resolutionIndex = 0;        // 기본값: 목록 첫 번째

    public class StationSaveInfo
    {
        public string id;
        public string gridCellName;
    }

    public List<string> unlockedStationIds = new();
}

/// <summary>
/// 퀘스트 진행상황용
/// </summary>
[Serializable]
public class SerializableQuestProgress
{
    public string questId;
    public List<string> objectiveTargetIds = new List<string>();
    public List<int> objectiveCurrentAmounts = new List<int>();
}
