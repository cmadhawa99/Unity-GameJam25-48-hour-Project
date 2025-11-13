using UnityEngine;

public class Rock : MonoBehaviour {
    [Header("Effects")]
    public GameObject explosionParticlePrefab;

    private void OnCollisionEnter2D(Collision2D collision) {
        // Check if the object we collided with has the "Player" tag
        if (collision.gameObject.CompareTag("Player")) {
            Explode();
        }
    }

    private void Explode() {
        // 1. Spawn the particle effect
        if (explosionParticlePrefab != null) {
            // Spawn the particle system at the rock's current position and rotation
            Instantiate(explosionParticlePrefab, transform.position, Quaternion.identity);
        }

        // 2. Destroy this rock object
        Destroy(gameObject);
    }
}