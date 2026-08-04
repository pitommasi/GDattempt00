using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class SmoglingStompZone : MonoBehaviour {
    [Header("Stomp")]
    [Min(0f)]
    [SerializeField] private float minimumDownwardSpeed = 0.1f;

    [Header("References")]
    [SerializeField] private SmoglingCombat smogling;

    private Collider2D stompCollider;

    private void Awake() {
        stompCollider = GetComponent<Collider2D>();

        if (!stompCollider.isTrigger) {
            Debug.LogWarning(
                $"{name}: enable Is Trigger on the Stomp Zone collider.",
                this
            );
        }

        if (smogling == null) {
            smogling = GetComponentInParent<SmoglingCombat>();
        }

        if (smogling == null) {
            Debug.LogError(
                $"{name}: no SmoglingCombat component was found.",
                this
            );

            enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (smogling == null || smogling.Defeated) {
            return;
        }

        PlayerHealth player =
            other.GetComponentInParent<PlayerHealth>();

        if (player == null || player.DamageSequenceRunning) {
            return;
        }

        Rigidbody2D playerBody =
            player.GetComponent<Rigidbody2D>();

        if (playerBody == null) {
            return;
        }

        if (
            playerBody.linearVelocity.y >
            -minimumDownwardSpeed
        ) {
            return;
        }

        smogling.DefeatByStomp(player);
    }
}