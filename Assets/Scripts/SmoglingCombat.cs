using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class SmoglingCombat : MonoBehaviour {
    [Header("Stomp")]
    [SerializeField] private float playerBounceSpeed = 7f;

    [Header("Death feedback")]
    [SerializeField] private Transform deathVisual;
    [SerializeField] private SpriteRenderer deathRenderer;

    [Min(0.05f)]
    [SerializeField] private float deathEffectSeconds = 0.25f;

    [Min(1f)]
    [SerializeField] private float finalHorizontalScale = 1.35f;

    [Range(0.01f, 1f)]
    [SerializeField] private float finalVerticalScale = 0.05f;

    [Header("Audio")]
    [SerializeField] private AudioClip playerDeathSound;
    [SerializeField] private AudioClip smoglingDeathSound;

    [Range(0f, 1f)]
    [SerializeField] private float soundVolume = 1f;

    [Header("References")]
    [SerializeField] private SmoglingPatrol patrol;
    [SerializeField] private Collider2D damageCollider;
    [SerializeField] private Collider2D stompZone;

    private Rigidbody2D _body;
    private bool _defeated;

    public bool Defeated => _defeated;

    private void Awake() {
        _body = GetComponent<Rigidbody2D>();

        if (patrol == null) {
            patrol = GetComponent<SmoglingPatrol>();
        }
    }

    public void DamagePlayer(PlayerHealth player) {
        if (_defeated || player == null) {
            return;
        }

        if (player.DamageSequenceRunning) {
            return;
        }

        PlaySound(
            playerDeathSound,
            player.transform.position
        );

        player.TakeDamage();
    }

    public void DefeatByStomp(PlayerHealth player) {
        if (_defeated) {
            return;
        }

        StartCoroutine(DefeatRoutine(player));
    }

    private IEnumerator DefeatRoutine(PlayerHealth player) {
        _defeated = true;

        if (patrol != null) {
            patrol.enabled = false;
        }

        if (damageCollider != null) {
            damageCollider.enabled = false;
        }

        if (stompZone != null) {
            stompZone.enabled = false;
        }

        _body.linearVelocity = Vector2.zero;
        _body.simulated = false;

        if (player != null) {
            player.Bounce(playerBounceSpeed);
        }

        PlaySound(
            smoglingDeathSound,
            transform.position
        );

        if (deathVisual == null || deathRenderer == null) {
            Destroy(gameObject);
            yield break;
        }

        SmoglingSizeShift sizeShift =
            deathVisual.GetComponent<SmoglingSizeShift>();

        if (sizeShift != null) {
            sizeShift.enabled = false;
        }

        Vector3 startingScale = deathVisual.localScale;
        Color startingColour = deathRenderer.color;

        float elapsedSeconds = 0f;

        while (elapsedSeconds < deathEffectSeconds) {
            elapsedSeconds += Time.deltaTime;

            float progress = Mathf.Clamp01(
                elapsedSeconds / deathEffectSeconds
            );

            float smoothProgress = Mathf.SmoothStep(
                0f,
                1f,
                progress
            );

            float horizontalMultiplier = Mathf.Lerp(
                1f,
                finalHorizontalScale,
                smoothProgress
            );

            float verticalMultiplier = Mathf.Lerp(
                1f,
                finalVerticalScale,
                smoothProgress
            );

            deathVisual.localScale = new Vector3(
                startingScale.x * horizontalMultiplier,
                startingScale.y * verticalMultiplier,
                startingScale.z
            );

            Color currentColour = startingColour;

            currentColour.a = Mathf.Lerp(
                startingColour.a,
                0f,
                smoothProgress
            );

            deathRenderer.color = currentColour;

            yield return null;
        }

        Destroy(gameObject);
    }

    private void PlaySound(
        AudioClip clip,
        Vector3 soundPosition
    ) {
        if (clip == null) {
            return;
        }

        GameObject temporaryAudio =
            new GameObject("Temporary death sound");

        temporaryAudio.transform.position = soundPosition;

        AudioSource audioSource =
            temporaryAudio.AddComponent<AudioSource>();

        audioSource.clip = clip;
        audioSource.volume = soundVolume;
        audioSource.spatialBlend = 0f;

        audioSource.Play();

        Destroy(
            temporaryAudio,
            clip.length + 0.1f
        );
    }
}