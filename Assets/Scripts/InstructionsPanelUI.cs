using UnityEngine;
using UnityEngine.InputSystem;

public class InstructionsPanelUI : MonoBehaviour {
    [SerializeField] private GameObject _instructionsPanel;

    private InputAction _helpAction;

    private float _timeScaleBeforeOpening;
    private bool _panelIsOpen;

    private void Awake() {
        if (_instructionsPanel == null) {
            Debug.LogError(
                $"{name}: assign the instructions panel.",
                this
            );

            enabled = false;
            return;
        }

        _helpAction = InputSystem.actions.FindAction("Player/Help");

        if (_helpAction == null) {
            Debug.LogError(
                $"{name}: Player/Help was not found in the project-wide Input Actions.",
                this
            );

            enabled = false;
            return;
        }

        _instructionsPanel.SetActive(false);
    }

    private void OnEnable() {
        if (_helpAction != null) {
            _helpAction.performed += TogglePanel;
        }
    }

    private void Start() {
        OpenPanel();
    }

    private void TogglePanel(InputAction.CallbackContext context) {
        if (_panelIsOpen) {
            ClosePanel();
        } else {
            OpenPanel();
        }
    }

    private void OpenPanel() {
        if (_panelIsOpen) {
            return;
        }

        _timeScaleBeforeOpening = Time.timeScale;
        _panelIsOpen = true;
        _instructionsPanel.SetActive(true);

        Time.timeScale = 0f;
    }

    private void ClosePanel() {
        if (!_panelIsOpen) {
            return;
        }

        _panelIsOpen = false;
        _instructionsPanel.SetActive(false);

        Time.timeScale = _timeScaleBeforeOpening;
    }

    private void OnDisable() {
        if (_helpAction != null) {
            _helpAction.performed -= TogglePanel;
        }

        if (_panelIsOpen) {
            ClosePanel();
        }
    }
}