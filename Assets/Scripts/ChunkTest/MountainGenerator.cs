using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshRenderer))]
public class MountainGenerator : MonoBehaviour {
    public Transform player;            // Player reference
    public float chunkWidth = 40f;      // Width of each terrain chunk
    public int pointsPerChunk = 80;     // Smoothness (higher = smoother)
    public float heightScale = 15f;     // How tall the mountains are
    public float noiseScale = 0.05f;    // Controls mountain frequency
    public int renderDistance = 4;      // How many chunks to load ahead/behind
    public float groundDepth = -25f;    // How far the mesh goes down

    private float seed;
    private Dictionary<int, GameObject> chunks = new Dictionary<int, GameObject>();

    // --- NEW VARIABLE ---
    // Store the chunk index the player was last in
    private int currentChunkIndex;

    void Start() {
        seed = Random.Range(0f, 9999f);

        // --- MODIFIED START ---
        // Get the player's starting chunk index
        currentChunkIndex = GetPlayerChunk();
        // Load the initial chunks
        UpdateChunks();
    }

    void Update() {
        // --- MODIFIED UPDATE ---
        // Get the player's current chunk
        int playerChunk = GetPlayerChunk();

        // Only run the logic if the player has moved to a new chunk
        if (playerChunk != currentChunkIndex) {
            currentChunkIndex = playerChunk;
            UpdateChunks();
        }
    }

    // --- NEW HELPER FUNCTION ---
    int GetPlayerChunk() {
        return Mathf.FloorToInt(player.position.x / chunkWidth);
    }

    void UpdateChunks() {
        // We now use the 'currentChunkIndex' member variable
        int playerChunk = currentChunkIndex;

        // Spawn new chunks around player
        for (int offset = -renderDistance; offset <= renderDistance; offset++) {
            int chunkIndex = playerChunk + offset;
            if (!chunks.ContainsKey(chunkIndex))
                CreateChunk(chunkIndex);
        }

        // Remove far-away chunks
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

    void CreateChunk(int index) {
        GameObject chunkObj = new GameObject("Chunk " + index);
        chunkObj.transform.parent = transform;

        MeshFilter mf = chunkObj.AddComponent<MeshFilter>();
        MeshRenderer mr = chunkObj.AddComponent<MeshRenderer>();
        PolygonCollider2D poly = chunkObj.AddComponent<PolygonCollider2D>();

        mr.sharedMaterial = GetComponent<MeshRenderer>().sharedMaterial;

        float startX = index * chunkWidth;
        float step = chunkWidth / (pointsPerChunk - 1);

        // Generate top line points using layered Perlin noise (more “mountainy”)
        Vector3[] topPoints = new Vector3[pointsPerChunk];
        for (int i = 0; i < pointsPerChunk; i++) {
            float x = startX + i * step;

            // Use multi-layer noise for realistic variation
            float y = Mathf.PerlinNoise(seed, x * noiseScale) * heightScale;
            y += Mathf.PerlinNoise(seed * 0.5f, x * noiseScale * 0.5f) * heightScale * 0.5f;
            y -= Mathf.PerlinNoise(seed * 2f, x * noiseScale * 2f) * heightScale * 0.3f;

            topPoints[i] = new Vector3(x, y, 0);
        }

        // Build mesh vertices
        List<Vector3> verts = new List<Vector3>(topPoints);
        verts.Add(new Vector3(startX + chunkWidth, groundDepth, 0));
        verts.Add(new Vector3(startX, groundDepth, 0));

        // Triangles
        List<int> tris = new List<int>();
        for (int i = 0; i < pointsPerChunk - 1; i++) {
            tris.Add(i);
            tris.Add(i + 1);
            tris.Add(pointsPerChunk);

            tris.Add(i + 1);
            tris.Add(pointsPerChunk + 1);
            tris.Add(pointsPerChunk);
        }

        Mesh mesh = new Mesh();
        mesh.vertices = verts.ToArray();
        mesh.triangles = tris.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mf.mesh = mesh;

        // Create polygon shape for collider (enclosed ground)
        List<Vector2> colliderPoints = new List<Vector2>();
        for (int i = 0; i < pointsPerChunk; i++)
            colliderPoints.Add(new Vector2(topPoints[i].x, topPoints[i].y));

        colliderPoints.Add(new Vector2(startX + chunkWidth, groundDepth));
        colliderPoints.Add(new Vector2(startX, groundDepth));

        poly.points = colliderPoints.ToArray();

        chunks[index] = chunkObj;
    }
}