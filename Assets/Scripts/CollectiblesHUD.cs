using UnityEngine;
using UnityEngine.UI;

public class CollectiblesHUD : MonoBehaviour {
    private const float _hiddenAlpha = 0f;
    private const float _discoveredAlpha = 0.35f;
    private const float _collectedAlpha = 1f;

    [Header("Inventory")]
    [SerializeField] private PlayerInventory _playerInventory;

    [Header("Background")]
    [SerializeField] private RectTransform _collectiblesBackground;

    [Header("Cogs — left to right")]
    [SerializeField] private Image[] _cogIcons;

    [Header("Fuel tanks — left to right")]
    [SerializeField] private Image[] _fuelTankIcons;

    [Header("Keys")]
    [SerializeField] private Image _warehouseKeyIcon;
    [SerializeField] private Image _repairShopKeyIcon;
    [SerializeField] private Image _backyardKeyIcon;

    private float _collapsedBackgroundWidth;
    private float _collapsedBackgroundRight;
    private float _backgroundRightPadding;
    private bool _backgroundSizeReady;

    private void OnEnable() {
        if (_playerInventory != null) {
            _playerInventory.InventoryChanged += RefreshHUD;
        }
    }

    private void Start() {
        if (!HasRequiredReferences()) {
            enabled = false;
            return;
        }

        Canvas.ForceUpdateCanvases();
        RememberCollapsedBackgroundSize();
        RefreshHUD();
    }

    private void OnDisable() {
        if (_playerInventory != null) {
            _playerInventory.InventoryChanged -= RefreshHUD;
        }
    }

    private bool HasRequiredReferences() {
        if (_playerInventory == null) {
            Debug.LogError($"{name}: assign the Player Inventory.", this);

            return false;
        }

        if (_collectiblesBackground == null) {
            Debug.LogError($"{name}: assign the Collectibles Background.", this);

            return false;
        }

        if (_fuelTankIcons.Length == 0 || _fuelTankIcons[_fuelTankIcons.Length - 1] == null) {
            Debug.LogError($"{name}: assign the fuel tank icons from left to right.", this);

            return false;
        }

        return true;
    }

    private void RememberCollapsedBackgroundSize() {
        RectTransform lastFuelTank = _fuelTankIcons[_fuelTankIcons.Length - 1].rectTransform;

        _collapsedBackgroundWidth = _collectiblesBackground.rect.width;

        _collapsedBackgroundRight = GetRightEdge(_collectiblesBackground);

        _backgroundRightPadding = _collapsedBackgroundRight - GetRightEdge(lastFuelTank);

        _backgroundSizeReady = true;
    }

    private void RefreshHUD() {
        UpdateProgressIcons(_cogIcons, _playerInventory.CogCount);

        UpdateProgressIcons(_fuelTankIcons, _playerInventory.FuelTankCount);

        UpdateKeyIcon(_warehouseKeyIcon, PlayerInventory.KeyType.Warehouse);

        UpdateKeyIcon(_repairShopKeyIcon, PlayerInventory.KeyType.RepairShop);

        UpdateKeyIcon(_backyardKeyIcon, PlayerInventory.KeyType.Backyard);

        UpdateBackgroundWidth();
    }

    private void UpdateProgressIcons(Image[] icons, int collectedAmount) {
        for (int index = 0; index < icons.Length; index++) {
            float targetAlpha = index < collectedAmount
                ? _collectedAlpha
                : _discoveredAlpha;

            SetIconAlpha(icons[index], targetAlpha);
        }
    }

    private void UpdateKeyIcon(Image icon, PlayerInventory.KeyType keyType) {
        float targetAlpha = _hiddenAlpha;

        if (_playerInventory.HasKey(keyType)) {
            targetAlpha = _collectedAlpha;
        } else if (_playerInventory.IsKeyRequirementRevealed(keyType)) {
            targetAlpha = _discoveredAlpha;
        }

        SetIconAlpha(icon, targetAlpha);
    }

    private void UpdateBackgroundWidth() {
        if (!_backgroundSizeReady) {
            return;
        }

        float targetRight = _collapsedBackgroundRight;

        targetRight = IncludeVisibleKey(targetRight, _warehouseKeyIcon, PlayerInventory.KeyType.Warehouse);

        targetRight = IncludeVisibleKey(targetRight, _repairShopKeyIcon, PlayerInventory.KeyType.RepairShop);

        targetRight = IncludeVisibleKey(targetRight, _backyardKeyIcon, PlayerInventory.KeyType.Backyard);

        float addedWidth = Mathf.Max(0f, targetRight - _collapsedBackgroundRight);

        _collectiblesBackground.SetSizeWithCurrentAnchors(
            RectTransform.Axis.Horizontal,
            _collapsedBackgroundWidth + addedWidth
        );
    }

    private float IncludeVisibleKey(float currentRight, Image icon, PlayerInventory.KeyType keyType) {
        if (icon == null || !IsKeyVisible(keyType)) {
            return currentRight;
        }

        float keyRight = GetRightEdge(icon.rectTransform) + _backgroundRightPadding;

        return Mathf.Max(currentRight, keyRight);
    }

    private bool IsKeyVisible(PlayerInventory.KeyType keyType) {
        return _playerInventory.HasKey(keyType) || _playerInventory.IsKeyRequirementRevealed(keyType);
    }

    private float GetRightEdge(RectTransform rectTransform) {
        Bounds bounds =
            RectTransformUtility.CalculateRelativeRectTransformBounds(_collectiblesBackground.parent, rectTransform);

        return bounds.max.x;
    }

    private void SetIconAlpha(Image icon, float alpha) {
        if (icon == null) {
            return;
        }

        Color iconColour = icon.color;
        iconColour.a = alpha;
        icon.color = iconColour;
    }
}