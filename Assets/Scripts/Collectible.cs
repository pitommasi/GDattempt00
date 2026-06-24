using UnityEngine;

public class Collectible : MonoBehaviour {
    public enum CollectibleType {
        Key,
        Keycard,
        Fuel,
        Coin
    }

    public CollectibleType collectibleType;
    public int amount = 1;
    public AudioClip pickupSound;

    private void OnTriggerEnter2D(Collider2D other) {
        if (!other.CompareTag("Player")) {
            return;
        }

        Debug.Log("Collected: " + collectibleType);

        if (pickupSound != null) {
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);
        }

        // Temporary prototype logic
        switch (collectibleType) {
            case CollectibleType.Key:
                Debug.Log("Player now has a key.");
                break;

            case CollectibleType.Keycard:
                Debug.Log("Player now has a keycard.");
                break;

            case CollectibleType.Fuel:
                Debug.Log("Added fuel: " + amount);
                break;

            case CollectibleType.Coin:
                Debug.Log("Added coins: " + amount);
                break;
        }

        Destroy(gameObject);
    }
}