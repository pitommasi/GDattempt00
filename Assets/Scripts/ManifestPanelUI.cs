using UnityEngine;
using UnityEngine.InputSystem;

public class ManifestPanelUI : MonoBehaviour {
    [Header("Panel")]
    [SerializeField] private GameObject _panelRoot;

    private bool _isOpen;
    private int _openedFrame;

    private void Awake() {
        SetPanelVisible(false);
    }

    private void Update() {
        if (
            !_isOpen ||
            Keyboard.current == null ||
            Time.frameCount <= _openedFrame
        ) {
            return;
        }

        bool closePressed =
            Keyboard.current.escapeKey.wasPressedThisFrame ||
            Keyboard.current.eKey.wasPressedThisFrame ||
            Keyboard.current.fKey.wasPressedThisFrame;

        if (closePressed) {
            Close();
        }
    }

    public void Open() {
        if (_panelRoot == null || _isOpen) {
            return;
        }

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
        Time.timeScale = 1f;
    }

    private void SetPanelVisible(bool visible) {
        if (_panelRoot != null) {
            _panelRoot.SetActive(visible);
        }
    }

    private void OnDestroy() {
        if (_isOpen) {
            Time.timeScale = 1f;
        }
    }
}