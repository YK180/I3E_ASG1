/*
* Author: Tan Ye Kai
* Date: 15/6/2025
* Description: Keycard Script
*/

using UnityEngine;

/// <summary>
/// Represents a collectible keycard object that spins visually and can be collected by the player.
/// </summary>
public class Keycard : MonoBehaviour
{

    /// <summary>
    /// Rotation speed of the keycard in degrees per second.
    /// </summary>
    public float spinSpeed = 100f; 

    /// <summary>
    /// Called to collect the keycard, sets the player's keycard status to true and destroys the keycard object.
    /// </summary>
    /// <param name="player">The player collecting the keycard.</param>
    public void CollectKeycard(PlayerBehaviour player)
    {
        player.hasKeycard = true;

        Debug.Log("Keycard collected!");
        Destroy(gameObject);  
    }

    /// <summary>
    /// Unity Start method (currently unused).
    /// </summary>
    void Start()
    {

    }

    /// <summary>
    /// Unity Update method that continuously spins the keycard for visual effect.
    /// </summary>
    void Update()
    {
        // Spin around the y-axis
        transform.Rotate(0, spinSpeed * Time.deltaTime, 0);
        
    }
}
