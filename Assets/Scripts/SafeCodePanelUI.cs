using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AudioSource))]
public class SafeCodePanelUI : MonoBehaviour {
    private const int _codeLength = 4;
    private const float _feedbackDuration = 0.75f;

    private static readonly Color _normalColour = Color.white;

    private static readonly Color _wrongColour = new Color(1f, 0.25f, 0.25f);

    private static readonly Color _correctColour = new Color(0.35f, 1f, 0.35f);

    [Header("Safe code panel")]
    [SerializeField] private GameObject _panelRoot;
    [SerializeField] private TMP_Text _digitsText;
    [SerializeField] private TMP_Text _feedbackText;

    [Header("Audio")]
    [SerializeField] private AudioClip _wrongCodeSound;
    [SerializeField] private AudioClip _correctCodeSound;

    private AudioSource _audioSource;
    private SafeCode _safeCode;
    private Action _correctCodeEntered;

    private string _enteredCode = string.Empty;
    private string _defaultFeedbackText;

    private float _timeScaleBeforeOpening;
    private bool _panelOpen;
    private bool _acceptingInput;
    private bool _codeAccepted;

    private void Awake() {
        _audioSource = GetComponent<AudioSource>();

        SetPanelVisible(false);

        if (_panelRoot == null || _digitsText == null || _feedbackText == null) {
            Debug.LogError($"{name}: assign the safe code panel and both text fields.", this);

            enabled = false;
            return;
        }

        _defaultFeedbackText = _feedbackText.text;

        ResetForNewAttempt();
    }

    private void Update() {
        if (!_panelOpen || !_acceptingInput) {
            return;
        }

        Keyboard keyboard = Keyboard.current;

        if (keyboard == null) {
            return;
        }

        if (keyboard.backspaceKey.wasPressedThisFrame) {
            RemoveLastDigit();
            return;
        }

        int pressedDigit = ReadPressedDigit(keyboard);

        if (pressedDigit >= 0) {
            AddDigit(pressedDigit);
        }
    }

    public void Open(SafeCode safeCode, Action correctCodeEntered) {
        if (!enabled || safeCode == null || _panelOpen) {
            return;
        }

        StopAllCoroutines();

        _safeCode = safeCode;
        _correctCodeEntered = correctCodeEntered;
        _timeScaleBeforeOpening = Time.timeScale;
        _panelOpen = true;
        _codeAccepted = false;

        ResetForNewAttempt();
        SetPanelVisible(true);

        Time.timeScale = 0f;
    }

    public void Close() {
        if (!_panelOpen || _codeAccepted) {
            return;
        }

        StopAllCoroutines();
        ClosePanel();
    }

    private void AddDigit(int digit) {
        if (_enteredCode.Length >= _codeLength) {
            return;
        }

        _enteredCode += digit.ToString();

        RefreshDigits();

        if (_enteredCode.Length == _codeLength) {
            CheckEnteredCode();
        }
    }

    private void RemoveLastDigit() {
        if (_enteredCode.Length == 0) {
            return;
        }

        _enteredCode = _enteredCode.Substring(0, _enteredCode.Length - 1);

        RefreshDigits();
    }

    private void CheckEnteredCode() {
        if (_enteredCode == _safeCode.CurrentCode) {
            AcceptCode();
            return;
        }

        RejectCode();
    }

    private void AcceptCode() {
        _acceptingInput = false;
        _codeAccepted = true;
        _digitsText.color = _correctColour;

        bool guessed0451WithoutKnowing = _enteredCode == SafeCode.EasterEggCode && !_safeCode.HasBeenDiscovered;

        _feedbackText.text = guessed0451WithoutKnowing
            ? "Nice try!"
            : string.Empty;

        PlaySound(_correctCodeSound);

        StartCoroutine(CompleteCorrectEntry());
    }

