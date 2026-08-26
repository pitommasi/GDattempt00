using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class MotorcycleOutcomeUI : MonoBehaviour {
    [Header("Outcome panels")]
    [SerializeField] private GameObject _missingPartsPanel;
    [SerializeField] private GameObject _victoryPanel;

    [Header("Audio")]
    [SerializeField] private AudioClip _missingPartsSound;
    [SerializeField] private AudioClip _victorySound;

    private AudioSource _audioSource;

    private float _timeScaleBeforeVictory;
    private bool _victoryShown;

    private void Awake() {
        _audioSource = GetComponent<AudioSource>();

        SetMissingPartsVisible(false);
        SetVictoryVisible(false);

        if (_missingPartsPanel == null || _victoryPanel == null) {
            Debug.LogError($"{name}: assign both motorcycle outcome panels.", this);

            enabled = false;
        }
    }

    public void ShowMissingParts() {
        if (!enabled || _victoryShown) {
            return;
        }

        SetMissingPartsVisible(true);
        PlaySound(_missingPartsSound);
    }

    public void HideMissingParts() {
        if (_victoryShown) {
            return;
        }

        SetMissingPartsVisible(false);
    }

    public void ShowVictory() {
        if (!enabled || _victoryShown) {
            return;
        }

        _victoryShown = true;
        _timeScaleBeforeVictory = Time.timeScale;

        SetMissingPartsVisible(false);
        SetVictoryVisible(true);
        PlaySound(_victorySound);

        Time.timeScale = 0f;
    }

    public void RestartGame() {
        if (!_victoryShown) {
            return;
        }

        Time.timeScale = _timeScaleBeforeVictory;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void PlaySound(AudioClip sound) {
        if (sound != null) {
            _audioSource.PlayOneShot(sound);
        }
    }

    private void SetMissingPartsVisible(bool visible) {
        if (_missingPartsPanel != null) {
            _missingPartsPanel.SetActive(visible);
        }
    }

    private void SetVictoryVisible(bool visible) {
        if (_victoryPanel != null) {
            _victoryPanel.SetActive(visible);
        }
    }

    private void OnDestroy() {
        if (_victoryShown) {
            Time.timeScale = _timeScaleBeforeVictory;
        }
    }
}