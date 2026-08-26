using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class SmoglingPatrol : MonoBehaviour {
    [Header("Facing")]
    [SerializeField] private bool artworkFacesRight = true;
    [SerializeField] private bool startMovingRight = true;

    [Header("Patrol limits")]
    [SerializeField] private Transform leftPoint;
    [SerializeField] private Transform rightPoint;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 1.5f;

    private Rigidbody2D body;
    private SpriteRenderer spriteRenderer;
    private Vector3 originalScale;
    private int direction;

    private void Awake() {
        body = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalScale = transform.localScale;
        
        direction = startMovingRight ? 1 : -1;
    }
    
    private void Start() {
        if (leftPoint == null || rightPoint == null) {
            Debug.LogError($"{name}: assign both patrol points in the Inspector.", this);

            enabled = false;
            return;
        }

        if (leftPoint.position.x > rightPoint.position.x) {
            Debug.LogError($"{name}: Left Point must be to the left of Right Point.", this);

            enabled = false;
        }
        
        UpdateFacing();
    }

    private void UpdateFacing() {
        bool movingRight = direction > 0;

        bool shouldMirror = artworkFacesRight
            ? !movingRight
            : movingRight;

        float scaleSign = shouldMirror ? -1f : 1f;

        transform.localScale = new Vector3(Mathf.Abs(originalScale.x) * scaleSign, originalScale.y, originalScale.z);

        spriteRenderer.flipX = false;
    }
    
    private void FixedUpdate() {
        if (direction > 0 && body.position.x >= rightPoint.position.x) {
            direction = -1;
        } else if (direction < 0 && body.position.x <= leftPoint.position.x) {
            direction = 1;
        }

        body.linearVelocity = new Vector2(direction * moveSpeed, body.linearVelocity.y);

        UpdateFacing();
    }

    private void OnDisable() {
        if (body != null) {
            body.linearVelocity = new Vector2(0f, body.linearVelocity.y);
        }
    }
}