    private void RejectCode() {
        string attemptedCode = _enteredCode;

        _enteredCode = string.Empty;
        _acceptingInput = false;

        RefreshDigits();
        _digitsText.color = _wrongColour;

        if (attemptedCode == SafeCode.EasterEggCode) {
            _feedbackText.text = "Nice try! Wrong place.";
        } else {
            RestoreDefaultFeedback();
        }

        PlaySound(_wrongCodeSound);

        StartCoroutine(ResetAfterWrongEntry());
    }

    private IEnumerator CompleteCorrectEntry() {
        yield return new WaitForSecondsRealtime(_feedbackDuration);

        Action correctCodeEntered = _correctCodeEntered;

        ClosePanel();
        correctCodeEntered?.Invoke();
    }

    private IEnumerator ResetAfterWrongEntry() {
        yield return new WaitForSecondsRealtime(_feedbackDuration);

        _digitsText.color = _normalColour;
        _acceptingInput = true;

        RestoreDefaultFeedback();
        RefreshDigits();
    }

    private void ResetForNewAttempt() {
        _enteredCode = string.Empty;
        _acceptingInput = true;
        _codeAccepted = false;

        if (_digitsText != null) {
            _digitsText.color = _normalColour;
        }

        RestoreDefaultFeedback();
        RefreshDigits();
    }

    private void RestoreDefaultFeedback() {
        if (_feedbackText != null) {
            _feedbackText.text = _defaultFeedbackText;
        }
    }

    private void RefreshDigits() {
        if (_digitsText == null) {
            return;
        }

        char[] displayedCharacters = {
            '#',
            '#',
            '#',
            '#'
        };

        for (int index = 0; index < _enteredCode.Length; index++) {
            displayedCharacters[index] = _enteredCode[index];
        }

        _digitsText.text =
            $"{displayedCharacters[0]} " +
            $"{displayedCharacters[1]} " +
            $"{displayedCharacters[2]} " +
            $"{displayedCharacters[3]}";
    }

    private int ReadPressedDigit(Keyboard keyboard) {
        if (keyboard.digit0Key.wasPressedThisFrame || keyboard.numpad0Key.wasPressedThisFrame) {
            return 0;
        }

        if (keyboard.digit1Key.wasPressedThisFrame || keyboard.numpad1Key.wasPressedThisFrame) {
            return 1;
        }

        if (keyboard.digit2Key.wasPressedThisFrame || keyboard.numpad2Key.wasPressedThisFrame) {
            return 2;
        }

        if (keyboard.digit3Key.wasPressedThisFrame || keyboard.numpad3Key.wasPressedThisFrame) {
            return 3;
        }

        if (keyboard.digit4Key.wasPressedThisFrame || keyboard.numpad4Key.wasPressedThisFrame) {
            return 4;
        }

        if (keyboard.digit5Key.wasPressedThisFrame || keyboard.numpad5Key.wasPressedThisFrame) {
            return 5;
        }

        if (keyboard.digit6Key.wasPressedThisFrame || keyboard.numpad6Key.wasPressedThisFrame) {
            return 6;
        }

        if (keyboard.digit7Key.wasPressedThisFrame || keyboard.numpad7Key.wasPressedThisFrame) {
            return 7;
        }

        if (keyboard.digit8Key.wasPressedThisFrame || keyboard.numpad8Key.wasPressedThisFrame) {
            return 8;
        }

        if (keyboard.digit9Key.wasPressedThisFrame || keyboard.numpad9Key.wasPressedThisFrame) {
            return 9;
        }

        return -1;
    }

    private void PlaySound(AudioClip sound) {
        if (sound != null) {
            _audioSource.PlayOneShot(sound);
        }
    }

    private void SetPanelVisible(bool visible) {
        if (_panelRoot != null) {
            _panelRoot.SetActive(visible);
        }
    }

    private void ClosePanel() {
        _panelOpen = false;
        _acceptingInput = false;
        _codeAccepted = false;

        SetPanelVisible(false);

        Time.timeScale = _timeScaleBeforeOpening;

        _safeCode = null;
        _correctCodeEntered = null;
    }

    private void OnDestroy() {
        if (_panelOpen) {
            Time.timeScale = _timeScaleBeforeOpening;
        }
    }
}