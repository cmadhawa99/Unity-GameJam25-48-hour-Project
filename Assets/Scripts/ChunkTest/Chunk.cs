// In Unity (C#)
using UnityEngine;

public class Chunk : MonoBehaviour {
    // Assign your two trigger objects in the Inspector

    // This will be set by the manager when it's spawned
    [HideInInspector]
    public ChunkManager manager;

}