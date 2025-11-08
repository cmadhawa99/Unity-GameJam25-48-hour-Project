using UnityEngine;

public class StiffenRagdoll : MonoBehaviour {
    [Header("Muscle Settings")]
    [Tooltip("How strongly the joints try to return to their target pose.")]
    public float stiffness = 500f;

    [Tooltip("Prevents the joints from overshooting and shaking violently.")]
    public float dampening = 50f;

    [Tooltip("The maximum force the 'muscles' can use.")]
    public float maxMotorTorque = 10000f;

    [Header("Root Body")]
    [Tooltip("Drag your 'root' body part (the one with the BalanceScript) here.")]
    public Rigidbody2D rootBodyToIgnore;

    private HingeJoint2D[] allJoints;
    private float[] initialRotations;

    void Start() {
        // Find all joints in the character
        allJoints = GetComponentsInChildren<HingeJoint2D>();
        initialRotations = new float[allJoints.Length];

        for (int i = 0; i < allJoints.Length; i++) {
            // Find the Rigidbody2D that this joint is attached to
            Rigidbody2D jointRb = allJoints[i].GetComponent<Rigidbody2D>();

            // --- THIS IS THE IMPORTANT PART ---
            // If this joint is on the "root" body, we skip it.
            // The BalanceScript is in charge of this one.
            if (jointRb == rootBodyToIgnore) {
                allJoints[i].useMotor = false; // Ensure its motor is off
                continue; // Skip the rest of the setup for this joint
            }

            // --- Setup for all OTHER joints (knees, elbows, neck, etc.) ---

            // Store the joint's starting "standing" angle
            initialRotations[i] = allJoints[i].jointAngle;

            // Get the motor, set its max force, and re-assign it
            JointMotor2D motor = allJoints[i].motor;
            motor.maxMotorTorque = maxMotorTorque;
            allJoints[i].motor = motor;

            // IMPORTANT: Enable the motor
            allJoints[i].useMotor = true;
        }
    }

    void FixedUpdate() {
        // This loop runs every physics step
        for (int i = 0; i < allJoints.Length; i++) {
            // Find the Rigidbody2D that this joint is attached to
            Rigidbody2D jointRb = allJoints[i].GetComponent<Rigidbody2D>();

            // Again, skip the root body
            if (jointRb == rootBodyToIgnore) {
                continue;
            }

            // If this joint wasn't set up (e.g., it was the root), skip it
            if (!allJoints[i].useMotor) {
                continue;
            }

            // --- Apply "muscle" force to all other joints ---
            HingeJoint2D joint = allJoints[i];
            JointMotor2D motor = joint.motor;

            // Calculate the error (how far the joint is from its "standing" pose)
            float angleError = initialRotations[i] - joint.jointAngle;

            // Calculate the opposing force to stop existing motion (dampening)
            float velocityError = -joint.jointSpeed;

            // Set the motor speed to correct the pose
            motor.motorSpeed = (angleError * stiffness) + (velocityError * dampening);

            // Re-assign the motor to the joint
            joint.motor = motor;
        }
    }
}