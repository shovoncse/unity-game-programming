using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    public float thrustSpeed = 3000f;
    public float turnSpeed = 200f;
    public float hoverPower = 500f;
    public float hoverHeight = 2.5f;
    public float hoverDamping = 5f; // Damping to smooth hover force
    public float hoverSpringStrength = 50f; // Spring force to stabilize hover
    public float rotationSmoothing = 2f; // Adjust slope alignment smoothness

    // Turbo Boost Variables
    public float boostMultiplier = 2.5f; // Speed multiplier during boost
    public float boostDuration = 2f; // How long the boost lasts
    public float boostCooldown = 5f; // Cooldown before boost can be used again

    private float thrustInput;
    private float turnInput;
    private Rigidbody shipRigidBody;

    private bool isBoosting = false;
    private float boostEndTime = 0f;
    private float nextBoostTime = 0f;

    void Start()
    {
        shipRigidBody = GetComponent<Rigidbody>();
        shipRigidBody.interpolation = RigidbodyInterpolation.Interpolate; // Smooth physics updates
    }

    void Update()
    {
        thrustInput = Input.GetAxis("Vertical");
        turnInput = Input.GetAxis("Horizontal");

        // Activate Turbo Boost when Left Shift is pressed and cooldown is over
        if (Input.GetKeyDown(KeyCode.LeftShift) && Time.time >= nextBoostTime)
        {
            isBoosting = true;
            boostEndTime = Time.time + boostDuration;
            nextBoostTime = Time.time + boostCooldown;
        }

        // Reset boost after duration ends
        if (isBoosting && Time.time >= boostEndTime)
        {
            isBoosting = false;
        }
    }

    void FixedUpdate()
    {
        // Apply turbo boost effect
        float currentThrustSpeed = isBoosting ? thrustSpeed * boostMultiplier : thrustSpeed;

        // **Smooth Turning with Rotation Torque**
        shipRigidBody.AddRelativeTorque(0f, turnInput * turnSpeed * Time.fixedDeltaTime, 0f, ForceMode.Acceleration);

        // **Apply Thrust with Boost**
        shipRigidBody.AddRelativeForce(0f, 0f, thrustInput * currentThrustSpeed * Time.fixedDeltaTime, ForceMode.Acceleration);

        // **Hovering with Spring-Based Stabilization**
        Ray ray = new Ray(transform.position, -transform.up);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, hoverHeight))
        {
            float groundDistance = hit.distance;
            float heightError = hoverHeight - groundDistance;

            // **Spring-based hover force (natural stabilization)**
            float hoverForce = heightError * hoverSpringStrength - shipRigidBody.velocity.y * hoverDamping;
            shipRigidBody.AddForce(Vector3.up * hoverForce, ForceMode.Acceleration);

            // **Align to Terrain Gradually**
            Quaternion targetRotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
            shipRigidBody.MoveRotation(Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * rotationSmoothing));
        }
    }
}
