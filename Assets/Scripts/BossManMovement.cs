using UnityEngine;
using System.Collections;

public class BossManMovement : MonoBehaviour {
    // 
    public Rigidbody2D mainBody;
    public Animator animator;

    // How fast to move
    public float moveSpeed = 10f;
    public float pushSpeed = 7f;
    public int moveDir = 0; // 1 = right, 0 = idle, -1 = slide
    private bool cooldown = false;

    private float horizontalInput;
    private bool isPushing;
    private bool isHolding;

    void Update() {
        // 1. Get input from A/D keys
        horizontalInput = Input.GetAxis("Horizontal"); // -1 for A, 1 for D

        isPushing = Input.GetKey(KeyCode.Space);
        isHolding = Input.GetKey(KeyCode.LeftShift);

        // Use Mathf.Abs to always send a positive speed (0 if idle, >0 if moving left or right)

        float animationSpeed = Mathf.Abs(horizontalInput);

        if (Input.GetKeyDown(KeyCode.LeftShift) && !cooldown) {
            Debug.Log("Shift key pressed!");
            moveDir = 0;
            StartCoroutine(CooldownRoutine(1f));
        }

        if (animator == null) {
            // If animator is missing, just skip the animation logic
            return;
        }

        // 2. Tell the Animator the speed
        animator.SetFloat("Speed", animationSpeed);
        animator.SetBool("isPushing", isPushing && animationSpeed > 0.01f);
        animator.SetBool("isHolding", isHolding && animationSpeed > 0.01f);
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

    private IEnumerator CooldownRoutine(float time) {
        cooldown = true;
        yield return new WaitForSeconds(time);
        cooldown = false;
        moveDir = 1;
    }

    public IEnumerator SlideBackRoutine() {
        moveDir = -1;
        cooldown = true;
        yield return new WaitForSeconds(5f);
        cooldown = false;
        moveDir = 1;
    }

}