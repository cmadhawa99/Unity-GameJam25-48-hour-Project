using UnityEngine;
using System.Collections.Generic;

//I had help from AI when updating this script, Perlin noise

[RequireComponent(typeof(MeshRenderer))]
public class MG : MonoBehaviour {
    public Transform player;            // Player reference
    public float chunkWidth = 40f;      // Width of each terrain chunk
    public int pointsPerChunk = 80;     // Smoothness (higher = smoother)
    public float heightScale = 15f;     // How tall the mountains are
    public float noiseScale = 0.05f;    // Controls mountain frequency
    public int renderDistance = 4;      // How many chunks to load ahead/behind

    [Header("Slope Settings")]
    public float globalSlope = 0.1f;
    public float relativeGroundDepth = -25f;

    [Header("Flat Start Settings")]
    public float flatZoneLength = 100f;
    public float transitionLength = 50f;
    public float flatZoneNoiseScale = 0.05f; // Noise scale for the area before flatZoneLength
    public float flatZoneHeightScale = 2f;   // Noise height for the area before flatZoneLength

    [Header("Boundaries")]
    public float minXBoundary = -20f;

    [Header("Scene Prefabs")]
    // Prefab to place at X=0
    public GameObject startViewPrefab;
    public float startViewYPosition = 0f;
    // Prefab to place at boundary
    public GameObject boundaryViewPrefab;

    public float boundaryViewXPosition = -20f; // X coordinate for the boundary prefab
    public float boundaryViewYPosition = 0f;   // Y coordinate for the boundary prefab

    private float seed;
    private Dictionary<int, GameObject> chunks = new Dictionary<int, GameObject>();
    private int currentChunkIndex;

    // References to the instantiated scene objects
    private GameObject startViewInstance;
    private GameObject boundaryViewInstance;

    void Start() {
        seed = Random.Range(0f, 9999f);
        currentChunkIndex = GetPlayerChunk();

        // Instantiate the prefabs at their designated locations
        InstantiateSceneViews();

        UpdateChunks();
    }

    void Update() {
        int playerChunk = GetPlayerChunk();

        if (playerChunk != currentChunkIndex) {
            currentChunkIndex = playerChunk;
            UpdateChunks();
        }

        EnforceBoundaries();

        // Check player position to destroy or re-create scene views
        UpdateViewVisibility();
    }

    /// <summary>
    /// Instantiates the start and boundary prefabs if they are assigned and don't already exist.
    /// </summary>
    void InstantiateSceneViews() {
        // Instantiate the start view at (0, y, 0)
        if (startViewPrefab != null && startViewInstance == null) {
            startViewInstance = Instantiate(startViewPrefab, new Vector3(0f, startViewYPosition, 0f), Quaternion.identity, transform);
        }

        // Instantiate the boundary view at its custom X and Y position

        if (boundaryViewPrefab != null && boundaryViewInstance == null) {
            // Used the public fields for exact X and Y positioning
            Vector3 boundaryPos = new Vector3(boundaryViewXPosition, boundaryViewYPosition, 0f);
            boundaryViewInstance = Instantiate(boundaryViewPrefab, boundaryPos, Quaternion.identity, transform);
        }
    }

    /// <summary>
    /// Destroys the start and boundary prefabs and nulls their references.
    /// </summary>
    void DestroySceneViews() {
        if (startViewInstance != null) {
            Destroy(startViewInstance);
            startViewInstance = null;
        }

        if (boundaryViewInstance != null) {
            Destroy(boundaryViewInstance);
            boundaryViewInstance = null;
        }
    }

    /// <summary>
    /// Destroys or Instantiates the scene views based on player's X position.
    /// </summary>
    void UpdateViewVisibility() {
        // Views to exist *only* if the player is before the 100m mark.
        bool shouldExist = (player.position.x < 100f);

        if (shouldExist) {
            // Player is in the start zone, re-create views if they were destroyed
            InstantiateSceneViews();
        } else {
            // Player is past the start zone, destroy the views
            DestroySceneViews();
        }
    }

    void EnforceBoundaries() {
        // ... (this function is unchanged and still uses minXBoundary)
        Vector3 currentPos = player.position;
        if (currentPos.x < minXBoundary) {
            player.position = new Vector3(minXBoundary, currentPos.y, currentPos.z);
        }
    }

    int GetPlayerChunk() {
        // ... (this function is unchanged)
        return Mathf.FloorToInt(player.position.x / chunkWidth);
    }

    void UpdateChunks() {
        // ... (this function is unchanged)
        int playerChunk = currentChunkIndex;
        for (int offset = -renderDistance; offset <= renderDistance; offset++) {
            int chunkIndex = playerChunk + offset;
            if (!chunks.ContainsKey(chunkIndex))
                CreateChunk(chunkIndex);
        }
        List<int> toRemove = new List<int>();
        foreach (var c in chunks) {
            if (Mathf.Abs(c.Key - playerChunk) > renderDistance)
                toRemove.Add(c.Key);
        }
        foreach (int key in toRemove) {
            Destroy(chunks[key]);
            chunks.Remove(key);
        }
    }

