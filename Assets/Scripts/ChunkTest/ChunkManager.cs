// In Unity (C#)
using UnityEngine;
using System.Collections.Generic; // Needed for Lists!

public class ChunkManager : MonoBehaviour {
    // --- The Core Variables ---

    // 1. The Chunk Prefab
    // Drag your "Chunk" prefab here in the Inspector
    public GameObject[] chunkPrefabs;

    // 2. The List of Recycled Chunks (Object Pool)
    // This is our "recycling bin"
    private Queue<GameObject> chunkPool = new Queue<GameObject>();

    // 3. The List of Active Chunks
    // This tracks the chunks currently in the game
    private List<GameObject> activeChunks = new List<GameObject>();

    // 4. Chunk Properties
    public float chunkHeight = 100f; // Must match your prefab's height!
    private float nextSpawnY; // Tracks where to place the next chunk

    // --- The Functions ---

    void Start() {
        // Start the game by spawning the first 3 chunks
        // (Player starts at 0, so we spawn 0m, 100m, 200m)
        nextSpawnY = 0;
        for (int i = 0; i < 3; i++) {
            SpawnChunk();
        }
    }

    public void SpawnChunk() {
        GameObject newChunk;

        // Use a pooled chunk if available
        if (chunkPool.Count > 0) {
            newChunk = chunkPool.Dequeue();
            newChunk.SetActive(true);
        } else {
            int randomIndex = Random.Range(0, chunkPrefabs.Length);
            GameObject prefabToSpawn = chunkPrefabs[randomIndex];
            newChunk = Instantiate(prefabToSpawn, transform);
        }

        // --- Horizontal + Diagonal Spawn Logic ---
        Vector3 spawnPos;

        // Randomly decide to spawn straight or diagonal (50/50 chance)
        bool diagonal = Random.value > 0.5f;

        if (activeChunks.Count == 0) {
            // First chunk always at origin
            spawnPos = Vector3.zero;
        } else {
            // Base the new chunk on the last one
            Vector3 lastPos = activeChunks[activeChunks.Count - 1].transform.position;

            // Horizontal distance between chunks
            float xOffset = chunkHeight;

            // Vertical offset for diagonal
            float yOffset = diagonal ? Random.Range(-2f, 2f) : 0f;

            spawnPos = new Vector3(lastPos.x + xOffset, lastPos.y + yOffset, 0);
        }

        // Apply the spawn position
        newChunk.transform.position = spawnPos;

        // Assign manager and track
        newChunk.GetComponent<Chunk>().manager = this;
        activeChunks.Add(newChunk);
    }



    public void RecycleChunk(GameObject chunkToRecycle) {
        // 1. Turn the chunk off
        chunkToRecycle.SetActive(false);

        // 2. Remove it from the "active" list
        activeChunks.Remove(chunkToRecycle);

        // 3. Add it to the "recycling bin" (pool)
        chunkPool.Enqueue(chunkToRecycle);
    }
}