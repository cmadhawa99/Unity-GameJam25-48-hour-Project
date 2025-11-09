using UnityEngine;
using System.Collections;

public class BossManMovement : MonoBehaviour
{
    public Rigidbody2D mainBody;
    public Animator animator;

    public float moveSpeed = 10f;
    public int moveDir = 0; // 1 = right, 0 = idle, -1 = slide
    private bool cooldown = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !cooldown)
        {
            Debug.Log("Space key pressed!");
            moveDir = 0;
            StartCoroutine(CooldownRoutine(1f));
        }

        if (animator != null)
            animator.SetInteger("direction", moveDir);
    }

    void FixedUpdate()
    {
        if (mainBody == null)
        {
            Debug.LogError("Main Rigidbody is not assigned!");
            return;
        }

        mainBody.linearVelocity = new Vector2(moveDir * moveSpeed, mainBody.linearVelocity.y);
    }

    IEnumerator CooldownRoutine(float time)
    {
        cooldown = true;
        yield return new WaitForSeconds(time);
        cooldown = false;
        moveDir = 1;
    }

    public IEnumerator SlideBackRoutine()
    {
        moveDir = -1;
        cooldown = true;
        yield return new WaitForSeconds(5f);
        cooldown = false;
        moveDir = 1;
    }
}
