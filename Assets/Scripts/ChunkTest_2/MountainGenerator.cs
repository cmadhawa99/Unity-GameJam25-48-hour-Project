using System.Collections.Generic;
using UnityEngine;

public class EndlessMountain : MonoBehaviour {
    [Header("Player & Prefab")]
    public Transform playerTransform; // Drag your Player
    public GameObject mountainChunkPrefab; // Drag your Triangle Prefab

    [Header("Terrain Generation")]
    [Tooltip("The fixed horizontal width of each chunk.")]
    public float chunkBaseWidth = 10f;

    [Tooltip("How many chunks to load in each direction (e.g., 5 = 11 total chunks).")]
    public int renderDistanceInChunks = 5;

    [Header("Noise Settings")]
    [Tooltip("Scale of the Perlin noise. Smaller = smoother/longer slopes.")]
    public float noiseScale = 0.1f;
    [Tooltip("Seed for the noise. Change this to get a new mountain.")]
    public float noiseSeed = 0f;

    // --- Private ---
    private int currentPlayerChunkIndex;
    private Dictionary<int, GameObject> activeChunks = new Dictionary<int, GameObject>();

    // Caches to store data about chunks we've already calculated
    private Dictionary<int, Vector2> chunkStartCache = new Dictionary<int, Vector2>();
    private Dictionary<int, float> chunkHeightCache = new Dictionary<int, float>();

    void Start() {
        // Initialize the caches with the starting chunk (index 0)
        chunkStartCache[0] = Vector2.zero;
        chunkHeightCache[0] = GetChunkHeight(0); // Calculate height for chunk 0

        // Find where the player is and load initial chunks
        currentPlayerChunkIndex = GetChunkIndexFromPosition(playerTransform.position.x);
        UpdateVisibleChunks();
    }

    void Update() {
        // Check if the player has moved into a new chunk index
        int newIndex = GetChunkIndexFromPosition(playerTransform.position.x);
        if (newIndex != currentPlayerChunkIndex) {
            currentPlayerChunkIndex = newIndex;
            UpdateVisibleChunks();
        }
    }

    /// <summary>
    /// Checks which chunks should be active and loads/unloads them.
    /// </summary>
    void UpdateVisibleChunks() {
        // Create a temporary list of chunks to remove
        List<int> chunksToRemove = new List<int>(activeChunks.Keys);

        // Loop through all chunks that *should* be visible
        for (int i = -renderDistanceInChunks; i <= renderDistanceInChunks; i++) {
            int indexToGenerate = currentPlayerChunkIndex + i;

            if (activeChunks.ContainsKey(indexToGenerate)) {
                // Chunk already exists, so "keep" it (remove it from the kill-list)
                chunksToRemove.Remove(indexToGenerate);
            } else {
                // This is a new chunk we need to create
                GenerateChunk(indexToGenerate);
            }
        }

        // Any index still in chunksToRemove is outside our render distance
        foreach (int index in chunksToRemove) {
            if (activeChunks.ContainsKey(index)) {
                Destroy(activeChunks[index]);
                activeChunks.Remove(index);
            }
        }
    }

    /// <summary>
    /// Creates a single new chunk at the given index.
    /// </summary>
    void GenerateChunk(int index) {
        // Get the deterministic height and start position for this chunk
        float height = GetChunkHeight(index);
        Vector2 startPos = GetChunkStartPosition(index);

        // Create the chunk
        GameObject newChunk = Instantiate(
            mountainChunkPrefab,
            (Vector3)startPos, // Cast to Vector3
            Quaternion.identity
        );

        // Scale it to match the calculated slope
        newChunk.transform.localScale = new Vector3(chunkBaseWidth, height, 1);
        newChunk.transform.SetParent(this.transform); // Keep hierarchy clean

        // Add to our dictionary
        activeChunks.Add(index, newChunk);
    }

    /// <summary>
    /// Gets the *height* of a chunk using Perlin noise (deterministic).
    /// </summary>
    float GetChunkHeight(int index) {
        // Check if we've already calculated this
        if (chunkHeightCache.ContainsKey(index)) {
            return chunkHeightCache[index];
        }

        // Use PerlinNoise to get a repeatable "random" value between 0.0 and 1.0
        float noise = Mathf.PerlinNoise((index * noiseScale) + noiseSeed, 0f);

        // Map this noise value (0-1) to our desired angle range (0-65)
        float slopeAngle = 65f * noise;

        // Use trigonometry to find the height
        float height = chunkBaseWidth * Mathf.Tan(slopeAngle * Mathf.Deg2Rad);

        // Ensure height is a small positive number to avoid 0-scale objects
        if (height <= 0.01f) height = 0.01f;

        // Store this in our cache and return it
        chunkHeightCache.Add(index, height);
        return height;
    }

    /// <summary>
    /// Gets the *starting (x,y) position* of a chunk.
    /// This is recursive and ensures the terrain is continuous.
    /// </summary>
    Vector2 GetChunkStartPosition(int index) {
        // Check the cache first
        if (chunkStartCache.ContainsKey(index)) {
            return chunkStartCache[index];
        }

        Vector2 newStartPos;

        if (index > 0) {
            // Get the start pos of the chunk *before* this one
            Vector2 previousStartPos = GetChunkStartPosition(index - 1);
            // Get the height of the chunk *before* this one
            float previousHeight = GetChunkHeight(index - 1);

            // This chunk starts where the last one ended
            newStartPos = new Vector2(
                previousStartPos.x + chunkBaseWidth,
                previousStartPos.y + previousHeight
            );
        } else // index must be < 0
          {
            // Get the start pos of the chunk *after* this one (e.g., get 0 for -1)
            Vector2 nextStartPos = GetChunkStartPosition(index + 1);
            // Get the height of *this* chunk
            float thisHeight = GetChunkHeight(index);

            // This chunk's "end" point is the next chunk's "start" point
            newStartPos = new Vector2(
                nextStartPos.x - chunkBaseWidth,
                nextStartPos.y - thisHeight
            );
        }

        // Save to cache and return
        chunkStartCache.Add(index, newStartPos);
        return newStartPos;
    }

    /// <summary>
    /// Helper to find which chunk index the player is standing on.
    /// </summary>
    int GetChunkIndexFromPosition(float xPos) {
        return Mathf.FloorToInt(xPos / chunkBaseWidth);
    }
}