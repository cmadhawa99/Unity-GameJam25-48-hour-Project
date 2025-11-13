using UnityEngine;

public class ObstacleSpawner : MonoBehaviour {

    [Header("Dependencies")]
    public Transform player; // Reference to the player to check distance

    [Header("Spawner Settings")]
    public GameObject rockPrefab;     // The rock prefab to spawn

    public float minSpawnDistance = 300f; // Player must pass this X-position to start spawns

    public float minSpawnTime = 1f;   // Minimum time between spawns
    public float maxSpawnTime = 3f;   // Maximum time between spawns
    public float spawnRangeX = 8f;    // How wide the random X offset is

    [Header("Rock Settings")]
    public float minScale = 1f;       // Minimum rock scale
    public float maxScale = 3f;       // Maximum rock scale

    private float timer;              // Countdown timer for next spawn
    private float nextSpawnTime;      // When the next rock should spawn

    void Start() {
        // Randomize the first spawn
        SetNextSpawnTime();
    }

    void Update() {
        // Check if a player is assigned
        if (player == null) {
            Debug.LogError("Player Transform not assigned to ObstacleSpawner!");
            return; // Stop if we don't know where the player is
        }

        // Check if the player has reached the minimum spawn distance
        if (player.position.x < minSpawnDistance) {
            // If not, reset the timer and do nothing.
            // This ensures the first spawn only happens *after* reaching the distance.
            SetNextSpawnTime();
            return;
        }

        // --- Original logic resumes if player.position.x >= minSpawnDistance ---
        timer += Time.deltaTime;

        if (timer >= nextSpawnTime) {
            SpawnRock();
            SetNextSpawnTime();
        }
    }

    void SetNextSpawnTime() {
        timer = 0f;
        nextSpawnTime = Random.Range(minSpawnTime, maxSpawnTime);
    }

    void SpawnRock() {
        if (rockPrefab == null) return;

        // Randomize spawn position horizontally
        Vector3 spawnPos = transform.position + new Vector3(Random.Range(-spawnRangeX, spawnRangeX), 0f, 1f);

        // Instantiate the rock
        GameObject newRock = Instantiate(rockPrefab, spawnPos, Quaternion.identity);

        // Randomize its size
        float randomScale = Random.Range(minScale, maxScale);
        newRock.transform.localScale = Vector3.one * randomScale;
    }
}