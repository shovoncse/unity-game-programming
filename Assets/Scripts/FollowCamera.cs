using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public GameObject playerShip;  // Reference to the player's ship

    private float height = 4.0f;       // Vertical offset
    private float distance = 5.0f;     // Distance behind the ship
    private float followDamping = 5f;  // How smoothly the camera follows
    private float rotationDamping = 8f; // How smoothly the camera rotates

    void LateUpdate() // Using LateUpdate() for smoother camera movement
    {
        if (playerShip == null) return; // Prevent errors if the ship is missing

        // Calculate the desired position
        Vector3 desiredPosition = playerShip.transform.TransformPoint(0f, height, -distance);

        // Smoothly move the camera to the desired position
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followDamping * Time.deltaTime);

        // Calculate the desired rotation
        Quaternion targetRotation = Quaternion.LookRotation(playerShip.transform.position - transform.position + Vector3.up * 1.5f);

        // Smoothly rotate the camera to match the ship's movement
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationDamping * Time.deltaTime);
    }
}
