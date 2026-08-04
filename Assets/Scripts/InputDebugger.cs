using UnityEngine;
using UnityEngine.InputSystem;

public class InputDebugger : MonoBehaviour {
    [Header("Input Actions")]
    [SerializeField] private string moveActionPath = "Player/Move";
    [SerializeField] private string jumpActionPath = "Player/Jump";

    [Header("Ground check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.12f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Debug output")]
    [SerializeField] private float logInterval = 0.25f;
    [SerializeField] private bool logMoveChanges = true;
    [SerializeField] private bool logJumpEvents = true;
    [SerializeField] private bool logRepeatedStatus = true;

    private Rigidbody2D rb;
    private InputAction moveAction;
    private InputAction jumpAction;

    private Vector2 previousMoveInput;
    private float nextLogTime;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();

        if (rb == null) {
            Debug.LogError("InputDebugger: no Rigidbody2D found on this GameObject.", this);
            enabled = false;
            return;
        }

        if (InputSystem.actions == null) {
            Debug.LogError(
                "InputDebugger: no project-wide Input Actions asset found. Check Edit > Project Settings > Input System Package.",
                this
            );

            enabled = false;
            return;
        }

        moveAction = InputSystem.actions.FindAction(moveActionPath, throwIfNotFound: false);
        jumpAction = InputSystem.actions.FindAction(jumpActionPath, throwIfNotFound: false);

        if (moveAction == null) {
            Debug.LogError(
                "InputDebugger: input action not found: " + moveActionPath,
                this
            );

            enabled = false;
            return;
        }

        if (jumpAction == null) {
            Debug.LogError(
                "InputDebugger: input action not found: " + jumpActionPath,
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

        if (logMoveChanges && moveInput != previousMoveInput) {
            Debug.Log(
                "Move changed | Vector: " + moveInput +
                " | X: " + moveInput.x.ToString("F2") +
                " | Y: " + moveInput.y.ToString("F2") +
                " | Active control: " + GetActiveControlName(moveAction)
            );

            previousMoveInput = moveInput;
        }

        if (logJumpEvents && jumpAction.WasPressedThisFrame()) {
            Debug.Log(
                "Jump pressed | Active control: " + GetActiveControlName(jumpAction)
            );
        }

        if (logJumpEvents && jumpAction.WasReleasedThisFrame()) {
            Debug.Log(
                "Jump released | Active control: " + GetActiveControlName(jumpAction)
            );
        }

        if (logRepeatedStatus && Time.time >= nextLogTime) {
            nextLogTime = Time.time + logInterval;
            LogStatus(moveInput);
        }
    }

    private void LogStatus(Vector2 moveInput) {
        Vector2 velocity = rb.linearVelocity;
        float horizontalSpeed = Mathf.Abs(velocity.x);
        float totalSpeed = velocity.magnitude;
        bool isGrounded = CheckIsGrounded();

        // Debug.Log(
        //     "Status | Move: " + moveInput +
        //     " | Horizontal input: " + moveInput.x.ToString("F2") +
        //     " | Speed X: " + horizontalSpeed.ToString("F2") +
        //     " | Total Speed: " + totalSpeed.ToString("F2") +
        //     " | Velocity: " + velocity +
        //     " | IsGrounded: " + isGrounded
        // );
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

    private string GetActiveControlName(InputAction action) {
        if (action == null || action.activeControl == null) {
            return "none";
        }

        return action.activeControl.displayName;
    }

    private void OnDrawGizmosSelected() {
        if (groundCheck == null) {
            return;
        }

        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}