using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class SmoglingDamageZone : MonoBehaviour {
    [Header("References")]
    [SerializeField] private SmoglingCombat smogling;

    private bool damagePending;

    private void Awake() {
        if (smogling == null) {
            smogling = GetComponentInParent<SmoglingCombat>();
        }

        if (smogling == null) {
            Debug.LogError($"{name}: no SmoglingCombat component was found.", this);

            enabled = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (damagePending || smogling == null || smogling.Defeated) {
            return;
        }

        PlayerHealth player = collision.collider.GetComponentInParent<PlayerHealth>();

        if (player == null || player.DamageSequenceRunning) {
            return;
        }

        damagePending = true;

        StartCoroutine(DamageAfterStompCheck(player));
    }

    private IEnumerator DamageAfterStompCheck(PlayerHealth player) {
        yield return new WaitForFixedUpdate();

        damagePending = false;

        if (smogling == null || smogling.Defeated) {
            yield break;
        }

        if (player == null || player.DamageSequenceRunning) {
            yield break;
        }

        smogling.DamagePlayer(player);
    }
}