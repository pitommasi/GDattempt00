using UnityEngine;
using UnityEngine.InputSystem;

public class ManifestPanelUI : MonoBehaviour {
    private const string _interactActionPath = "Player/Interact";
    private const string _cancelActionPath = "UI/Cancel";

    [Header("Panel")]
    [SerializeField] private GameObject _panelRoot;

    private InputAction _interactAction;
    private InputAction _cancelAction;

    private bool _isOpen;
    private bool _resumeTimeAtEndOfFrame;
    private int _openedFrame;

    private void Awake() {
        SetPanelVisible(false);

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

        _cancelAction = InputSystem.actions.FindAction(
            _cancelActionPath,
            throwIfNotFound: false
        );

        if (_interactAction == null || _cancelAction == null) {
            Debug.LogError(
                $"{name}: the Interact or Cancel input action was not found.",
                this
            );

            enabled = false;
            return;
        }

        if (!_interactAction.enabled) {
            _interactAction.Enable();
        }

        if (!_cancelAction.enabled) {
            _cancelAction.Enable();
        }
    }

    private void Update() {
        if (
            !_isOpen ||
            Time.frameCount <= _openedFrame
        ) {
            return;
        }

        bool closePressed =
            _interactAction.WasPressedThisFrame() ||
            _cancelAction.WasPressedThisFrame();

        if (closePressed) {
            Close();
        }
    }

    private void LateUpdate() {
        if (!_resumeTimeAtEndOfFrame) {
            return;
        }

        _resumeTimeAtEndOfFrame = false;
        Time.timeScale = 1f;
    }

    public void Open() {
        if (_panelRoot == null || _isOpen) {
            return;
        }

        _resumeTimeAtEndOfFrame = false;
        _isOpen = true;
        _openedFrame = Time.frameCount;

        SetPanelVisible(true);

        Time.timeScale = 0f;
    }

    public void Close() {
        if (!_isOpen) {
            return;
        }

        _isOpen = false;

        SetPanelVisible(false);

        // Resume after every Update has finished so the key that
        // closes the panel cannot reopen it during the same frame.
        _resumeTimeAtEndOfFrame = true;
    }

    private void SetPanelVisible(bool visible) {
        if (_panelRoot != null) {
            _panelRoot.SetActive(visible);
        }
    }

    private void OnDestroy() {
        if (_isOpen || _resumeTimeAtEndOfFrame) {
            Time.timeScale = 1f;
        }
    }
}