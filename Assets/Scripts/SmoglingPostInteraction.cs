using UnityEngine;

public class SmoglingPostInteraction : InteractableBase {
    [Header("Manifest")]
    [SerializeField] private ManifestPanelUI _manifestPanel;

    protected override void Interact(
        PlayerHealth player
    ) {
        if (_manifestPanel == null) {
            Debug.LogError(
                $"{name}: assign the Manifest Panel UI.",
                this
            );

            return;
        }

        DisableInteractionPrompt();
        _manifestPanel.Open();

        Debug.Log(
            "Smogling manifest read.",
            this
        );
    }
}