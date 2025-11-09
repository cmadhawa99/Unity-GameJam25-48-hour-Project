using UnityEngine;

public class SunAndMoon : MonoBehaviour {
    [Header("Reference")]
    [SerializeField]
    private DayNightCycle dayNightManager; // Drag your DayNightManager object

    void Update() {
        if (dayNightManager == null) return;

        // 1. Get the current cycle progress from the other script
        float progress = dayNightManager.CycleProgress;

        // 2. Rotate the pivot
        // We map the 0.0-1.0 progress to a 360-degree rotation.
        // (0.5 - progress) makes 0.5 (midday) the top of the circle.
        float rotationAngle = (0.5f - progress) * 360f;
        transform.rotation = Quaternion.Euler(0, 0, rotationAngle);
    }
}