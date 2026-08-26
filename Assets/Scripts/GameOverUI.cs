using System.Collections;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(CanvasGroup))]
public class GameOverUI : MonoBehaviour {
    [Header("Pop animation")]
    [Min(0.01f)]
    [SerializeField] private float growSeconds = 0.25f;

    [Range(0.01f, 1f)]
    [SerializeField] private float startingScale = 0.05f;

    private RectTransform _rectTransform;
    private CanvasGroup _canvasGroup;
    private Coroutine _growRoutine;

    private void Awake() {
        _rectTransform = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();

        HideImmediately();
    }

    public void Show() {
        if (_growRoutine != null) {
            StopCoroutine(_growRoutine);
        }

        _growRoutine = StartCoroutine(GrowRoutine());
    }

    private IEnumerator GrowRoutine() {
        _canvasGroup.alpha = 1f;

        float elapsedSeconds = 0f;

        while (elapsedSeconds < growSeconds) {
            elapsedSeconds += Time.unscaledDeltaTime;

            float progress = Mathf.Clamp01(elapsedSeconds / growSeconds);

            float smoothProgress = Mathf.SmoothStep(0f, 1f, progress);

            float currentScale = Mathf.Lerp(startingScale, 1f, smoothProgress);

            _rectTransform.localScale = Vector3.one * currentScale;

            yield return null;
        }

        _rectTransform.localScale = Vector3.one;
        _growRoutine = null;
    }

    private void HideImmediately() {
        _canvasGroup.alpha = 0f;
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;

        _rectTransform.localScale = Vector3.one * startingScale;
    }
}