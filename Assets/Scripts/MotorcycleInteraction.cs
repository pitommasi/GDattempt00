using UnityEngine;

public class MotorcycleInteraction : InteractableBase {
    [Header("Outcome")]
    [SerializeField]
    private MotorcycleOutcomeUI _outcomeUI;

    protected override void Awake() {
        base.Awake();

        if (_outcomeUI == null) {
            Debug.LogError(
                $"{name}: assign the Motorcycle Outcome UI.",
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

        DisableInteractionPrompt();

        if (playerInventory.HasAllMotorcycleParts) {
            DisableInteraction();
            _outcomeUI.ShowVictory();
            return;
        }

        _outcomeUI.ShowMissingParts();
    }

    protected override void OnPlayerExitedRange(
        PlayerHealth player
    ) {
        _outcomeUI.HideMissingParts();
    }
}