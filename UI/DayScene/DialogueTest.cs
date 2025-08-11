using UnityEngine;

public class DialogueTest : MonoBehaviour
{
    [Header("트리거 대사 데이터")]
    [SerializeField] private DialogueData dialogueToPlay;

    [Header("플레이어 감지 범위")]
    [SerializeField] private float triggerRadius = 2f;

    [Header("상호작용 키")]
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    private Transform player;

    private bool isPlayerInRange => player && Vector3.Distance(player.position, transform.position) <= triggerRadius;

    private void Start()
    {
        player = GameObject.FindWithTag("Player")?.transform;
        if (!player) Debug.LogWarning("[DialogueTrigger] Player 오브젝트를 찾을 수 없음 (Tag 필요)");
    }

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(interactKey))
        {
            EventBus.Raise(UIEventType.FadeInStore);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, triggerRadius);
    }
}
