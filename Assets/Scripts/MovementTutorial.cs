using UnityEngine;

public class MovementTutorial : MonoBehaviour {
    [Header("References")]
    [SerializeField] private Transform _player;
    [SerializeField] private GameObject _leftArrow;
    [SerializeField] private GameObject _rightArrow;

    [Header("Flashing")]
    [Min(0.05f)]
    [SerializeField] private float _flashIntervalSeconds = 0.3f;

    [Header("Disappearance")]
    [Min(0.1f)]
    [SerializeField] private float _hideDistance = 6f;

    private float _startingPlayerX;
    private float _flashTimer;
    private bool _arrowsVisible = true;

    private void Start() {
        if (
            _player == null ||
            _leftArrow == null ||
            _rightArrow == null
        ) {
            Debug.LogError(
                $"{name}: assign the Player and both tutorial arrows.",
                this
            );

            enabled = false;
            return;
        }

        _startingPlayerX = _player.position.x;
        SetArrowsVisible(true);
    }

    private void Update() {
        float horizontalDistance = Mathf.Abs(
            _player.position.x - _startingPlayerX
        );

        if (horizontalDistance >= _hideDistance) {
            gameObject.SetActive(false);
            return;
        }

        _flashTimer += Time.deltaTime;

        if (_flashTimer < _flashIntervalSeconds) {
            return;
        }

        _flashTimer = 0f;
        _arrowsVisible = !_arrowsVisible;

        SetArrowsVisible(_arrowsVisible);
    }

    private void SetArrowsVisible(bool visible) {
        _leftArrow.SetActive(visible);
        _rightArrow.SetActive(visible);
    }
}