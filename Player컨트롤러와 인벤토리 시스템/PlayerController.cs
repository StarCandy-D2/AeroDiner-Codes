using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : Singleton<PlayerController>
{
    #region 참조
    
    [Header("입력 액션 참조")]
    public InputActionReference moveActionRef;
    public InputActionReference interactActionRef;
    public InputActionReference pickupActionRef;

    [Header("이동 속도")]
    public float moveSpeed = 5f;

    [Header("상호작용 설정")]
    [SerializeField] private Vector2 interactionBoxSize = new(0.8f, 0.5f);
    public float interactionRadius = 1.5f;
    public LayerMask interactableLayer;
    

    [SerializeField] private PlayerInventory playerInventory;
    [SerializeField] private Transform itemSlotTransform;
    [SerializeField] private Vector2 slotOffsetUp = new(0, 0.5f);
    [SerializeField] private Vector2 slotOffsetDown = new(0, -0.5f);
    [SerializeField] private Vector2 slotOffsetLeft = new(-0.5f, 0);
    [SerializeField] private Vector2 slotOffsetRight = new(0.5f, 0);
    [SerializeField] private AudioSource moveSFXSource;
    [SerializeField] private AudioClip moveSFXClip;

    public InteractionType interactionType;
    
    private Rigidbody2D rb;
    private Animator animator;

    private float idleTimer;
    private Vector2 moveInput;
    private Vector2 lastMoveDir = Vector2.down;
    
    private IInteractable currentTarget;
    private TilemapController tilemapController;

    private string lastWallMessage;
    private bool isTouchingWall;

    // const
    private const float IDLE_BREAK_TIME = 5f;
    
    // animation hash
    private static readonly int PutDown = Animator.StringToHash("PutDown");
    private static readonly int PickUp = Animator.StringToHash("PickUp");
    private static readonly int MoveX = Animator.StringToHash("MoveX");
    private static readonly int MoveY = Animator.StringToHash("MoveY");
    private static readonly int IsMoving = Animator.StringToHash("IsMoving");
    private static readonly int LastMoveX = Animator.StringToHash("LastMoveX");
    private static readonly int LastMoveY = Animator.StringToHash("LastMoveY");
    private static readonly int IsCarrying = Animator.StringToHash("IsCarrying");
    private static readonly int IsInteract = Animator.StringToHash("IsInteract");
    private static readonly int IdleBreakIndex = Animator.StringToHash("IdleBreakIndex");
    private static readonly int TriggerIdleBreak = Animator.StringToHash("TriggerIdleBreak");
    
    #endregion
    
    #region 외부 접근용 Getter

    public bool IsHoldingFood(string id) => GetHeldFoodID() == id;
    public bool IsHoldingStation(string id) => GetHeldStationID() == id;
    
    private string GetHeldFoodID() => playerInventory?.HoldingFood?.foodData?.id;
    private string GetHeldStationID() => playerInventory?.HoldingFood?.foodData?.id;

    #endregion
    
    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
        playerInventory = GetComponent<PlayerInventory>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        tilemapController = FindObjectOfType<TilemapController>();
    }

    private void OnEnable()
    {
        moveActionRef.action.Enable();
        interactActionRef.action.Enable();
        pickupActionRef.action.Enable();

        interactActionRef.action.performed += OnInteract;
        pickupActionRef.action.performed += OnPickupDown;
        interactActionRef.action.canceled += OnInteractCancel;
    }

    private void OnDisable()
    {
        moveActionRef.action.Disable();
        interactActionRef.action.Disable();
        pickupActionRef.action.Disable();

        interactActionRef.action.performed -= OnInteract;
        pickupActionRef.action.performed -= OnPickupDown;
        interactActionRef.action.canceled -= OnInteractCancel;
    }

    private void Update()
    {
        // 플레이어 움직임 가능 Phase 설정
        bool canMove = GameManager.Instance.CurrentPhase 
            is GamePhase.EditStation 
            or GamePhase.Day 
            or GamePhase.Operation 
            or GamePhase.Closing;
        
        moveInput = canMove ? moveActionRef.action.ReadValue<Vector2>() : Vector2.zero;

        Animate();
        UpdateItemSlotPosition();
        RaycastForInteractable();

        animator.SetBool(IsCarrying, playerInventory.IsHoldingItem);

        bool isHolding = interactActionRef.action.IsPressed();
        bool isInteracting = currentTarget != null && interactionType == InteractionType.Use && isHolding;
        animator.SetBool(IsInteract, isInteracting);

        if (isInteracting)
        {
            currentTarget.Interact(playerInventory, interactionType);
        }

        if (moveInput.sqrMagnitude < 0.01f)
        {
            idleTimer += Time.deltaTime;
            if (idleTimer >= IDLE_BREAK_TIME)
            {
                int random = Random.Range(0, 2);
                animator.SetInteger(IdleBreakIndex, random);
                animator.SetTrigger(TriggerIdleBreak);
                idleTimer = 0f;
            }
        }
        else
        {
            idleTimer = 0f;
        }
    }
    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveInput * (moveSpeed * Time.fixedDeltaTime));
    }
    
    #region 상호작용부분
    
    private void OnInteract(InputAction.CallbackContext context)
    {
        var target = FindBestInteractable(InteractionType.Use);
        target?.Interact(playerInventory, InteractionType.Use);
    }

    private void OnPickupDown(InputAction.CallbackContext context)
    {
        if (!context.performed || playerInventory == null) return;

        if (playerInventory.IsHoldingItem)
        {
            if (currentTarget != null)
            {
                bool wasHoldingStation = playerInventory.HoldingStation != null;

                SetDirectionParams();
                animator.SetTrigger(PutDown);
                playerInventory.DropItem(currentTarget);
                EventBus.OnSFXRequested(SFXType.ItemLaydown);
                
                if (wasHoldingStation)
                {
                    EventBus.Raise(GameEventType.StationLayoutChanged);
                }
            }
        }
        else
        {
            var pickupTarget = FindBestInteractable(InteractionType.Pickup);

            if (pickupTarget != null)
            {
                if (pickupTarget is FoodDisplay food)
                {
                    if (food.originPlace is BaseStation station && station.IsCookingOrWaiting)
                    {
                        EventBus.Raise(UIEventType.ShowIngredientWarn);
                        return;
                    }
                }

                SetDirectionParams();
                animator.SetTrigger(PickUp);
                playerInventory.TryPickup(pickupTarget);
                EventBus.OnSFXRequested(SFXType.ItemPickup);
            }
        }
    }

    private void RaycastForInteractable()
    {
        var hits = Physics2D.BoxCastAll(
            transform.position, 
            interactionBoxSize, 
            0f, 
            lastMoveDir, 
            interactionRadius, 
            interactableLayer
        );

        IInteractable stationTarget = null;
        IInteractable gridTarget = null;
        float closestStationDist = Mathf.Infinity;
        float closestGridDist = Mathf.Infinity;
        
        foreach (var hit in hits)
        {
            var interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable == null) continue;

            float dist = Vector2.Distance(transform.position, hit.transform.position);

            if (hit.collider.CompareTag(StringTag.STATION_TAG))
            {
                if (dist < closestStationDist)
                {
                    stationTarget = interactable;
                    closestStationDist = dist;
                }
            }
            else if (hit.collider.CompareTag(StringTag.GRID_CELL_TAG))
            {
                if (dist < closestGridDist)
                {
                    gridTarget = interactable;
                    closestGridDist = dist;
                }
            }
        }
        
        bool holdingStation = playerInventory.HoldingStation != null;
        IInteractable newTarget = holdingStation ? gridTarget ?? stationTarget 
            : stationTarget ?? gridTarget;
        
        if (newTarget != currentTarget)
        {
            currentTarget?.OnHoverExit();
            newTarget?.OnHoverEnter();
            currentTarget = newTarget;

            if (currentTarget is GridCellStatus gridCell)
            {
                tilemapController.HighlightSelectedCell(gridCell.gameObject);
            }
            else
            {
                tilemapController.ClearSelection();
            }
        }
    }

    private IInteractable FindBestInteractable(InteractionType type)
    {
        var hits = CastAll(transform.position, lastMoveDir, interactionRadius, interactableLayer);
        IInteractable best = null;
        float closestDist = Mathf.Infinity;

        foreach (var hit in hits)
        {
            if (hit.collider == null) continue;
            var interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable == null) continue;

            float dist = Vector2.Distance(transform.position, hit.point);

            if (type == InteractionType.Pickup)
            {
                switch (interactable)
                {
                    case IMovableStation when GameManager.Instance.CurrentPhase == GamePhase.EditStation:
                    {
                        if (dist < closestDist)
                        {
                            best = interactable;
                            closestDist = dist;
                        }
                        continue;
                    }
                    
                    case FoodDisplay food when food.CanPickup():
                    case IngredientStation:
                    {
                        if (dist < closestDist)
                        {
                            best = interactable;
                            closestDist = dist;
                        }

                        break;
                    }
                }
            }
            
            
        }
        return best;
    }

    private RaycastHit2D[] CastAll(Vector2 origin, Vector2 direction, float distance, LayerMask layer)
    {
        return Physics2D.RaycastAll(origin, direction, distance, layer);
    }
    #endregion
    
    #region 애니메이션 부분
    private void Animate()
    {
        if (moveInput.sqrMagnitude > 0.01f)
        {
            animator.SetFloat(MoveX, moveInput.x);
            animator.SetFloat(MoveY, moveInput.y);
            animator.SetBool(IsMoving, true);

            lastMoveDir = moveInput.normalized;
            animator.SetFloat(LastMoveX, lastMoveDir.x);
            animator.SetFloat(LastMoveY, lastMoveDir.y);

            if (!moveSFXSource.isPlaying)
            {
                moveSFXSource.clip = moveSFXClip;
                moveSFXSource.loop = true;
                moveSFXSource.Play();
            }
        }
        else
        {
            animator.SetBool(IsMoving, false);
            if (moveSFXSource.isPlaying)
            {
                moveSFXSource.Stop();
            }
        }
    }

    private void UpdateItemSlotPosition()
    {
        if (!itemSlotTransform) return;

        Vector2 dir = lastMoveDir;
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        {
            itemSlotTransform.localPosition = dir.x > 0 ? slotOffsetRight : slotOffsetLeft;
        }
        else
        {
            itemSlotTransform.localPosition = dir.y > 0 ? slotOffsetUp : slotOffsetDown;
        }
    }

    private void SetDirectionParams()
    {
        if (lastMoveDir == Vector2.zero)
            lastMoveDir = Vector2.down;

        animator.SetFloat(LastMoveX, lastMoveDir.x);
        animator.SetFloat(LastMoveY, lastMoveDir.y);
    }

    private void OnInteractCancel(InputAction.CallbackContext context)
    {
        currentTarget?.Interact(playerInventory, InteractionType.Stop);
    }
    #endregion

    #region 투명벽 상호작용
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(StringTag.INVISIBLE_WALL_TAG))
        {
            isTouchingWall = true;

            string msg = GetWallPopupMessage();
            lastWallMessage = msg;

            EventBus.Raise(UIEventType.ShowWallPopup, msg);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag(StringTag.INVISIBLE_WALL_TAG) && !isTouchingWall)
        {
            isTouchingWall = true;

            string msg = GetWallPopupMessage();
            if (msg != lastWallMessage)
            {
                lastWallMessage = msg;
                EventBus.Raise(UIEventType.ShowWallPopup, msg);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(StringTag.INVISIBLE_WALL_TAG))
        {
            isTouchingWall = false;
            EventBus.Raise(UIEventType.HideWallPopup);
        }
    }

    private string GetWallPopupMessage()
    {
        if (GameManager.Instance.CurrentPhase == GamePhase.Operation)
            return StringMessage.OPERATION_ALERT;
        
        if (playerInventory.HoldingStation != null)
            return StringMessage.STATION_ALERT;
        
        return StringMessage.ESCAPE_ALERT;
    }
    #endregion
    
    #region Gizmo
    private void OnDrawGizmosSelected()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying) return;

        Vector2 origin = rb.position;
        Vector2 direction = lastMoveDir == Vector2.zero ? Vector2.down : lastMoveDir;
        int rayCount = 5;
        float spread = 0.3f;
        float distance = interactionRadius;

        Vector2 perpendicular = new Vector2(-direction.y, direction.x);

        for (int i = 0; i < rayCount; i++)
        {
            float t = (i / (float)(rayCount - 1)) - 0.5f;
            Vector2 offset = perpendicular * t * spread;
            Vector2 rayOrigin = origin + offset;

            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(rayOrigin, rayOrigin + direction.normalized * distance);
        }
#endif
    }
    #endregion
}
