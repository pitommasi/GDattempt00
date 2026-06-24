using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController2D : MonoBehaviour {
    [Header("Movement")]
    public float moveSpeed = 6f;
    public float jumpVelocity = 10f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.12f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private float horizontalInput;
    private bool jumpRequested;
    private bool isGrounded;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
    }

    private void Update() {
        horizontalInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.UpArrow)) {
            jumpRequested = true;
        }
    }

    private void FixedUpdate() {
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );

        rb.linearVelocity = new Vector2(
            horizontalInput * moveSpeed,
            rb.linearVelocity.y
        );

        if (jumpRequested && isGrounded) {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                jumpVelocity
            );
        }

        jumpRequested = false;
    }
}