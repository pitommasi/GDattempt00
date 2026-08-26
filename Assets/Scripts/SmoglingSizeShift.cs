using System.Collections;
using UnityEngine;

public class SmoglingSizeShift : MonoBehaviour {
    [Header("Size")]
    [SerializeField] private float minimumScaleMultiplier = 0.85f;
    [SerializeField] private float maximumScaleMultiplier = 1.2f;

    [Header("Pause between changes")]
    [Min(0f)]
    [SerializeField] private float minimumPauseSeconds = 0.6f;

    [Min(0f)]
    [SerializeField] private float maximumPauseSeconds = 1.2f;

    [Header("Transition")]
    [Min(0.01f)]
    [SerializeField] private float minimumTransitionSeconds = 0.25f;

    [Min(0.01f)]
    [SerializeField] private float maximumTransitionSeconds = 0.45f;

    private Vector3 normalScale;
    private Coroutine sizeRoutine;

    private void Awake() {
        normalScale = transform.localScale;
    }

    private void OnEnable() {
        sizeRoutine = StartCoroutine(ChangeSizeRoutine());
    }

    private void OnDisable() {
        if (sizeRoutine != null) {
            StopCoroutine(sizeRoutine);
            sizeRoutine = null;
        }

        transform.localScale = normalScale;
    }

    private IEnumerator ChangeSizeRoutine() {
        while (true) {
            float pauseSeconds = Random.Range(minimumPauseSeconds, maximumPauseSeconds);

            yield return new WaitForSeconds(pauseSeconds);

            float targetMultiplier = Random.Range(minimumScaleMultiplier, maximumScaleMultiplier);

            float transitionSeconds = Random.Range(minimumTransitionSeconds, maximumTransitionSeconds);

            Vector3 scaleAtStart = transform.localScale;
            Vector3 targetScale = normalScale * targetMultiplier;

            float elapsedSeconds = 0f;

            while (elapsedSeconds < transitionSeconds) {
                elapsedSeconds += Time.deltaTime;

                float progress = Mathf.Clamp01(elapsedSeconds / transitionSeconds);

                float smoothProgress = Mathf.SmoothStep(0f, 1f, progress);

                transform.localScale = Vector3.Lerp(scaleAtStart, targetScale, smoothProgress);

                yield return null;
            }

            transform.localScale = targetScale;
        }
    }
}