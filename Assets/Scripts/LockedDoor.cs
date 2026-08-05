using UnityEngine;

public class LockedDoor : InteractableBase {
    [Header("Required key")]
    [SerializeField]
    private PlayerInventory.KeyType _requiredKey;

    [SerializeField]
    private GameObject _requiredKeyBubble;

    [Header("Opening")]
    [SerializeField]
    private Collider2D _blockingCollider;

    [SerializeField]
    private AreaFacade _facadeToReveal;

    protected override void Awake() {
        base.Awake();

        SetRequiredKeyBubbleVisible(false);

        if (_requiredKeyBubble == null) {
            Debug.LogError(
                $"{name}: assign the required-key bubble.",
                this
            );

            enabled = false;
            return;
        }

        if (_blockingCollider == null) {
            Debug.LogError(
                $"{name}: assign the doorway's blocking collider.",
                this
            );

            enabled = false;
        }
    }

    protected override void Interact(
        PlayerHealth player
    ) {
        PlayerInventory playerInventory =
            player.GetComponent<PlayerInventory>();

        if (playerInventory == null) {
            Debug.LogError(
                $"{name}: PlayerInventory was not found beside PlayerHealth.",
                this
            );

            return;
        }

        if (!playerInventory.HasKey(_requiredKey)) {
            playerInventory.RevealKeyRequirement(
                _requiredKey
            );

            SetRequiredKeyBubbleVisible(true);
            return;
        }

        OpenDoor();
    }

    protected override void OnPlayerExitedRange(
        PlayerHealth player
    ) {
        SetRequiredKeyBubbleVisible(false);
    }

    private void OpenDoor() {
        DisableInteraction();

        _blockingCollider.enabled = false;

        if (_facadeToReveal != null) {
            _facadeToReveal.Reveal();
        }

        gameObject.SetActive(false);
    }

    private void SetRequiredKeyBubbleVisible(
        bool visible
    ) {
        if (_requiredKeyBubble != null) {
            _requiredKeyBubble.SetActive(visible);
        }
    }
}