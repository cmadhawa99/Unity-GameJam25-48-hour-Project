using UnityEngine;

public class CameraFollow : MonoBehaviour {

    public GameObject target;
    public float smoothFactor = 0.25f; // Adjust this in the Inspector
    public Vector3 offset;

    // FixedUpdate is the correct place for follow-camera logic
    void FixedUpdate() {
        if (target) {

            // 1. Define the final, desired position for the camera
            // This is the target's position plus our offset
            Vector3 desiredPosition = target.transform.position + offset;

            // 2. IMPORTANT: Keep the camera's original Z-position.
            // This is the key for 2D. We only want to follow X and Y.
            desiredPosition.z = transform.position.z;

            // 3. Smoothly move from the current position to the desired position
            // Vector3.Lerp moves *part* of the way (the smoothFactor)
            // from the start (transform.position) to the end (desiredPosition).
            Vector3 smoothedPosition = Vector3.Lerp(
                transform.position,
                desiredPosition,
                smoothFactor
            );

            transform.position = smoothedPosition;
        }
    }
}