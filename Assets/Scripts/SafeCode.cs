using UnityEngine;

public class SafeCode : MonoBehaviour {
    public const string EasterEggCode = "0451";

    private const int _minimumCode = 0;
    private const int _maximumCodeExclusive = 10000;

    private string _currentCode;
    private bool _hasBeenDiscovered;

    public string CurrentCode => _currentCode;
    public bool HasBeenDiscovered => _hasBeenDiscovered;

    private void Awake() {
        GenerateCode();
    }

    public void Discover() {
        _hasBeenDiscovered = true;
    }

    private void GenerateCode() {
        int generatedNumber = Random.Range(
            _minimumCode,
            _maximumCodeExclusive
        );

        _currentCode = generatedNumber.ToString("D4");
    }
}