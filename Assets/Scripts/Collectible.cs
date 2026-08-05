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

    [Header("Key")]
    [SerializeField] private PlayerInventory.KeyType _keyType;

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

        PlayerInventory playerInventory =
            other.GetComponentInParent<PlayerInventory>();

        bool effectApplied = TryApplyEffect(
            playerHealth,
            playerInventory
        );

        if (!effectApplied) {
            return;
        }

        _collected = true;
        _pickupCollider.enabled = false;

        PlayPickupSound();

        Debug.Log(
            $"Collected {_collectibleType}. Amount: {_amount}",
            this
        );

        Destroy(gameObject);
    }

    private bool TryApplyEffect(
        PlayerHealth playerHealth,
        PlayerInventory playerInventory
    ) {
        if (_collectibleType == CollectibleType.Life) {
            return TryAddLives(playerHealth);
        }

        if (playerInventory == null) {
            Debug.LogError(
                $"{name}: the player needs a PlayerInventory component.",
                this
            );

            return false;
        }

        switch (_collectibleType) {
            case CollectibleType.Key:
                return playerInventory.TryCollectKey(
                    _keyType
                );

            case CollectibleType.Cog:
                playerInventory.AddCogs(_amount);
                return true;

            case CollectibleType.Tank:
                playerInventory.AddFuelTanks(_amount);
                return true;

            default:
                Debug.LogWarning(
                    $"{name}: unsupported collectible type.",
                    this
                );

                return false;
        }
    }

    private bool TryAddLives(
        PlayerHealth playerHealth
    ) {
        bool addedAtLeastOneLife = false;

        for (int index = 0; index < _amount; index++) {
            if (!playerHealth.TryAddLife()) {
                break;
            }

            addedAtLeastOneLife = true;
        }

        return addedAtLeastOneLife;
    }

    private void PlayPickupSound() {
        if (_pickupSound == null) {
            return;
        }

        AudioSource.PlayClipAtPoint(
            _pickupSound,
            transform.position,
            _pickupVolume
        );
    }
}