/*
* Author: Tan Ye Kai
* Date: 15/6/2025
* Description: Door Script
*/

using UnityEngine;

/// <summary>
/// Controls the opening and closing of a door in the game.
/// The door smoothly rotates between open and closed states and may require a keycard to be accessed.
/// </summary>
public class Door : MonoBehaviour
{
    /// <summary>
    /// The angle in degrees that the door will rotate when opened.
    /// </summary>
    public float openAngle = 90f;    

    /// <summary>
    /// The speed at which the door opens and closes.
    /// </summary> 
    public float openSpeed = 2f;      

    /// <summary>
    /// Whether the door requires a keycard to open.
    /// </summary>
    public bool requiresKeycard = true;

    /// <summary>
    /// Tracks whether the door is currently open or closed.
    /// </summary>
    private bool isOpen = false;     

    /// <summary>
    /// The rotation of the door when it is closed.
    /// </summary>
    private Quaternion closedRotation;

    /// <summary>
    /// The rotation of the door when it is open.
    /// </summary>
    private Quaternion openRotation;

    /// <summary>
    /// Unity's Start method. Initializes the closed and open rotations.
    /// </summary>
    void Start()
    {

        // Store initial closed rotation
        closedRotation = transform.rotation;
        // Calculate open rotation by rotating around Y axis
        openRotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0, openAngle, 0));

    }

    /// <summary>
    /// Toggles the door's state between open and closed.
    /// </summary>
    public void ToggleDoor()
    {

        isOpen = !isOpen;

    }

    /// <summary>
    /// Unity's Update method. Smoothly rotates the door based on its current state.
    /// </summary>
    void Update()
    {
    
        if (isOpen)
            transform.rotation = Quaternion.Slerp(transform.rotation, openRotation, Time.deltaTime * openSpeed);
        else
            transform.rotation = Quaternion.Slerp(transform.rotation, closedRotation, Time.deltaTime * openSpeed);

    }


}
