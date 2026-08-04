using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider2D))]
public abstract class InteractableBase : MonoBehaviour {
    [Header("Interaction prompt")]
    [SerializeField] private GameObject _interactionPrompt;

    private PlayerHealth _playerInRange;
    private bool _interactionEnabled = true;

    protected virtual void Awake() {
        Collider2D interactionCollider =
            GetComponent<Collider2D>();

        if (!interactionCollider.isTrigger) {
            Debug.LogWarning(
                $"{name}: enable Is Trigger on the interaction collider.",
                this
            );
        }

        SetPromptVisible(false);
    }

    private void Update() {
        if (
            !_interactionEnabled ||
            _playerInRange == null ||
            Keyboard.current == null ||
            Time.timeScale == 0f
        ) {
            return;
        }

        bool interactionPressed =
            Keyboard.current.eKey.wasPressedThisFrame ||
            Keyboard.current.fKey.wasPressedThisFrame;

        if (interactionPressed) {
            Interact(_playerInRange);
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (!_interactionEnabled) {
            return;
        }

        PlayerHealth player =
            other.GetComponentInParent<PlayerHealth>();

        if (player == null) {
            return;
        }

        _playerInRange = player;
        SetPromptVisible(true);
    }

    private void OnTriggerExit2D(Collider2D other) {
        PlayerHealth player =
            other.GetComponentInParent<PlayerHealth>();

        if (player != _playerInRange) {
            return;
        }

        _playerInRange = null;
        SetPromptVisible(false);
    }

    protected abstract void Interact(
        PlayerHealth player
    );

    protected void DisableInteraction() {
        _interactionEnabled = false;
        _playerInRange = null;

        SetPromptVisible(false);
    }

    private void SetPromptVisible(bool visible) {
        if (_interactionPrompt != null) {
            _interactionPrompt.SetActive(visible);
        }
    }
}