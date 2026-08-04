using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Collectible : MonoBehaviour {
    public enum CollectibleType {
        Key,
        Life,
        Cog,
        Tank
    }

    [Header("Collectible")]
    [SerializeField] private CollectibleType _collectibleType;

    [Min(1)]
    [SerializeField] private int _amount = 1;

    [Header("Audio")]
    [SerializeField] private AudioClip _pickupSound;

    [Range(0f, 1f)]
    [SerializeField] private float _pickupVolume = 1f;

    private Collider2D _pickupCollider;
    private bool _collected;

    private void Awake() {
        _pickupCollider = GetComponent<Collider2D>();

        if (!_pickupCollider.isTrigger) {
            Debug.LogWarning(
                $"{name}: enable Is Trigger on the collectible collider.",
                this
            );
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (_collected) {
            return;
        }

        PlayerHealth playerHealth =
            other.GetComponentInParent<PlayerHealth>();

        if (playerHealth == null) {
            return;
        }

        bool effectApplied = TryApplyEffect(playerHealth);

        if (!effectApplied) {
            return;
        }

        _collected = true;
        _pickupCollider.enabled = false;

        if (_pickupSound != null) {
            AudioSource.PlayClipAtPoint(
                _pickupSound,
                transform.position,
                _pickupVolume
            );
        }

        Debug.Log(
            $"Collected {_collectibleType}. Amount: {_amount}",
            this
        );

        Destroy(gameObject);
    }

    private bool TryApplyEffect(PlayerHealth playerHealth) {
        switch (_collectibleType) {
            case CollectibleType.Key:
                Debug.Log(
                    $"Key collected. Amount: {_amount}",
                    this
                );

                return true;

            case CollectibleType.Life:
                return TryAddLives(playerHealth);

            case CollectibleType.Cog:
                Debug.Log(
                    $"Cog collected. Amount: {_amount}",
                    this
                );

                return true;

            case CollectibleType.Tank:
                Debug.Log(
                    $"Tank collected. Amount: {_amount}",
                    this
                );

                return true;

            default:
                Debug.LogWarning(
                    $"{name}: unsupported collectible type.",
                    this
                );

                return false;
        }
    }

    private bool TryAddLives(PlayerHealth playerHealth) {
        bool addedAtLeastOneLife = false;

        for (int index = 0; index < _amount; index++) {
            if (!playerHealth.TryAddLife()) {
                break;
            }

            addedAtLeastOneLife = true;
        }

        return addedAtLeastOneLife;
    }
}