using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class SmoglingCombat : MonoBehaviour {
    [Header("Stomp")]
    [SerializeField] private float playerBounceSpeed = 7f;

    [Header("References")]
    [SerializeField] private SmoglingPatrol patrol;
    [SerializeField] private Collider2D damageCollider;
    [SerializeField] private Collider2D stompZone;

    private Rigidbody2D body;
    private bool defeated;

    public bool Defeated => defeated;

    private void Awake() {
        body = GetComponent<Rigidbody2D>();

        if (patrol == null) {
            patrol = GetComponent<SmoglingPatrol>();
        }
    }

    public void DamagePlayer(PlayerHealth player) {
        if (defeated || player == null) {
            return;
        }

        if (player.DamageSequenceRunning) {
            return;
        }

        player.TakeDamage();
    }

    public void DefeatByStomp(PlayerHealth player) {
        if (defeated) {
            return;
        }

        defeated = true;

        if (patrol != null) {
            patrol.enabled = false;
        }

        if (damageCollider != null) {
            damageCollider.enabled = false;
        }

        if (stompZone != null) {
            stompZone.enabled = false;
        }

        body.linearVelocity = Vector2.zero;
        body.simulated = false;

        if (player != null) {
            player.Bounce(playerBounceSpeed);
        }

        Destroy(gameObject);
    }
}