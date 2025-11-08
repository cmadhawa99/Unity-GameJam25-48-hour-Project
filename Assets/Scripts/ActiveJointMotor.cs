using UnityEngine;

[RequireComponent(typeof(HingeJoint2D))]
public class ActiveJointMotor : MonoBehaviour {
    private HingeJoint2D hingeJoint;


    [Tooltip("The target angle (in degrees) the joint will try to hold.")]
    public float targetAngle = 0f;

    [Tooltip("How strong the motor is. Higher values are 'stiffer'.")]
    public float strength = 100f;

    [Tooltip("How much the motor resists overshooting. (Keeps it stable)")]
    public float damping = 5f;

    [Tooltip("The maximum force the motor can use. Leave high (e.g., 10000).")]
    public float maxTorque = 10000f;


    void Start() {
        hingeJoint = GetComponent<HingeJoint2D>();

        hingeJoint.useMotor = true;
    }

    // Physics code must run in FixedUpdate
    void FixedUpdate() {
        float angleError = Mathf.DeltaAngle(hingeJoint.jointAngle, targetAngle);

        float targetVelocity = angleError * strength;

        JointMotor2D motor = hingeJoint.motor;

        motor.motorSpeed = targetVelocity;

        motor.maxMotorTorque = maxTorque;

        hingeJoint.motor = motor;

        Rigidbody2D rb = hingeJoint.attachedRigidbody; 
        rb.AddTorque(-rb.angularVelocity * damping * Time.fixedDeltaTime);
    }
}