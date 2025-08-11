using System;
using System.Collections.Generic;
using UnityEngine;

public enum FadeEventType
{
    FadeIn,                 // 밝게
    FadeOut,                // 어둡게
    FadeTo,                 // 지정 알파값으로
    FadeOutAndLoadScene,    // 로딩씬을 거쳐서
    FadeToSceneDirect       // 바로 해당 씬으로
}
public enum BGMEventType
{
    PlayMainTheme,
    PlayResultTheme,
    PlayStartMenu,
    PlayRecipeChoice,
    PlayLifeTheme,
    StopBGM,
    Intro1, Intro2, Intro3, Title
}

public enum GameEventType
{
    GamePhaseChanged, // 게임 상태 변경
    
    RoundTimerEnded,  // 영업 시간 종료
    AllCustomersLeft,
    DialogueEnded,
    QuestStatusChanged,
    
    NoMoreStoriesInPhase,
    
    // 튜토리얼용
    PlayerPickedUpItem,
    StationUsed,
    CustomerServed,
    StationLayoutChanged,
    UISceneReady,
}

public enum SFXType
{
    // UI
    PressAnyKey, ButtonClick, BlankClick,

    // 플레이어
    PlayerMove, ItemPickup, ItemLaydown,
    
    // 손님
    CustomerOrder, CustomerAngry,
    CustomerServe, CustomerPay,
    
    // 요리
    DoneCooking, ThrowAway,
    Pot, Oven,
    CuttingBoard, Grinding,
    Grater, FryingPan,
    AutoCutter, Blender,
    AutoGrater, RollPan,

    // 수동 조리
    PlayCooking, StopCook,

    // 일상
    NPCScript, BuyInShop,
    
    //추후 추가(리스트 번호 오류 방지)
    OpenPause, ClosePause,
    OpenBook, OpenInventory,Rain, Sunder, Crash1, Crash2,
}
public enum UIEventType
{
    //pause, Option
    OpenPause, ClosePause,
    OpenOption, CloseOption,
    CloseSound, CloseVideo,
    CloseControl, ShowSoundTab,
    ShowVideoTab, ShowControlTab,
    OnClickNewGame,
    
    //StartScene
    ShowStartWarningPanel, ShowStartMenuWithSave,
    ShowStartMenuNoSave, LoadMainScene,
    QuitGame, ShowPressAnyKey,LoadDayScene,
    LoadIntroScene,
    //MainSceneUI
    ShowRoundTimer, HideRoundTimer,
    UpdateTotalEarnings, UpdateTodayEarnings,
    ShowMenuPanel, UpdateMenuPanel, HideMenuPanel, 
    ShowResultPanel, HideResultPanel, 
    ShowOrderPanel, HideOrderPanel,
    ShowWallPopup, HideWallPopup,
    ShowIngredientWarn,

    //Inventory
    ShowInventory, HideInventory,
    ShowRecipeBook, ShowStationPanel,
    ShowQuestPanel,
    FadeInInventory, FadeInRecipeBook,

    //store
    FadeInStore, FadeOutStore,
    ShowStationStore, ShowRecipeStore,
    
    //Dialogue
    ShowDialogueLine, HideDialoguePanel,
    ShowDialoguePanel,
    
    //Tutorial
    tu1, tu2, tu3, tu3_step2, tu3_step3, tu3_step4, tu3_step5, tu3_step6, tu3_step7, tu3_stop,
    tu4, tu5, tu6, tu7, tu8, tu9, tu8_stop, ShowDemoEnd
}

public static class EventBus
{
    public static Action<SFXType> OnSFXRequested;
    public static Action<BGMEventType> OnBGMRequested;
    public static Action<UIEventType, object> OnUIEvent;
    public static Action<GameEventType, object> OnGameEvent;
    public static event Action<FadeEventType, FadeEventPayload> OnFadeRequested;
    private static AudioSource cookingLoopSource;
    public static event Action<SFXType> OnLoopSFXRequested;
    public static event Action<SFXType> OnStopLoopSFXRequested;
    private static Dictionary<GameEventType, Action<object>> gameEventListeners = new();

    public static void PlaySFX(SFXType type)
    {
        //Debug.Log($"[SFX DEBUG] 요청된 SFXType: {type} | 호출 스택:\n{Environment.StackTrace}");
        OnSFXRequested?.Invoke(type);
    }
    
    public static void Raise(UIEventType eventType, object payload = null)
    {
        OnUIEvent?.Invoke(eventType, payload);
    }

    public static void Raise(GameEventType eventType, object payload = null)
    {
        OnGameEvent?.Invoke(eventType, payload);

        if (gameEventListeners.TryGetValue(eventType, out var callback))
        {
            callback.Invoke(payload);
        }
    }

    public static void PlayBGM(BGMEventType type)
    {
        OnBGMRequested?.Invoke(type);
    }
    
    public static void RaiseFadeEvent(FadeEventType type, FadeEventPayload payload = null)
    {
        OnFadeRequested?.Invoke(type, payload);
    }
    
    //게임 종료 시점 혹은 씬 변경 시점에 호출하여 메모리 누수 방지
    public static void ClearAll()
    {
        OnSFXRequested = null;
        OnUIEvent = null;
    }

    public static void PlayLoopSFX(SFXType type)
    {
        OnLoopSFXRequested?.Invoke(type);
    }

    public static void StopLoopSFX(SFXType type)
    {
        OnStopLoopSFXRequested?.Invoke(type);
    }

    public static void Register(GameEventType eventType, Action<object> listener)
    {
        if (!gameEventListeners.ContainsKey(eventType))
        {
            gameEventListeners[eventType] = delegate { };
        }
        gameEventListeners[eventType] += listener;
    }

    public static void Unregister(GameEventType eventType, Action<object> listener)
    {
        if (gameEventListeners.ContainsKey(eventType))
        {
            gameEventListeners[eventType] -= listener;
        }
    }
}