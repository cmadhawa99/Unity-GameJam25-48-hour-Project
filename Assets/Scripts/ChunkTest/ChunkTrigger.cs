using UnityEngine;

public class ChunkTrigger : MonoBehaviour {
    public enum TriggerType { Spawn, Recycle }
    public TriggerType triggerType;

    private Chunk parentChunk;

    void Start() {
        parentChunk = GetComponentInParent<Chunk>();
    }

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player") && triggerType == TriggerType.Spawn) {
            parentChunk.manager.SpawnChunk();
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player") && triggerType == TriggerType.Recycle) {
            parentChunk.manager.RecycleChunk(parentChunk.gameObject);
        }
    }
}
