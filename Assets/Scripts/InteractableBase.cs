using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider2D))]
public abstract class InteractableBase : MonoBehaviour {
    private const string _interactActionPath = "Player/Interact";

    [Header("Interaction prompt")]
    [SerializeField] private GameObject _interactionPrompt;

    private InputAction _interactAction;
    private PlayerHealth _playerInRange;

    private bool _interactionEnabled = true;
    private bool _promptEnabled = true;

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

        if (InputSystem.actions == null) {
            Debug.LogError(
                $"{name}: no project-wide Input Actions asset was found.",
                this
            );

            enabled = false;
            return;
        }

        _interactAction = InputSystem.actions.FindAction(
            _interactActionPath,
            throwIfNotFound: false
        );

        if (_interactAction == null) {
            Debug.LogError(
                $"{name}: input action not found: {_interactActionPath}.",
                this
            );

            enabled = false;
            return;
        }

        if (!_interactAction.enabled) {
            _interactAction.Enable();
        }
    }

    private void Update() {
        if (
            !_interactionEnabled ||
            _playerInRange == null ||
            Time.timeScale == 0f
        ) {
            return;
        }

        if (!_interactAction.WasPressedThisFrame()) {
            return;
        }

        Interact(_playerInRange);
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

        SetPromptVisible(_promptEnabled);
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

    protected void DisableInteractionPrompt() {
        _promptEnabled = false;

        SetPromptVisible(false);
    }

    protected void DisableInteraction() {
        _interactionEnabled = false;
        _playerInRange = null;

        DisableInteractionPrompt();
    }

    private void SetPromptVisible(bool visible) {
        if (_interactionPrompt != null) {
            _interactionPrompt.SetActive(visible);
        }
    }
}