using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public GameObject rockPrefab;     // The rock prefab to spawn
    public float minSpawnTime = 1f;   // Minimum time between spawns
    public float maxSpawnTime = 3f;   // Maximum time between spawns
    public float spawnRangeX = 8f;    // How wide the random X offset is

    [Header("Rock Settings")]
    public float minScale = 1f;       // Minimum rock scale
    public float maxScale = 3f;       // Maximum rock scale

    private float timer;              // Countdown timer for next spawn
    private float nextSpawnTime;      // When the next rock should spawn

    void Start()
    {
        // Randomize the first spawn
        SetNextSpawnTime();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= nextSpawnTime)
        {
            SpawnRock();
            SetNextSpawnTime();
        }
    }

    void SetNextSpawnTime()
    {
        timer = 0f;
        nextSpawnTime = Random.Range(minSpawnTime, maxSpawnTime);
    }

    void SpawnRock()
    {
        if (rockPrefab == null) return;

        // Randomize spawn position horizontally
        Vector3 spawnPos = transform.position + new Vector3(Random.Range(-spawnRangeX, spawnRangeX), 0f, 0f);

        // Instantiate the rock
        GameObject newRock = Instantiate(rockPrefab, spawnPos, Quaternion.identity);

        // Randomize its size
        float randomScale = Random.Range(minScale, maxScale);
        newRock.transform.localScale = Vector3.one * randomScale;
    }
}
