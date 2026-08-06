using TMPro;
using UnityEngine;

public class SafeInteraction : InteractableBase {
    [Header("Safe code")]
    [SerializeField] private SafeCode _safeCode;
    [SerializeField] private SafeCodePanelUI _codePanelUI;

    [Header("Proximity bubbles")]
    [SerializeField] private GameObject _unknownCodeBubble;
    [SerializeField] private GameObject _knownCodeBubble;
    [SerializeField] private TMP_Text _knownCodeText;

    [Header("Safe state")]
    [SerializeField] private GameObject _closedSafe;
    [SerializeField] private GameObject _openSafe;
    [SerializeField] private GameObject _backyardKey;

    private Collider2D _interactionCollider;

    protected override void Awake() {
        base.Awake();

        _interactionCollider = GetComponent<Collider2D>();

        SetSafeBubblesVisible(false);

        if (!HasRequiredReferences()) {
            enabled = false;
            return;
        }

        _knownCodeText.text = "# # # #";

        SetInteractionPrompt(_unknownCodeBubble);
        ResetSafe();
    }

    protected override void Interact(
        PlayerHealth player
    ) {
        DisableInteractionPrompt();
        SetSafeBubblesVisible(false);

        _codePanelUI.Open(
            _safeCode,
            OpenSafe
        );
    }

    protected override void OnPlayerEnteredRange(
        PlayerHealth player
    ) {
        if (_safeCode.HasBeenDiscovered) {
            _knownCodeText.text =
                FormatCode(_safeCode.CurrentCode);

            SetInteractionPrompt(_knownCodeBubble);
        } else {
            SetInteractionPrompt(_unknownCodeBubble);
        }

        ShowInteractionPrompt();
    }

    protected override void OnPlayerExitedRange(
        PlayerHealth player
    ) {
        SetSafeBubblesVisible(false);
        _codePanelUI.Close();
    }

    private bool HasRequiredReferences() {
        if (
            _safeCode == null ||
            _codePanelUI == null ||
            _unknownCodeBubble == null ||
            _knownCodeBubble == null ||
            _knownCodeText == null ||
            _closedSafe == null ||
            _openSafe == null ||
            _backyardKey == null
        ) {
            Debug.LogError(
                $"{name}: complete every Safe Interaction assignment.",
                this
            );

            return false;
        }

        return true;
    }

    private string FormatCode(string code) {
        return $"{code[0]} {code[1]} {code[2]} {code[3]}";
    }

    private void ResetSafe() {
        _closedSafe.SetActive(true);
        _openSafe.SetActive(false);
        _backyardKey.SetActive(false);
    }

    private void OpenSafe() {
        DisableInteraction();

        _interactionCollider.enabled = false;

        _openSafe.SetActive(true);
        _backyardKey.SetActive(true);
        _closedSafe.SetActive(false);
    }

    private void SetSafeBubblesVisible(
        bool visible
    ) {
        if (_unknownCodeBubble != null) {
            _unknownCodeBubble.SetActive(visible);
        }

        if (_knownCodeBubble != null) {
            _knownCodeBubble.SetActive(visible);
        }
    }
}