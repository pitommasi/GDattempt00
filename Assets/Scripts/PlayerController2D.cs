using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(AudioSource))]
public class PlayerController2D : MonoBehaviour {
    [Header("Input Actions")]
    [FormerlySerializedAs("moveActionPath")]
    [SerializeField] private string _moveActionPath = "Player/Move";

    [FormerlySerializedAs("jumpActionPath")]
    [SerializeField] private string _jumpActionPath = "Player/Jump";

    [Header("Movement")]
    [FormerlySerializedAs("moveSpeed")]
    [SerializeField] private float _moveSpeed = 7f;

    [FormerlySerializedAs("jumpVelocity")]
    [SerializeField] private float _jumpVelocity = 8.5f;

    [Header("Ground Check")]
    [FormerlySerializedAs("groundCheck")]
    [SerializeField] private Transform _groundCheck;

    [FormerlySerializedAs("groundCheckRadius")]
    [SerializeField] private float _groundCheckRadius = 0.12f;

    [FormerlySerializedAs("groundLayer")]
    [SerializeField] private LayerMask _groundLayer;

    [Header("Animation")]
    [FormerlySerializedAs("animator")]
    [SerializeField] private Animator _animator;

    [FormerlySerializedAs("playerSpriteRenderer")]
    [SerializeField] private SpriteRenderer _playerSpriteRenderer;

    [Header("Audio")]
    [SerializeField] private AudioClip _jumpSound;

    [Range(0f, 1f)]
    [SerializeField] private float _jumpVolume = 1f;

    [SerializeField] private AudioClip _footstepSound;

    [Range(0f, 1f)]
    [SerializeField] private float _footstepVolume = 1f;

    [Min(0.05f)]
    [SerializeField] private float _footstepInterval = 0.35f;

    private Rigidbody2D _rigidbody;
    private AudioSource _audioSource;
    private InputAction _moveAction;
    private InputAction _jumpAction;

    private float _horizontalInput;
    private float _footstepTimer;

    private bool _jumpPressed;
    private bool _isGrounded;
    private bool _footstepSequenceActive;

    private void Awake() {
        _rigidbody = GetComponent<Rigidbody2D>();
        _audioSource = GetComponent<AudioSource>();

        if (InputSystem.actions == null) {
            Debug.LogError(
                "PlayerController2D: no project-wide Input Actions asset found. Check Edit > Project Settings > Input System Package.",
                this
            );

            enabled = false;
            return;
        }

        _moveAction = InputSystem.actions.FindAction(
            _moveActionPath,
            throwIfNotFound: false
        );

        _jumpAction = InputSystem.actions.FindAction(
            _jumpActionPath,
            throwIfNotFound: false
        );

        if (_moveAction == null) {
            Debug.LogError(
                "PlayerController2D: input action not found: " + _moveActionPath,
                this
            );

            enabled = false;
            return;
        }

        if (_jumpAction == null) {
            Debug.LogError(
                "PlayerController2D: input action not found: " + _jumpActionPath,
                this
            );

            enabled = false;
            return;
        }

        if (!_moveAction.enabled) {
            _moveAction.Enable();
        }

        if (!_jumpAction.enabled) {
            _jumpAction.Enable();
        }
    }

    private void Update() {
        Vector2 moveInput = _moveAction.ReadValue<Vector2>();
        _horizontalInput = moveInput.x;

        if (_jumpAction.WasPressedThisFrame()) {
            _jumpPressed = true;
        }

        UpdateFacingDirection();
    }

    private void FixedUpdate() {
        _isGrounded = CheckIsGrounded();

        ApplyHorizontalMovement();
        TryJump();
        UpdateFootsteps();

        _jumpPressed = false;

        UpdateAnimationParameters();
    }

    private void ApplyHorizontalMovement() {
        _rigidbody.linearVelocity = new Vector2(
            _horizontalInput * _moveSpeed,
            _rigidbody.linearVelocity.y
        );
    }

    private void TryJump() {
        if (!_jumpPressed || !_isGrounded) {
            return;
        }

        StopFootsteps();

        _rigidbody.linearVelocity = new Vector2(
            _rigidbody.linearVelocity.x,
            _jumpVelocity
        );

        _isGrounded = false;

        PlaySound(_jumpSound, _jumpVolume);
    }

    private void UpdateFootsteps() {
        bool shouldPlayFootsteps =
            _isGrounded &&
            _rigidbody.linearVelocity.y <= 0.01f &&
            Mathf.Abs(_horizontalInput) > 0.01f;

        if (!shouldPlayFootsteps) {
            StopFootsteps();
            return;
        }

        _footstepTimer -= Time.fixedDeltaTime;

        if (_footstepTimer > 0f) {
            return;
        }

        PlayFootsteps();
        _footstepTimer = _footstepInterval;
    }

    private void PlayFootsteps() {
        _footstepSequenceActive = PlaySound(
            _footstepSound,
            _footstepVolume
        );
    }

    private void StopFootsteps() {
        _footstepTimer = 0f;

        if (!_footstepSequenceActive) {
            return;
        }

        _audioSource.Stop();
        _footstepSequenceActive = false;
    }

    private bool PlaySound(AudioClip sound, float volume) {
        if (sound == null) {
            return false;
        }

        _audioSource.PlayOneShot(sound, volume);
        return true;
    }

    private bool CheckIsGrounded() {
        if (_groundCheck == null) {
            return false;
        }

        return Physics2D.OverlapCircle(
            _groundCheck.position,
            _groundCheckRadius,
            _groundLayer
        );
    }

    private void UpdateFacingDirection() {
        if (_playerSpriteRenderer == null) {
            return;
        }

        if (_horizontalInput > 0f) {
            _playerSpriteRenderer.flipX = false;
        } else if (_horizontalInput < 0f) {
            _playerSpriteRenderer.flipX = true;
        }
    }

    private void UpdateAnimationParameters() {
        if (_animator == null) {
            return;
        }

        _animator.SetFloat("Speed", Mathf.Abs(_horizontalInput));
        _animator.SetFloat(
            "VerticalVelocity",
            _rigidbody.linearVelocity.y
        );
        _animator.SetBool("IsGrounded", _isGrounded);
    }

    private void OnDrawGizmosSelected() {
        if (_groundCheck == null) {
            return;
        }

        Gizmos.DrawWireSphere(
            _groundCheck.position,
            _groundCheckRadius
        );
    }
}