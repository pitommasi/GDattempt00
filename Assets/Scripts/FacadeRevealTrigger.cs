using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class FacadeRevealTrigger : MonoBehaviour {
    [SerializeField]
    private AreaFacade _facadeToReveal;

    private void Awake() {
        Collider2D triggerCollider =
            GetComponent<Collider2D>();

        if (!triggerCollider.isTrigger) {
            Debug.LogWarning(
                $"{name}: enable Is Trigger on the reveal collider.",
                this
            );
        }

        if (_facadeToReveal == null) {
            Debug.LogError(
                $"{name}: assign the facade to reveal.",
                this
            );

            enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        PlayerHealth player =
            other.GetComponentInParent<PlayerHealth>();

        if (player == null) {
            return;
        }

        _facadeToReveal.Reveal();
        gameObject.SetActive(false);
    }
}