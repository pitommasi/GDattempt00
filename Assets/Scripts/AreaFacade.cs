using UnityEngine;

public class AreaFacade : MonoBehaviour {
    private const float _revealedAlpha = 0f;

    private SpriteRenderer[] _facadeRenderers;

    private void Awake() {
        _facadeRenderers =
            GetComponentsInChildren<SpriteRenderer>(
                includeInactive: true
            );

        if (_facadeRenderers.Length == 0) {
            Debug.LogError(
                $"{name}: no facade SpriteRenderers were found.",
                this
            );

            enabled = false;
        }
    }

    public void Reveal() {
        if (_facadeRenderers == null) {
            return;
        }

        foreach (SpriteRenderer facadeRenderer in _facadeRenderers) {
            SetRendererAlpha(
                facadeRenderer,
                _revealedAlpha
            );
        }
    }

    private void SetRendererAlpha(
        SpriteRenderer facadeRenderer,
        float alpha
    ) {
        if (facadeRenderer == null) {
            return;
        }

        Color facadeColour = facadeRenderer.color;
        facadeColour.a = alpha;
        facadeRenderer.color = facadeColour;
    }
}