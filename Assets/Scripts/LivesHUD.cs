using UnityEngine;

public class LivesHUD : MonoBehaviour {
    [Header("References")]
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private GameObject[] lifeIcons;

    private void OnEnable() {
        if (playerHealth != null) {
            playerHealth.LivesChanged += UpdateLives;
        }
    }

    private void Start() {
        if (playerHealth == null) {
            Debug.LogError($"{name}: assign PlayerHealth.", this);

            enabled = false;
            return;
        }

        UpdateLives(playerHealth.CurrentLives);
    }

    private void OnDisable() {
        if (playerHealth != null) {
            playerHealth.LivesChanged -= UpdateLives;
        }
    }

    private void UpdateLives(int currentLives) {
        for (int index = 0; index < lifeIcons.Length; index++) {
            if (lifeIcons[index] == null) {
                continue;
            }

            lifeIcons[index].SetActive(index < currentLives);
        }
    }
}