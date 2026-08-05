using TMPro;
using UnityEngine;

public class TabletInteraction : InteractableBase {
    [Header("Safe code")]
    [SerializeField] private SafeCode _safeCode;

    [Header("Tablet UI")]
    [SerializeField] private GameObject _codePanel;
    [SerializeField] private TMP_Text _codeText;

    [Header("0451 reaction")]
    [SerializeField] private GameObject _easterEggBubble;

    private bool _easterEggReactionShown;

    protected override void Awake() {
        base.Awake();

        SetCodePanelVisible(false);
        SetEasterEggBubbleVisible(false);

        if (_safeCode == null) {
            Debug.LogError(
                $"{name}: assign the Safe Code.",
                this
            );

            enabled = false;
            return;
        }

        if (_codePanel == null || _codeText == null) {
            Debug.LogError(
                $"{name}: assign the tablet code panel and text.",
                this
            );

            enabled = false;
        }
    }

    protected override void Interact(
        PlayerHealth player
    ) {
        DisableInteractionPrompt();

        _safeCode.Discover();
        _codeText.text = FormatCode(_safeCode.CurrentCode);

        SetCodePanelVisible(true);
        ShowEasterEggReactionIfNeeded();
    }

    protected override void OnPlayerExitedRange(
        PlayerHealth player
    ) {
        SetCodePanelVisible(false);
        SetEasterEggBubbleVisible(false);
    }

    private string FormatCode(string code) {
        return $"{code[0]} {code[1]} {code[2]} {code[3]}";
    }

    private void ShowEasterEggReactionIfNeeded() {
        if (
            _easterEggReactionShown ||
            _safeCode.CurrentCode != SafeCode.EasterEggCode
        ) {
            return;
        }

        _easterEggReactionShown = true;
        SetEasterEggBubbleVisible(true);
    }

    private void SetCodePanelVisible(bool visible) {
        if (_codePanel != null) {
            _codePanel.SetActive(visible);
        }
    }

    private void SetEasterEggBubbleVisible(
        bool visible
    ) {
        if (_easterEggBubble != null) {
            _easterEggBubble.SetActive(visible);
        }
    }
}