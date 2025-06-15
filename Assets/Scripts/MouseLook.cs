/*
* Author: Tan Ye Kai
* Date: 15/6/2025
* Description: MouseLook Script
*/

using UnityEngine;

/// <summary>
/// Controls the player's camera view based on mouse movement.
/// Allows smooth rotation of the camera and orientation with clamping to prevent unnatural rotation.
/// </summary>
public class MouseLook : MonoBehaviour
{
    /// <summary>
    /// Sensitivity for horizontal mouse movement.
    /// </summary>
    public float sensX;

    /// <summary>
    /// Sensitivity for vertical mouse movement.
    /// </summary>
    public float sensY;

    /// <summary>
    /// The player's orientation transform, used for directional movement.
    /// </summary>
    public Transform Orientation;

    /// <summary>
    /// Current vertical rotation value.
    /// </summary>
    float xRotation;

    /// <summary>
    /// Current horizontal rotation value.
    /// </summary>
    float yRotation;

    /// <summary>
    /// Unity Start method (currently unused).
    /// </summary>
    void Start()
    {

    }

    /// <summary>
    /// Updates the camera rotation based on mouse input every frame.
    /// </summary>
    void Update()
    {
        //Get mouse input
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        // Adjust rotation values
        yRotation += mouseX;
        xRotation -= mouseY;

        // Clamp vertical rotation to avoid flipping
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Apply rotation to camera and orientation
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        Orientation.rotation = Quaternion.Euler(0, yRotation, 0);

    }

    /// <summary>
    /// Resets the camera and orientation rotation to the default (forward-facing) state.
    /// </summary>
    public void ResetLook()
    {

        xRotation = 0f;
        yRotation = 0f;

        transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        Orientation.localRotation = Quaternion.Euler(0f, 0f, 0f);
        
    }
}