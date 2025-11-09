using UnityEngine;

public class FollowPlayerX : MonoBehaviour {
    // Drag your Player GameObject here in the Inspector
    public Transform playerTransform;

    // We store this object's starting Y and Z position
    private float startY;
    private float startZ;

    void Start() {
        // Check if the player transform is assigned
        if (playerTransform == null) {
            Debug.LogError("Player Transform is not assigned in " + gameObject.name);
            return; // Stop if no player is set
        }

        // Store the initial Y and Z positions of this object
        startY = transform.position.y;
        startZ = transform.position.z;
    }

    void LateUpdate() {
        // If we have a player, update our position
        if (playerTransform != null) {
            // Create a new position vector.
            // We use the player's X, but our own original Y and Z.
            Vector3 newPosition = new Vector3(playerTransform.position.x, startY, startZ);

            // Apply the new position to this GameObject
            transform.position = newPosition;
        }
    }
}