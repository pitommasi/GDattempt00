using UnityEngine;

public class SafeCode : MonoBehaviour {
    public const string EasterEggCode = "0451";

    private const int _minimumCode = 0;
    private const int _maximumCodeExclusive = 10000;

#if UNITY_EDITOR
    [Header("Testing")]
    [SerializeField] private bool _force0451ForTesting;
#endif

    private string _currentCode;
    private bool _hasBeenDiscovered;

    public string CurrentCode => _currentCode;
    public bool HasBeenDiscovered => _hasBeenDiscovered;

    private void Awake() {
        GenerateCode();
    }

    public bool Discover() {
        if (_hasBeenDiscovered) {
            return false;
        }

        _hasBeenDiscovered = true;

        return true;
    }

    private void GenerateCode() {
#if UNITY_EDITOR
        if (_force0451ForTesting) {
            _currentCode = EasterEggCode;
            return;
        }
#endif

        int generatedNumber = Random.Range(_minimumCode, _maximumCodeExclusive);

        _currentCode = generatedNumber.ToString("D4");
    }
}