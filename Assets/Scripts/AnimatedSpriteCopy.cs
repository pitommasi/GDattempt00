using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class AnimatedSpriteCopy : MonoBehaviour {
    [Header("Animated source")]
    [SerializeField] private SpriteRenderer animatedSource;

    private SpriteRenderer displayRenderer;

    private void Awake() {
        displayRenderer = GetComponent<SpriteRenderer>();

        if (animatedSource == null) {
            Debug.LogError(
                $"{name}: assign the animated Sprite Renderer.",
                this
            );

            enabled = false;
        }
    }

    private void LateUpdate() {
        displayRenderer.sprite = animatedSource.sprite;
        displayRenderer.flipX = animatedSource.flipX;
    }
}