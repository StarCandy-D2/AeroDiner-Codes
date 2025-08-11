## 💾 저장·불러오기 시스템 — SaveLoadManager & SaveData

---

🌟 **요구 사항**

- 플레이어 진행 상황, 설정값, 게임 상태를 **JSON 형식**으로 저장하고 불러오기.
- 자동 저장(하루 종료 시)과 수동 저장(Start Scene 메뉴에서 Load 선택)을 지원.
- PlayerPrefs보다 **유연하고 확장 가능한 구조**로 설계.
- 단일 저장 파일 유지 — New Game 시 기존 저장 데이터 삭제.

---

✅ **구현 방법 — SaveLoadManager(직렬화·파일 I/O) + SaveData(저장 데이터 구조)**

### 1. SaveData
- **저장 필드**
  - `currentDay` — 현재 일차
  - `totalEarnings` — 누적 수익
  - `unlockedMenuIds` — 해금된 메뉴 ID 리스트
  - `bgmVolume`, `sfxVolume` — 설정 볼륨 값
  - `keyBindings` — 사용자 키 설정
  - `isTutorialCompleted` — 튜토리얼 완료 여부
- **Newtonsoft.Json 기반 직렬화**
  - Dictionary, List, 복합 구조를 지원.
  - Unity 기본 JsonUtility보다 확장성 뛰어남.

---

### 2. SaveLoadManager
- **저장**
  - SaveData 객체를 JSON 문자열로 변환 후 Application.persistentDataPath 내 저장.
  - 하루 종료 시 자동 저장(`RestaurantManager`에서 호출).
  - New Game 선택 시 기존 파일 삭제.
- **불러오기**
  - 저장 파일이 존재하면 JSON → SaveData 역직렬화.
  - 없으면 기본 SaveData 생성.
- **연동**
  - GameManager, QuestManager, StoryManager, StationManager 등 각 매니저에서 자신의 상태를 SaveData에 반영.
  - UI 옵션 패널에서 즉시 저장 가능.
- **보안**
  - 파일 입출력 예외 처리 및 무결성 체크.
  - 잘못된 데이터 로드시 기본값으로 초기화.

---

### 3. 설계 특징
- **단일 진입점 관리**: SaveLoadManager가 저장/불러오기 로직을 전담.
- **확장성 높은 데이터 구조**: 새 필드 추가 시 SaveData에만 정의하면 바로 저장 가능.
- **자동·수동 저장 지원**: 플레이어 편의성 향상.
- **플랫폼 호환성**: Application.persistentDataPath 사용으로 PC/모바일 모두 지원.

---

### 4. 대표 시나리오

**자동 저장**
```csharp
// 하루 종료 시
SaveData data = GatherCurrentGameState();
SaveLoadManager.SaveGame(data);
```
**불러오기**
```csharp
SaveData data = SaveLoadManager.LoadGame();
if (data != null) {
    ApplyGameState(data);
}
```
**New Game**
```csharp
SaveLoadManager.DeleteSaveFile();
GameManager.StartNewGame();
```
