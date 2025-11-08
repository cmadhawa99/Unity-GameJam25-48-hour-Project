using UnityEngine;

public class SpawnTrigger : MonoBehaviour {
    // This will be set by the ChunkManager when it spawns
    public ChunkManager manager;

    void OnTriggerEnter(Collider other) {
        // When the player hits this trigger...
        if (other.CompareTag("Player")) {
            // ...tell the manager to spawn a new chunk.
            manager.SpawnChunk();

            // IMPORTANT: Disable this collider so it only
            // fires once per pass.
            gameObject.GetComponent<Collider>().enabled = false;
        }
    }
}