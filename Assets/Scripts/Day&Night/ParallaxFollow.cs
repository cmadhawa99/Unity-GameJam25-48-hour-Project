using UnityEngine;

public class ParallaxFollow : MonoBehaviour {
    public Transform playerTransform;

    // 0 = Doesn't move at all
    // 0.5 = Moves at half the player's speed
    // 1 = Moves at the exact same speed (like Method 1)
    [Range(0f, 1f)]
    public float parallaxFactor;

    private float startY;
    private float startZ;
    private float startX;
    private float playerStartX;

    void Start() {
        if (playerTransform == null) {
            Debug.LogError("Player Transform is not assigned in " + gameObject.name);
            return;
        }

        // Store all our starting positions
        startX = transform.position.x;
        startY = transform.position.y;
        startZ = transform.position.z;

        // Store the player's starting X
        playerStartX = playerTransform.position.x;
    }

    void LateUpdate() {
        if (playerTransform != null) {
            // Calculate how far the player has moved from their start
            float playerDistance = playerTransform.position.x - playerStartX;

            // Calculate our new X position based on the player's movement and our factor
            float newX = startX + playerDistance * parallaxFactor;

            // Apply the new position, keeping our original Y and Z
            transform.position = new Vector3(newX, startY, startZ);
        }
    }
}