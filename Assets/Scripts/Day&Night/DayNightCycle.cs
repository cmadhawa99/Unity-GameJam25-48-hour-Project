using UnityEngine;
using UnityEngine.Rendering.Universal; // Make sure to add this line!

public class DayNightCycle : MonoBehaviour {
    [Header("Light Settings")]
    [SerializeField]
    private Light2D globalLight; // Assign your Global Light 2D here

    [SerializeField]
    private Gradient lightColor; // Defines the color of the light over the cycle

    [Header("Cycle Settings")]
    [SerializeField]
    private float cycleDurationSeconds = 60f; // Full cycle length in seconds

    private float cycleProgress; // Current point in the cycle (0.0 to 1.0)

    void Update() {
        if (globalLight == null) return;

        // 1. Increment the cycle progress
        // Time.deltaTime is the time since the last frame
        // Dividing by cycleDurationSeconds makes it progress from 0 to 1 over that duration
        cycleProgress += Time.deltaTime / cycleDurationSeconds;

        // 2. Loop the progress
        // If progress is 1 (or more), reset it to 0
        if (cycleProgress >= 1.0f) {
            cycleProgress -= 1.0f;
        }

        // 3. Apply the new light color
        // Evaluate the gradient at the current progress point
        globalLight.color = lightColor.Evaluate(cycleProgress);
    }
}