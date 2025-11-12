using UnityEngine;

public class BossManMovement : MonoBehaviour {
    // Drag your ragdoll's central Rigidbody (like the Torso) here
    public Rigidbody2D mainBody;

    // Drag the GameObject that has the Animator on it here
    // This might be the same GameObject as this script, or a child (like the "Sprite")
    public Animator animator;

    // How fast to move
    public float moveSpeed = 10f;
    public float pushSpeed = 7f;

    private float horizontalInput;
    private bool isPushing;

    void Update() {
        // 1. Get input from A/D keys
        horizontalInput = Input.GetAxis("Horizontal"); // -1 for A, 1 for D

        isPushing = Input.GetKey(KeyCode.Space);

        if (animator == null) {
            // If animator is missing, just skip the animation logic
            return;
        }

        // 2. Tell the Animator the speed
        // Use Mathf.Abs to always send a positive speed (0 if idle, >0 if moving left or right)
        float animationSpeed = Mathf.Abs(horizontalInput);
        animator.SetFloat("Speed", animationSpeed);
        animator.SetBool("isPushing", isPushing && animationSpeed > 0.01f);
    }

    void FixedUpdate() {
        if (mainBody == null) {
            Debug.LogError("Main Rigidbody is not assigned!");
            return;
        }

        float currentSpeed = isPushing ? pushSpeed : moveSpeed;

        // 4. Apply the movement as a velocity
        mainBody.linearVelocity = new Vector2(horizontalInput * currentSpeed, mainBody.linearVelocity.y);
    }

}