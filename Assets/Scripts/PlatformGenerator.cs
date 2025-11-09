using UnityEngine;
using System.Collections.Generic;

public class SlopedPlatformGenerator : MonoBehaviour
{
    [Header("References")]
    public GameObject platformPrefab;
    public Transform player;

    [Header("Settings")]
    [Range(10f, 45f)] public float slopeAngle = 25f;
    public float platformLength = 10f;
    public int initialPlatforms = 3;
    public float spawnDistanceAhead = 25f; // how far ahead to spawn
    public float despawnDistanceBehind = 15f; // how far behind player to remove

    private List<GameObject> activePlatforms = new List<GameObject>();
    private Vector3 nextSpawnPos;

    void Start()
    {
        nextSpawnPos = transform.position;

        // spawn only in front of player initially
        for (int i = 0; i < initialPlatforms; i++)
        {
            SpawnPlatform();
        }
    }

    void Update()
    {
        // if the player is near the last platform, spawn a new one ahead
        if (Vector3.Distance(player.position, nextSpawnPos) < spawnDistanceAhead)
        {
            SpawnPlatform();
        }

        // remove any platforms that are behind the player
        for (int i = activePlatforms.Count - 1; i >= 0; i--)
        {
            GameObject platform = activePlatforms[i];
            if (player.position.x - platform.transform.position.x > despawnDistanceBehind)
            {
                Destroy(platform);
                activePlatforms.RemoveAt(i);
            }
        }
    }

    void SpawnPlatform()
    {
        if (!platformPrefab) return;

        Quaternion rotation = Quaternion.Euler(0f, 0f, slopeAngle);
        GameObject platform = Instantiate(platformPrefab, nextSpawnPos, rotation);
        activePlatforms.Add(platform);

        float rad = slopeAngle * Mathf.Deg2Rad;
        float deltaX = Mathf.Cos(rad) * platformLength;
        float deltaY = Mathf.Sin(rad) * platformLength;
        nextSpawnPos += new Vector3(deltaX, deltaY, 0f);
    }
}

