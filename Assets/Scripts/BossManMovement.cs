using UnityEngine;

public class BossManMovement : MonoBehaviour {
    // Drag your ragdoll's central Rigidbody (like the Torso) here
    public Rigidbody2D mainBody;

    // How fast to move
    public float moveSpeed = 10f;

    private float horizontalInput;

    void Update() {
        // 1. Get input from A/D keys
        horizontalInput = Input.GetAxis("Horizontal"); // -1 for A, 1 for D
    }

    void FixedUpdate() {
        if (mainBody == null) {
            Debug.LogError("Main Rigidbody is not assigned!");
            return;
        }

        // 2. Apply the movement as a velocity
        // We set the velocity directly for responsive control
        // We keep the current vertical velocity (for gravity/jumping)
        mainBody.linearVelocity = new Vector2(horizontalInput * moveSpeed, mainBody.linearVelocity.y);
    }
}



