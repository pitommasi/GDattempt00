using UnityEngine;

public class LockedDoor : InteractableBase {
    [Header("Required key")]
    [SerializeField]
    private PlayerInventory.KeyType _requiredKey;

    [SerializeField]
    private GameObject _requiredKeyBubble;

    [Header("Opening")]
    [SerializeField]
    private Collider2D _blockingCollider;

    [SerializeField]
    private AreaFacade _facadeToReveal;

    [Header("Audio")]
    [SerializeField]
    private AudioClip _openingSound;

    [Range(0f, 1f)]
    [SerializeField]
    private float _openingVolume = 1f;

    protected override void Awake() {
        base.Awake();

        SetRequiredKeyBubbleVisible(false);

        if (_requiredKeyBubble == null) {
            Debug.LogError(
                $"{name}: assign the required-key bubble.",
                this
            );

            enabled = false;
            return;
        }

        if (_blockingCollider == null) {
            Debug.LogError(
                $"{name}: assign the doorway's blocking collider.",
                this
            );

            enabled = false;
        }
    }

    protected override void Interact(
        PlayerHealth player
    ) {
        PlayerInventory playerInventory =
            player.GetComponent<PlayerInventory>();

        if (playerInventory == null) {
            Debug.LogError(
                $"{name}: PlayerInventory was not found beside PlayerHealth.",
                this
            );

            return;
        }

        if (!playerInventory.HasKey(_requiredKey)) {
            playerInventory.RevealKeyRequirement(
                _requiredKey
            );

            SetRequiredKeyBubbleVisible(true);
            return;
        }

        OpenDoor();
    }

    protected override void OnPlayerExitedRange(
        PlayerHealth player
    ) {
        SetRequiredKeyBubbleVisible(false);
    }

    private void OpenDoor() {
        PlayOpeningSound();
        DisableInteraction();

        _blockingCollider.enabled = false;

        if (_facadeToReveal != null) {
            _facadeToReveal.Reveal();
        }

        gameObject.SetActive(false);
    }

    private void PlayOpeningSound() {
        if (_openingSound == null) {
            return;
        }

        GameObject soundObject =
            new GameObject("Door opening sound");

        AudioSource soundSource =
            soundObject.AddComponent<AudioSource>();

        soundSource.clip = _openingSound;
        soundSource.volume = _openingVolume;
        soundSource.spatialBlend = 0f;
        soundSource.Play();

        Destroy(soundObject, _openingSound.length);
    }

    private void SetRequiredKeyBubbleVisible(
        bool visible
    ) {
        if (_requiredKeyBubble != null) {
            _requiredKeyBubble.SetActive(visible);
        }
    }
}