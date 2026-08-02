using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerHealth : MonoBehaviour {
    [Header("Lives")]
    [Min(1)]
    [SerializeField] private int startingLives = 3;

    [Header("Respawn")]
    [SerializeField] private Transform respawnPoint;

    [Min(0f)]
    [SerializeField] private float respawnDelaySeconds = 0.4f;

    [Min(0f)]
    [SerializeField] private float invulnerabilitySeconds = 1f;

    [Min(0f)]
    [SerializeField] private float gameOverDelaySeconds = 1f;

    [Header("References")]
    [SerializeField] private PlayerController2D movementController;
    [SerializeField] private SpriteRenderer playerRenderer;

    private Rigidbody2D body;
    private int currentLives;
    private bool damageSequenceRunning;

    public int CurrentLives => currentLives;
    public bool DamageSequenceRunning => damageSequenceRunning;

    private void Awake() {
        body = GetComponent<Rigidbody2D>();

        if (movementController == null) {
            movementController = GetComponent<PlayerController2D>();
        }

        if (playerRenderer == null) {
            playerRenderer = GetComponentInChildren<SpriteRenderer>();
        }
    }

    private void Start() {
        currentLives = Mathf.Max(1, startingLives);

        if (respawnPoint == null) {
            Debug.LogError(
                $"{name}: PlayerHealth needs a Respawn Point.",
                this
            );

            enabled = false;
            return;
        }

        Debug.Log($"{name}: starting with {currentLives} lives.");
    }

    public void TakeDamage(float delayOverrideSeconds = -1f) {
        if (damageSequenceRunning) {
            return;
        }

        currentLives--;

        Debug.Log(
            $"{name}: life lost. Lives remaining: {currentLives}",
            this
        );

        if (currentLives <= 0) {
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
        if (damageSequenceRunning || !body.simulated) {
            return;
        }

        body.linearVelocity = new Vector2(
            body.linearVelocity.x,
            bounceSpeed
        );
    }

    private IEnumerator RespawnRoutine(float delayBeforeRespawn) {
        damageSequenceRunning = true;

        SetPlayerActive(false);

        yield return new WaitForSeconds(delayBeforeRespawn);

        transform.position = respawnPoint.position;

        SetPlayerActive(true);

        yield return new WaitForSeconds(invulnerabilitySeconds);

        damageSequenceRunning = false;
    }

    private IEnumerator GameOverRoutine(float delayBeforeReload) {
        damageSequenceRunning = true;

        Debug.Log($"{name}: GAME OVER.", this);

        SetPlayerActive(false);

        yield return new WaitForSeconds(delayBeforeReload);

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().name
        );
    }

    private void SetPlayerActive(bool active) {
        if (movementController != null) {
            movementController.enabled = active;
        }

        if (playerRenderer != null) {
            playerRenderer.enabled = active;
        }

        if (active) {
            body.simulated = true;
            body.linearVelocity = Vector2.zero;
        } else {
            body.linearVelocity = Vector2.zero;
            body.simulated = false;
        }
    }
}