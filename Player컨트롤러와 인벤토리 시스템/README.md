🕹 플레이어 제어 & 인벤토리 시스템 — PlayerController & PlayerInventory
🌟 요구 사항

플레이어가 4방향 이동 및 **상호작용(사용/픽업/드롭)**을 자연스럽게 수행.

현재 GamePhase에 따라 이동·픽업·드롭 가능 여부를 제어.

애니메이션, SFX, UI 경고, 타일맵 하이라이트 등을 상호작용 흐름에 통합.

손에 든 아이템(음식/설비)에 따라 아이템 슬롯 위치·애니메이션을 변경.

✅ 구현 방법 — PlayerController(입력·상호작용) + PlayerInventory(아이템 관리)

1. PlayerController
입력 처리

InputActionReference(move/interact/pickup)로 입력을 읽음.

OnEnable/OnDisable에서 액션 활성/비활성 및 콜백 등록.

이동 제어

GamePhase가 EditStation / Day / Operation / Closing일 때만 이동 가능.

Rigidbody2D로 FixedUpdate에서 위치 갱신.

상호작용 로직

RaycastForInteractable()로 플레이어 전방 BoxCast → IInteractable 타겟 결정.

설비 들고 있으면 GridCell 우선, 아니면 Station 우선.

OnInteract() — 현재 방향 최적 타겟과 InteractionType.Use 상호작용.

OnPickupDown() — 손에 아이템 없으면 픽업, 있으면 드롭. 조건별 애니메이션·SFX·이벤트 호출.

애니메이션 & SFX

이동 방향, 마지막 방향, IsCarrying, IsInteract 등 Animator 파라미터 업데이트.

Idle 상태 일정 시간 유지 시 랜덤 IdleBreak 트리거.

이동 시 발소리 SFX 반복 재생, 정지 시 정지.

UI/환경 상호작용

투명벽(Collider Tag: INVISIBLE_WALL_TAG) 충돌 시 상황별 메시지 UIEvent로 표시 (ShowWallPopup / HideWallPopup).

TilemapController를 통해 선택 셀 하이라이트.

2. PlayerInventory
보유 상태

HoldingFood (FoodDisplay), HoldingStation (IMovableStation)로 현재 들고 있는 아이템 추적.

IsHoldingItem 플래그로 보유 여부 확인.

픽업

EditStation Phase → 설비 픽업 (HandleStationPickup)

부모를 itemSlot으로 변경, Rigidbody/Collider 비활성화, 모든 GridCell 표시.

Operation/Closing Phase → 재료/음식 픽업 (HandleFoodPickup)

부모를 itemSlot으로 변경, 물리/충돌 비활성화, originPlace 콜백 호출.

드롭

EditStation Phase → GridCell에 설비 드롭 (HandleStationDrop → PlacementManager 배치 시도).

Operation/Closing Phase → 음식/재료 드롭 (HandleFoodDrop)

대상 Station/Shelf/Table에서 CanPlaceIngredient 검사.

성공 시 아이템 파괴(ConsumeHeldItem) 및 인벤토리 비우기.

이벤트 연동

픽업/드롭 시 EventBus.Raise(GameEventType.PlayerPickedUpItem, data) 호출.

드롭 성공 후 Station 레이아웃 변경 이벤트 전송.

디버그

showDebugInfo 활성화 시 부적절한 동작이나 거부 사유 로그 출력.

3. 설계 특징
Phase 기반 제어: 이동, 픽업, 드롭 모두 GamePhase 조건에 따라 허용/차단.

상호작용 우선순위: 들고 있는 아이템 종류(Station/Food)에 따라 타겟 우선순위 변경.

물리 안전성: 픽업 시 Rigidbody2D와 Collider 비활성화로 충돌 방지.

UI·SFX 연계: 행동마다 UI 경고, 사운드, 애니메이션 동기화.

타일맵 연동: EditStation Phase에서 GridCell 시각화로 배치 가이드 제공.

4. 대표 시나리오
재료 픽업

csharp
복사
편집
// Operation Phase
var pickupTarget = FindBestInteractable(InteractionType.Pickup);
playerInventory.TryPickup(pickupTarget);
// => itemSlot에 배치, 물리/충돌 끔, EventBus로 픽업 이벤트
설비 배치

csharp
복사
편집
// EditStation Phase
var gridCell = FindBestInteractable(InteractionType.Pickup) as GridCellStatus;
playerInventory.DropItem(gridCell);
// => PlacementManager.TryPlaceStationAt 호출, 성공 시 손에서 제거
투명벽 경고

csharp
복사
편집
// Operation Phase 중 벽에 부딪힘
EventBus.Raise(UIEventType.ShowWallPopup, "영업 중엔 나갈 수 없습니다!");