    float GetTransitionFactor(float worldX) {
        // ... (this function is unchanged)
        float transitionStart = flatZoneLength;
        float transitionEnd = flatZoneLength + transitionLength;
        if (worldX <= transitionStart || transitionLength <= 0) {
            return 0f;
        }
        if (worldX >= transitionEnd) {
            return 1f;
        }
        float t_linear = (worldX - transitionStart) / transitionLength;
        return Mathf.SmoothStep(0f, 1f, t_linear);
    }

    float GetBaselineHeight(float worldX) {
        // ... (this function is unchanged)
        float transitionStart = flatZoneLength;
        float transitionEnd = flatZoneLength + transitionLength;
        if (worldX <= transitionStart || transitionLength <= 0) {
            return 0f;
        }
        if (worldX >= transitionEnd) {
            float h_transition_end = (0.5f * transitionLength) * globalSlope;
            return h_transition_end + (worldX - transitionEnd) * globalSlope;
        }
        float t_x = (worldX - transitionStart) / transitionLength;
        float h_transition = (transitionLength * globalSlope) * (Mathf.Pow(t_x, 3) - 0.5f * Mathf.Pow(t_x, 4));
        return h_transition;
    }

    float GetHeight(float worldX) {
        // ... (this function is unchanged)
        float baselineHeight = GetBaselineHeight(worldX);

        // Calculate noise for the "flat" zone using the new fields
        float flatNoiseVariation = Mathf.PerlinNoise(seed, worldX * flatZoneNoiseScale) * flatZoneHeightScale;
        flatNoiseVariation += Mathf.PerlinNoise(seed * 0.5f, worldX * flatZoneNoiseScale * 0.5f) * flatZoneHeightScale * 0.5f;
        flatNoiseVariation -= Mathf.PerlinNoise(seed * 2f, worldX * flatZoneNoiseScale * 2f) * flatZoneHeightScale * 0.3f;

        // Calculate noise for the "main" terrain using the main fields
        float mainNoiseVariation = Mathf.PerlinNoise(seed, worldX * noiseScale) * heightScale;
        mainNoiseVariation += Mathf.PerlinNoise(seed * 0.5f, worldX * noiseScale * 0.5f) * heightScale * 0.5f;
        mainNoiseVariation -= Mathf.PerlinNoise(seed * 2f, worldX * noiseScale * 2f) * heightScale * 0.3f;

        // Get the transition factor (0.0 for flat zone, 1.0 for main terrain)
        float noiseFactor = GetTransitionFactor(worldX);

        // Linearly interpolate between the two noise types based on the transition factor
        float finalNoiseVariation = Mathf.Lerp(flatNoiseVariation, mainNoiseVariation, noiseFactor);

        return baselineHeight + finalNoiseVariation;
    }

    void CreateChunk(int index) {
        // ... (this function is unchanged)
        GameObject chunkObj = new GameObject("Chunk " + index);
        chunkObj.transform.parent = transform;
        chunkObj.transform.position = new Vector3(index * chunkWidth, 0f, 0f);
        MeshFilter mf = chunkObj.AddComponent<MeshFilter>();
        MeshRenderer mr = chunkObj.AddComponent<MeshRenderer>();
        PolygonCollider2D poly = chunkObj.AddComponent<PolygonCollider2D>();
        mr.sharedMaterial = GetComponent<MeshRenderer>().sharedMaterial;
        float step = chunkWidth / (pointsPerChunk - 1);
        List<Vector2> colliderPoints = new List<Vector2>();
        Vector3[] topPoints = new Vector3[pointsPerChunk];
        for (int i = 0; i < pointsPerChunk; i++) {
            float t = (float)i / (pointsPerChunk - 1);
            float localX = t * chunkWidth;
            float worldX = (index * chunkWidth) + localX;
            float y = GetHeight(worldX);
            topPoints[i] = new Vector3(localX, y, 0);
            colliderPoints.Add(new Vector2(localX, y));
        }
        float startBaselineY = GetBaselineHeight(index * chunkWidth);
        float endBaselineY = GetBaselineHeight(index * chunkWidth + chunkWidth);
        colliderPoints.Add(new Vector2(chunkWidth, endBaselineY + relativeGroundDepth));
        colliderPoints.Add(new Vector2(0, startBaselineY + relativeGroundDepth));
        poly.points = colliderPoints.ToArray();
        List<Vector3> verts = new List<Vector3>(topPoints);
        float next_localX = chunkWidth + step;
        float next_worldX = (index * chunkWidth) + next_localX;
        float next_y = GetHeight(next_worldX);
        verts.Add(new Vector3(next_localX, next_y, 0));
        float next_baselineY = GetBaselineHeight(next_worldX);
        int bottomRightIdx = pointsPerChunk + 1;
        int bottomLeftIdx = pointsPerChunk + 2;
        verts.Add(new Vector3(next_localX, next_baselineY + relativeGroundDepth, 0));
        verts.Add(new Vector3(0, startBaselineY + relativeGroundDepth, 0));
        List<int> tris = new List<int>();
        for (int i = 0; i < pointsPerChunk; i++) {
            tris.Add(i);
            tris.Add(i + 1);
            tris.Add(bottomRightIdx);
            tris.Add(i + 1);
            tris.Add(bottomLeftIdx);
            tris.Add(bottomRightIdx);
        }
        Mesh mesh = new Mesh();
        mesh.vertices = verts.ToArray();
        mesh.triangles = tris.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mf.mesh = mesh;
        chunks[index] = chunkObj;
    }
}