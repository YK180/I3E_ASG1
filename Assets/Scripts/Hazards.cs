/*
* Author: Tan Ye Kai
* Date: 15/6/2025
* Description: Hazards Script
*/

using System.Security.Cryptography;
using UnityEngine;

/// <summary>
/// Damages the player upon contact and plays a hazard sound.
/// This component is attached to hazard objects that harm the player when touched.
/// </summary>

public class Hazards : MonoBehaviour
{
    /// <summary>
    /// Amount of damage dealt to the player on contact.
    /// </summary>
    public int damageAmount = 10;
    
    /// <summary>
    /// Audio clip that plays when the player touches the hazard.
    /// </summary>
    public AudioClip hazardBGM;

    /// <summary>
    /// AudioSource component used to play the hazard sound.
    /// </summary>
    private AudioSource audioSource;

    /// <summary>
    /// Called when another collider enters this object's trigger collider.
    /// Applies damage to the player and plays a hazard sound.
    /// </summary>
    /// <param name="other">The collider that entered the hazard area.</param>
    private void OnTriggerEnter(Collider other)
    {

        PlayerBehaviour player = other.GetComponent<PlayerBehaviour>();
        if (player != null)
        {
            // Apply damage to the player
            player.TakeDamage(damageAmount);

            // Play the hazard sound
            if (hazardBGM != null && audioSource != null)
            {
                audioSource.clip = hazardBGM;
                audioSource.Play();
            }

        }

    }

    /// <summary>
    /// Unity's Start method. Initializes the audio source component.
    /// </summary>
    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;
        audioSource.loop = false;
    
    }

    /// <summary>
    /// Unity's Update method (currently unused).
    /// </summary>
    void Update()
    {

    }
}
