using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(AudioSource))]
public class HazardDeathZone : MonoBehaviour {
    [Header("Audio")]
    [SerializeField] private AudioClip deathSound;

    [Range(0f, 1f)]
    [SerializeField] private float soundVolume = 1f;

    private Collider2D triggerCollider;
    private AudioSource audioSource;

    private void Awake() {
        triggerCollider = GetComponent<Collider2D>();
        audioSource = GetComponent<AudioSource>();

        if (!triggerCollider.isTrigger) {
            Debug.LogWarning($"{name}: the Collider 2D should have Is Trigger enabled.", this);
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        PlayerHealth player = other.GetComponentInParent<PlayerHealth>();

        if (player == null) {
            return;
        }

        if (player.DamageSequenceRunning) {
            return;
        }

        float delayBeforeRespawn = -1f;

        if (deathSound != null) {
            audioSource.PlayOneShot(deathSound, soundVolume);

            float playbackPitch = Mathf.Max(0.01f, Mathf.Abs(audioSource.pitch));

            delayBeforeRespawn = deathSound.length / playbackPitch;
        }

        player.TakeDamage(delayBeforeRespawn);
    }
}