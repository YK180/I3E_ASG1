/*
* Author: Tan Ye Kai
* Date: 15/6/2025
* Description: Coin Script
*/

using UnityEngine;

/// <summary>
/// Handles the behaviour of the coin in the game.
/// The coins also rotates in game for visual feedbacks and are collected when the player touches them, adding value
/// to the player's total and destroying the coin object.
/// </summary>
public class Coin : MonoBehaviour
{   
    /// <summary>
    /// The number of coins this objects adds to the player's total.
    /// </summary>
    public int value = 1;

    /// <summary>
    /// The rotation speed of the coin in degrees per second.
    /// </summary>
    public float spinSpeed = 100f;

    /// <summary>
    /// Triggered when another collider enters the coin's trigger zone.
    /// If it's the player, the coin is collected.
    /// <summary>

    /// <param name="other">The collider that entered the trigger.</param>
    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            // Add to player's coin count
            PlayerBehaviour collector = other.GetComponent<PlayerBehaviour>();
            if (collector != null)
            {
                collector.CollectCoin(value);
            }

            // Destroy coin
            Destroy(gameObject);

        }
        
    }

    /// <summary>
    /// Unity Start method (currently unused).
    /// </summary>
    void Start()
    {

    }

    /// <summary>
    /// Rotates the coin continuously to make it visually noticeable.
    /// </summary>
    void Update()
    {

        // Spin around the y-axis
        transform.Rotate(0, spinSpeed * Time.deltaTime , 0);
        
    }

}



