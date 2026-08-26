using UnityEngine;

public class UIPulse : MonoBehaviour {
    public float pulseSpeed = 3f;
    public float pulseAmount = 0.15f;

    private Vector3 baseScale;

    private void Awake() {
        baseScale = transform.localScale;
    }

    private void Update() {
        float pulse = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;

        transform.localScale = new Vector3(baseScale.x * pulse, baseScale.y * pulse, baseScale.z);
    }
}