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
        chunkObj.transform.position = new Vector3(index * chunkWidth, 0f, 0f);

        MeshFilter mf = chunkObj.AddComponent<MeshFilter>();
        MeshRenderer mr = chunkObj.AddComponent<MeshRenderer>();
        PolygonCollider2D poly = chunkObj.AddComponent<PolygonCollider2D>();

        mr.sharedMaterial = GetComponent<MeshRenderer>().sharedMaterial;

        float step = chunkWidth / (pointsPerChunk - 1);

        // --- 1. GENERATE COLLIDER POINTS (Must be precise) ---
        // These points will range from localX=0 to localX=chunkWidth

        List<Vector2> colliderPoints = new List<Vector2>();
        // We store these in a Vector3[] to reuse them for the mesh
        Vector3[] topPoints = new Vector3[pointsPerChunk];

        for (int i = 0; i < pointsPerChunk; i++) {
            // Use a normalized 't' value to guarantee t=0.0 at i=0 and t=1.0 at i=pointsPerChunk-1
            // This prevents floating-point drift from (i * step)
            float t = (float)i / (pointsPerChunk - 1);
            float localX = t * chunkWidth;
            float worldX = (index * chunkWidth) + localX;

            float y = Mathf.PerlinNoise(seed, worldX * noiseScale) * heightScale;
            y += Mathf.PerlinNoise(seed * 0.5f, worldX * noiseScale * 0.5f) * heightScale * 0.5f;
            y -= Mathf.PerlinNoise(seed * 2f, worldX * noiseScale * 2f) * heightScale * 0.3f;

            topPoints[i] = new Vector3(localX, y, 0);
            colliderPoints.Add(new Vector2(localX, y));
        }

        // Add bottom points for the collider
        colliderPoints.Add(new Vector2(chunkWidth, groundDepth));
        colliderPoints.Add(new Vector2(0, groundDepth));
        poly.points = colliderPoints.ToArray();


        // --- 2. GENERATE MESH VERTICES (With overlap) ---
        // We copy the collider points and add ONE EXTRA vertex
        // to "stitch" this mesh to the next one.

        List<Vector3> verts = new List<Vector3>(topPoints);

        // Calculate the "stitch" vertex (which is the first vertex of the *next* chunk)
        float next_localX = chunkWidth + step;
        float next_worldX = (index * chunkWidth) + next_localX;
        float next_y = Mathf.PerlinNoise(seed, next_worldX * noiseScale) * heightScale;
        next_y += Mathf.PerlinNoise(seed * 0.5f, next_worldX * noiseScale * 0.5f) * heightScale * 0.5f;
        next_y -= Mathf.PerlinNoise(seed * 2f, next_worldX * noiseScale * 2f) * heightScale * 0.3f;

        verts.Add(new Vector3(next_localX, next_y, 0)); // Add the stitch vertex

        // Our top line now has (pointsPerChunk + 1) vertices [indices 0 to pointsPerChunk]

        // Add bottom vertices, aligned with the new, wider mesh
        int bottomRightIdx = pointsPerChunk + 1; // Index after the stitch vertex
        int bottomLeftIdx = pointsPerChunk + 2;  // Index after that

        verts.Add(new Vector3(next_localX, groundDepth, 0)); // New bottom-right
        verts.Add(new Vector3(0, groundDepth, 0));           // Original bottom-left


        // --- 3. TRIANGLES (Adjusted for the extra vertex) ---
        // We now build 'pointsPerChunk' quads, not 'pointsPerChunk - 1'

        List<int> tris = new List<int>();
        for (int i = 0; i < pointsPerChunk; i++) { // Loop one more time
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