using System;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the obstacle hits the rock
        if (collision.gameObject.CompareTag("Rock"))
        {
            // Try to find the BossManMovement script (player controller)
            BossManMovement player = FindObjectOfType<BossManMovement>();
            Debug.LogError("Collided!");

            if (player != null)
            {
                // If space is NOT being pressed, trigger the slide back
                if (!Input.GetKey(KeyCode.Space))
                {
                    player.StartCoroutine(player.SlideBackRoutine());
                    Debug.LogError("Game Over!");
                }
            }

            // Destroy this obstacle
            Destroy(gameObject);
        }
    }
}
