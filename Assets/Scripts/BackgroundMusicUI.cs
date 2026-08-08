using TMPro;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BackgroundMusicUI : MonoBehaviour {
    [SerializeField] private TMP_Text _buttonLabel;

    private AudioSource _musicSource;

    private void Awake() {
        _musicSource = GetComponent<AudioSource>();

        if (_buttonLabel == null) {
            Debug.LogError(
                $"{name}: assign the music button label.",
                this
            );

            enabled = false;
            return;
        }

        UpdateButtonLabel();
    }

    public void ToggleMusic() {
        if (!enabled) {
            return;
        }

        _musicSource.mute = !_musicSource.mute;
        UpdateButtonLabel();
    }

    private void UpdateButtonLabel() {
        _buttonLabel.text = _musicSource.mute
            ? "MUSIC: OFF"
            : "MUSIC: ON";
    }
}