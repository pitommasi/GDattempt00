using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AudioSource))]
public class BackgroundMusicUI : MonoBehaviour {
    private const string MusicActionPath = "Player/MusicToggle";

    [SerializeField] private TMP_Text _buttonLabel;

    private AudioSource _musicSource;
    private InputAction _musicAction;

    private void Awake() {
        _musicSource = GetComponent<AudioSource>();

        if (_buttonLabel == null) {
            Debug.LogError($"{name}: assign the music button label.", this);

            enabled = false;
            return;
        }

        FindMusicAction();
        UpdateButtonLabel();
    }

    private void Update() {
        if (_musicAction != null && _musicAction.WasPressedThisFrame()) {
            ToggleMusic();
        }
    }

    public void ToggleMusic() {
        if (!enabled) {
            return;
        }

        _musicSource.mute = !_musicSource.mute;
        UpdateButtonLabel();
    }

    private void FindMusicAction() {
        if (InputSystem.actions == null) {
            Debug.LogError($"{name}: no project-wide Input Actions asset was found.", this);

            return;
        }

        _musicAction = InputSystem.actions.FindAction(MusicActionPath, throwIfNotFound: false);

        if (_musicAction == null) {
            Debug.LogError($"{name}: {MusicActionPath} was not found in the project-wide Input Actions.", this);

            return;
        }

        if (!_musicAction.enabled) {
            _musicAction.Enable();
        }
    }

    private void UpdateButtonLabel() {
        _buttonLabel.text = _musicSource.mute
            ? "MUSIC: OFF"
            : "MUSIC: ON";
    }
}