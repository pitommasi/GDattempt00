using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerHealth : MonoBehaviour {
    [Header("Lives")]
    [Min(1)]
    [SerializeField] private int startingLives = 3;

    [Min(1)]
    [SerializeField] private int maximumLives = 3;

    [Header("Respawn")]
    [SerializeField] private Transform respawnPoint;

    [Min(0f)]
    [SerializeField] private float respawnDelaySeconds = 0.4f;

    [Min(0f)]
    [SerializeField] private float invulnerabilitySeconds = 1f;

    [Min(0f)]
    [SerializeField] private float gameOverDelaySeconds = 1f;

    [Header("Respawn flash")]
    [Min(0.02f)]
    [SerializeField] private float flashIntervalSeconds = 0.1f;

    [Header("Game over")]
    [SerializeField] private GameOverUI gameOverUI;

    [Header("References")]
    [SerializeField] private PlayerController2D movementController;
    [SerializeField] private SpriteRenderer playerRenderer;

    private Rigidbody2D _body;
    private int _currentLives;
    private bool _damageSequenceRunning;

    public event Action<int> LivesChanged;

    public int CurrentLives => _currentLives;
    public int MaximumLives => maximumLives;
    public bool DamageSequenceRunning => _damageSequenceRunning;

    private void Awake() {
        _body = GetComponent<Rigidbody2D>();

        if (movementController == null) {
            movementController = GetComponent<PlayerController2D>();
        }

        if (playerRenderer == null) {
            playerRenderer = GetComponentInChildren<SpriteRenderer>();
        }
    }

    private void Start() {
        maximumLives = Mathf.Max(1, maximumLives);

        _currentLives = Mathf.Clamp(startingLives, 1, maximumLives);

        if (respawnPoint == null) {
            Debug.LogError($"{name}: PlayerHealth needs a Respawn Point.", this);

            enabled = false;
            return;
        }

        if (gameOverUI == null) {
            Debug.LogWarning($"{name}: no Game Over UI has been assigned.", this);
        }

        LivesChanged?.Invoke(_currentLives);

        Debug.Log($"{name}: starting with {_currentLives} lives.", this);
    }

    public bool TryAddLife() {
        if (_currentLives >= maximumLives) {
            Debug.Log($"{name}: extra life ignored because lives are full.", this);

            return false;
        }

        _currentLives++;
        LivesChanged?.Invoke(_currentLives);

        Debug.Log($"{name}: extra life collected. Lives: {_currentLives}", this);

        return true;
    }

    public void TakeDamage(float delayOverrideSeconds = -1f) {
        if (_damageSequenceRunning) {
            return;
        }

        _currentLives = Mathf.Max(0, _currentLives - 1);
        LivesChanged?.Invoke(_currentLives);

        Debug.LogWarning($"PLAYER DEATH | Lives remaining: {_currentLives}", this);

        if (_currentLives <= 0) {
            float gameOverDelay = delayOverrideSeconds >= 0f
                ? delayOverrideSeconds
                : gameOverDelaySeconds;

            StartCoroutine(GameOverRoutine(gameOverDelay));
        } else {
            float respawnDelay = delayOverrideSeconds >= 0f
                ? delayOverrideSeconds
                : respawnDelaySeconds;

            StartCoroutine(RespawnRoutine(respawnDelay));
        }
    }

    public void Bounce(float bounceSpeed) {
        if (_damageSequenceRunning || !_body.simulated) {
            return;
        }

        _body.linearVelocity = new Vector2(_body.linearVelocity.x, bounceSpeed);
    }

    private IEnumerator RespawnRoutine(float delayBeforeRespawn) {
        _damageSequenceRunning = true;

        SetPlayerActive(false);

        yield return new WaitForSeconds(delayBeforeRespawn);

        transform.position = respawnPoint.position;

        SetPlayerActive(true);

        yield return StartCoroutine(FlashRoutine());

        _damageSequenceRunning = false;
    }

    private IEnumerator FlashRoutine() {
        if (playerRenderer == null || invulnerabilitySeconds <= 0f) {
            yield break;
        }

        float flashInterval = Mathf.Max(0.02f, flashIntervalSeconds);

        float elapsedSeconds = 0f;

        while (elapsedSeconds < invulnerabilitySeconds) {
            playerRenderer.enabled = !playerRenderer.enabled;

            yield return new WaitForSeconds(flashInterval);

            elapsedSeconds += flashInterval;
        }

        playerRenderer.enabled = true;
    }

    private IEnumerator GameOverRoutine(float delayBeforeGameOver) {
        _damageSequenceRunning = true;

        Debug.Log($"{name}: GAME OVER.", this);

        SetPlayerActive(false);

        yield return new WaitForSeconds(delayBeforeGameOver);

        if (gameOverUI != null) {
            gameOverUI.Show();
        }

        while (Keyboard.current == null || !Keyboard.current.spaceKey.wasPressedThisFrame) {
            yield return null;
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void SetPlayerActive(bool active) {
        if (movementController != null) {
            movementController.enabled = active;
        }

        if (playerRenderer != null) {
            playerRenderer.enabled = active;
        }

        if (active) {
            _body.simulated = true;
            _body.linearVelocity = Vector2.zero;
        } else {
            _body.linearVelocity = Vector2.zero;
            _body.simulated = false;
        }
    }
}