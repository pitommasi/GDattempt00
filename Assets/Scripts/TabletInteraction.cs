using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class TabletInteraction : InteractableBase {
    private const float _easterEggBubbleDuration = 3f;

    [Header("Safe code")]
    [SerializeField] private SafeCode _safeCode;

    [Header("Tablet UI")]
    [SerializeField] private GameObject _codePanel;
    [SerializeField] private TMP_Text _codeText;

    [Header("0451 reaction")]
    [SerializeField] private GameObject _easterEggBubble;

    [Header("Audio")]
    [SerializeField] private AudioClip _codeDiscoveredSound;

    private AudioSource _audioSource;
    private bool _easterEggReactionPending;

    protected override void Awake() {
        base.Awake();

        _audioSource = GetComponent<AudioSource>();

        SetCodePanelVisible(false);
        SetEasterEggBubbleVisible(false);

        if (_safeCode == null) {
            Debug.LogError($"{name}: assign the Safe Code.", this);

            enabled = false;
            return;
        }

        if (_codePanel == null || _codeText == null) {
            Debug.LogError($"{name}: assign the tablet code panel and text.", this);

            enabled = false;
        }
    }

    protected override void Interact(PlayerHealth player) {
        DisableInteractionPrompt();

        StopAllCoroutines();
        SetEasterEggBubbleVisible(false);

        bool codeDiscoveredNow = _safeCode.Discover();

        _codeText.text = FormatCode(_safeCode.CurrentCode);

        SetCodePanelVisible(true);

        if (!codeDiscoveredNow) {
            return;
        }

        PlayDiscoverySound();

        _easterEggReactionPending = _safeCode.CurrentCode == SafeCode.EasterEggCode;
    }

    protected override void OnPlayerExitedRange(PlayerHealth player) {
        SetCodePanelVisible(false);

        if (_easterEggReactionPending) {
            StartCoroutine(ShowQueuedEasterEggReaction());
        }
    }

    private IEnumerator ShowQueuedEasterEggReaction() {
        _easterEggReactionPending = false;

        SetEasterEggBubbleVisible(true);

        yield return new WaitForSecondsRealtime(_easterEggBubbleDuration);

        SetEasterEggBubbleVisible(false);
    }

    private string FormatCode(string code) {
        return $"{code[0]} {code[1]} {code[2]} {code[3]}";
    }

    private void PlayDiscoverySound() {
        if (_codeDiscoveredSound != null) {
            _audioSource.PlayOneShot(_codeDiscoveredSound);
        }
    }

    private void SetCodePanelVisible(bool visible) {
        if (_codePanel != null) {
            _codePanel.SetActive(visible);
        }
    }

    private void SetEasterEggBubbleVisible(bool visible) {
        if (_easterEggBubble != null) {
            _easterEggBubble.SetActive(visible);
        }
    }

    private void OnDisable() {
        StopAllCoroutines();

        SetCodePanelVisible(false);
        SetEasterEggBubbleVisible(false);
    }
}