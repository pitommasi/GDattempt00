using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController2D : MonoBehaviour {
    [Header("Input Actions")]
    [SerializeField] private string moveActionPath = "Player/Move";
    [SerializeField] private string jumpActionPath = "Player/Jump";

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float jumpVelocity = 8.5f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.12f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer playerSpriteRenderer;

    private Rigidbody2D rb;
    private InputAction moveAction;
    private InputAction jumpAction;

    private float horizontalInput;
    private bool jumpPressed;
    private bool isGrounded;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();

        if (InputSystem.actions == null) {
            Debug.LogError(
                "PlayerController2D: no project-wide Input Actions asset found. Check Edit > Project Settings > Input System Package.",
                this
            );

            enabled = false;
            return;
        }

        moveAction = InputSystem.actions.FindAction(moveActionPath, throwIfNotFound: false);
        jumpAction = InputSystem.actions.FindAction(jumpActionPath, throwIfNotFound: false);

        if (moveAction == null) {
            Debug.LogError(
                "PlayerController2D: input action not found: " + moveActionPath,
                this
            );

            enabled = false;
            return;
        }

        if (jumpAction == null) {
            Debug.LogError(
                "PlayerController2D: input action not found: " + jumpActionPath,
                this
            );

            enabled = false;
            return;
        }

        if (!moveAction.enabled) {
            moveAction.Enable();
        }

        if (!jumpAction.enabled) {
            jumpAction.Enable();
        }
    }

    private void Update() {
        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        horizontalInput = moveInput.x;

        if (jumpAction.WasPressedThisFrame()) {
            jumpPressed = true;
        }

        UpdateFacingDirection();
    }

    private void FixedUpdate() {
        isGrounded = CheckIsGrounded();

        rb.linearVelocity = new Vector2(
            horizontalInput * moveSpeed,
            rb.linearVelocity.y
        );

        if (jumpPressed && isGrounded) {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                jumpVelocity
            );
        }

        jumpPressed = false;

        UpdateAnimationParameters();
    }

    private bool CheckIsGrounded() {
        if (groundCheck == null) {
            return false;
        }

        return Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );
    }

    private void UpdateFacingDirection() {
        if (playerSpriteRenderer == null) {
            return;
        }

        if (horizontalInput > 0f) {
            playerSpriteRenderer.flipX = false;
        }
        else if (horizontalInput < 0f) {
            playerSpriteRenderer.flipX = true;
        }
    }

    private void UpdateAnimationParameters() {
        if (animator == null) {
            return;
        }

        animator.SetFloat("Speed", Mathf.Abs(horizontalInput));
        animator.SetFloat("VerticalVelocity", rb.linearVelocity.y);
        animator.SetBool("IsGrounded", isGrounded);
    }

    private void OnDrawGizmosSelected() {
        if (groundCheck == null) {
            return;
        }

        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}