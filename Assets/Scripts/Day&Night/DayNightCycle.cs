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
    public float CycleProgress { get; private set; } // Current point in the cycle (0.0 to 1.0)

    void Update() {
        if (globalLight == null) return;

        // 1. Increment the cycle progress
        CycleProgress += Time.deltaTime / cycleDurationSeconds;

        // 2. Loop the progress
        if (CycleProgress >= 1.0f) {
            CycleProgress -= 1.0f;
        }

        // 3. Apply the new light color
        globalLight.color = lightColor.Evaluate(CycleProgress);
    }
}