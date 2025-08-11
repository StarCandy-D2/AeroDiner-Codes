using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections.Generic;

public class UIManager : Singleton<UIManager>
{
    //이 리스트에 있는 UI는 모두 비활성화 상태로 시작.
    private readonly System.Type[] initiallyDisabledTypes = {
        typeof(ResultPanel),
        typeof(MenuPanel3),
        typeof(MenuPanel4),
        typeof(Inventory),
        typeof(MenuPanel),
        typeof(DialogueUI),
        typeof(Store),
        typeof(Tu1),
        typeof(Tu2),
        typeof(Tu3),
        typeof(Tu4),
        typeof(Tu5),
        typeof(Tu6),
        typeof(Tu7),
        typeof(Tu8),
        typeof(DemoEnd),
        typeof(IngredientWarnPopup),

        
        // 필요한 타입 추가 가능
    };

    private List<IUIEventHandler> uiHandlers = new();
    [System.Serializable]
    public class SceneUIEntry
    {
        public string sceneName;
        public List<AssetReferenceGameObject> uiPrefabs;
    }
    [Header("Debug")]
    [SerializeField] private bool showDebugInfo;

    [Header("씬별 UI 매핑")]
    [SerializeField] private List<SceneUIEntry> sceneUIPrefabs;

    private Dictionary<string, List<AssetReferenceGameObject>> uiMap;
    private List<GameObject> currentSceneUIs = new();
    public List<GameObject> CurrentSceneUIs => currentSceneUIs;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);

        uiMap = new();
        foreach (var entry in sceneUIPrefabs)
        {
            if (!uiMap.ContainsKey(entry.sceneName))
                uiMap[entry.sceneName] = new();
            uiMap[entry.sceneName].AddRange(entry.uiPrefabs);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        EventBus.OnUIEvent += HandleUIEvent;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        EventBus.OnUIEvent -= HandleUIEvent;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        LoadSceneUI(scene.name);

    }
    
    private void LoadSceneUI(string sceneName)
    {
        StartCoroutine(LoadSceneUICoroutine(sceneName));
    }

    private IEnumerator LoadSceneUICoroutine(string sceneName)
    {
        // 기존 UI 제거
        foreach (var ui in currentSceneUIs)
        {
            if (ui != null)
                Destroy(ui);
        }
        currentSceneUIs.Clear();

        if (!uiMap.TryGetValue(sceneName, out var assetRefs))
        {
            if (showDebugInfo)
                Debug.LogWarning($"[UIManager] UI 프리팹을 찾을 수 없음: {sceneName}");
            yield break; // 코루틴 종료
        }

        // 프리팹 비어있을 경우 경고 로그 (선택)
        if (assetRefs.Count == 0 && showDebugInfo)
            Debug.LogWarning($"[UIManager] {sceneName} 씬에 로드할 UI 프리팹이 없습니다.");

        // Addressable 기반 UI 인스턴스 생성
        foreach (var assetRef in assetRefs)
        {
            var handle = assetRef.InstantiateAsync(transform);
            yield return handle;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                currentSceneUIs.Add(handle.Result);
            }
            else if (showDebugInfo)
            {
                Debug.LogError($"[UIManager] UI 로딩 실패: {assetRef.RuntimeKey}");
            }
        }

        // 특정 UI는 시작 시 비활성화
        foreach (var ui in currentSceneUIs)
        {
            foreach (var type in initiallyDisabledTypes)
            {
                var target = ui.GetComponentInChildren(type, true) as MonoBehaviour;
                if (target != null)
                    target.gameObject.SetActive(false);
            }
        }

        if (showDebugInfo)
            Debug.Log($"[UIManager] {sceneName} 씬 UI 로딩 완료, 프리팹 수: {assetRefs.Count}");

        RegisterHandlersForScene(sceneName); // 중복 제거
        EventBus.Raise(GameEventType.UISceneReady);
    }

    private void RegisterHandlersForScene(string sceneName)
    {
        uiHandlers.Clear();

        // 공통 핸들러 (항상 등록)
        uiHandlers.Add(new OverSceneUIHandler(currentSceneUIs));
        uiHandlers.Add(new TutorialUIHandler(currentSceneUIs));

        switch (sceneName)
        {
            case StringScene.START_SCENE:
                uiHandlers.Add(new StartSceneUIHandler(currentSceneUIs));
                break;
            
            case StringScene.MAIN_SCENE:
                uiHandlers.Add(new MainSceneUIHandler(currentSceneUIs));
                break;
            case StringScene.DAY_SCENE:
                uiHandlers.Add(new DaySceneUIHandler(currentSceneUIs));
                break;
        }

        // 필요 시 다른 씬별 핸들러도 추가
    }
    private void HandleUIEvent(UIEventType type, object payload)
    {
        foreach (var handler in uiHandlers)
        {
            if (handler.Handle(type, payload))
                break; // 이벤트 처리 완료된 핸들러 있으면 종료
        }
    }
}