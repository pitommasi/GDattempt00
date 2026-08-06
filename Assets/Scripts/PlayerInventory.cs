using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour {
    public const int RequiredCogCount = 3;
    public const int RequiredFuelTankCount = 3;

    public enum KeyType {
        Warehouse,
        RepairShop,
        Backyard
    }

    private readonly HashSet<KeyType> _revealedKeyRequirements =
        new HashSet<KeyType>();

    private readonly HashSet<KeyType> _collectedKeys =
        new HashSet<KeyType>();

    private int _cogCount;
    private int _fuelTankCount;

    public event Action InventoryChanged;

    public int CogCount => _cogCount;
    public int FuelTankCount => _fuelTankCount;

    public bool HasAllMotorcycleParts =>
        _cogCount >= RequiredCogCount &&
        _fuelTankCount >= RequiredFuelTankCount;

    public void AddCogs(int amount) {
        _cogCount += Mathf.Max(1, amount);

        NotifyInventoryChanged();
    }

    public void AddFuelTanks(int amount) {
        _fuelTankCount += Mathf.Max(1, amount);

        NotifyInventoryChanged();
    }

    public void RevealKeyRequirement(KeyType keyType) {
        if (_collectedKeys.Contains(keyType)) {
            return;
        }

        if (!_revealedKeyRequirements.Add(keyType)) {
            return;
        }

        NotifyInventoryChanged();
    }

    public bool TryCollectKey(KeyType keyType) {
        if (!_collectedKeys.Add(keyType)) {
            return false;
        }

        _revealedKeyRequirements.Add(keyType);

        NotifyInventoryChanged();

        return true;
    }

    public bool HasKey(KeyType keyType) {
        return _collectedKeys.Contains(keyType);
    }

    public bool IsKeyRequirementRevealed(
        KeyType keyType
    ) {
        return _revealedKeyRequirements.Contains(keyType);
    }

    private void NotifyInventoryChanged() {
        InventoryChanged?.Invoke();
    }
}