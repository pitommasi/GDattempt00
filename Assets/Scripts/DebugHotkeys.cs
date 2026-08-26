using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
public class DebugHotkeys : MonoBehaviour {
    [Header("Debug")]
    [Tooltip("Enables the debug restart and teleport keyboard shortcuts.")]
    [SerializeField] private bool _debugMode = true;

    [Header("Teleport points")]
    [SerializeField] private Transform _playerSpawnPoint;
    [SerializeField] private Transform _shopPoint;
    [SerializeField] private Transform _warehousePoint;
    [SerializeField] private Transform _backyardPoint;
    [SerializeField] private Transform _safePoint;

    private Rigidbody2D _body;

    private void Awake() {
        _body = GetComponent<Rigidbody2D>();
    }

    private void Update() {
        // enable teleport shortcuts for easier testing only for editor and dev build
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        if (!_debugMode || Keyboard.current == null) {
            return;
        }

        if (Keyboard.current.rKey.wasPressedThisFrame) {
            RestartScene();
            return;
        }

        if (Keyboard.current.pKey.wasPressedThisFrame) {
            TeleportTo(_playerSpawnPoint, "player spawn point");
            return;
        }

        if (Keyboard.current.sKey.wasPressedThisFrame) {
            TeleportTo(_shopPoint, "shop");
            return;
        }

        if (Keyboard.current.cKey.wasPressedThisFrame) {
            TeleportTo(_warehousePoint, "warehouse");
            return;
        }

        if (Keyboard.current.bKey.wasPressedThisFrame) {
            TeleportTo(_backyardPoint, "backyard");
            return;
        }

        if (Keyboard.current.oKey.wasPressedThisFrame) {
            TeleportTo(_safePoint, "safe");
        }
#endif
    }

    private void RestartScene() {
        Debug.Log("DEBUG | Restarting current scene.", this);

        Time.timeScale = 1f;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void TeleportTo(Transform destination, string destinationDescription) {
        if (destination == null) {
            Debug.LogWarning($"DEBUG | No teleport point assigned for " + $"{destinationDescription}.", this);

            return;
        }

        _body.linearVelocity = Vector2.zero;
        _body.angularVelocity = 0f;
        _body.position = destination.position;

        Physics2D.SyncTransforms();

        Debug.Log($"DEBUG | Teleported to {destinationDescription}.", this);
    }
}
