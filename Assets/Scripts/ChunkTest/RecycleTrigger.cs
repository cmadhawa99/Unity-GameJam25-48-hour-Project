using UnityEngine;

public class RecycleTrigger : MonoBehaviour
{
    // These will be set by the ChunkManager
    public ChunkManager manager;
    public GameObject chunkToRecycle; // The parent chunk

    // We use OnTriggerExit so it recycles *after* the player
    // has left the chunk completely.
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Tell the manager to recycle our parent chunk
            manager.RecycleChunk(chunkToRecycle);
        }
    }
}