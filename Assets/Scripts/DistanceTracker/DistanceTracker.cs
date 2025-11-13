using UnityEngine;
using TMPro;

public class DistanceTracker : MonoBehaviour {
    public TextMeshProUGUI distanceText;
    public enum DisplayUnit { Meters, Yards }
    public DisplayUnit unit = DisplayUnit.Meters;

    private float totalDistance = 0f;
    private float startX;

    // Conversion constant: 1 meter = 1.09361 yards
    private const float METERS_TO_YARDS = 1.09361f;

    void Start() {
        startX = transform.position.x;
        UpdateDistanceText();
    }

    void Update() {
        // --- MODIFIED ---
        // Calculate the distance from the start
        float displacement = transform.position.x - startX;

        // Use Mathf.Max to ensure the distance is never less than 0
        totalDistance = Mathf.Max(0f, displacement);

        UpdateDistanceText();
    }

    private void UpdateDistanceText() {
        float displayDistance = 0f;
        string unitSuffix = "";

        if (unit == DisplayUnit.Meters) {
            displayDistance = totalDistance;
            unitSuffix = "m";
        } else if (unit == DisplayUnit.Yards) {
            displayDistance = totalDistance * METERS_TO_YARDS;
            unitSuffix = "yd";
        }

        distanceText.text = $"Distance: {displayDistance.ToString("F1")} {unitSuffix}";
    }
}