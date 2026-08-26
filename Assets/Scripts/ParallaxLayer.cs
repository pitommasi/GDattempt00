using UnityEngine;

public class ParallaxLayer : MonoBehaviour {
    [Header("Movement source")]

    [SerializeField] private Transform player;

    [Header("Parallax")]

    [Tooltip("How far the layer moves in the opposite direction to the player.")]
    [SerializeField, Range(-0.25f, 0.25f)]
    private float parallaxStrength = 0.1f;

    [SerializeField] private bool moveVertically = false;

    private Vector3 playerStartPosition;
    private Vector3 layerStartPosition;

    private void Awake() {
        if (player == null) {
            Debug.LogError($"ParallaxLayer on '{gameObject.name}' requires the player's Transform!", gameObject);

            enabled = false;
            return;
        }

        playerStartPosition = player.position;
        layerStartPosition = transform.position;
    }

    private void LateUpdate() {
        Vector3 playerTravel = player.position - playerStartPosition;

        float horizontalOffset = -playerTravel.x * parallaxStrength;

        float verticalOffset = moveVertically
            ? -playerTravel.y * parallaxStrength
            : 0f;

        transform.position = layerStartPosition + new Vector3(horizontalOffset, verticalOffset, 0f);
    }
}