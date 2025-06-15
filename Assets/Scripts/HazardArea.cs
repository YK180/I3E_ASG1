/*
* Author: Tan Ye Kai
* Date: 15/6/2025
* Description: HazardArea Script
*/

using UnityEngine;

/// <summary>
/// Causes instant death to the player upon entering the hazard area.
/// Plays a death audio clip if assigned.
/// </summary>
public class HazardArea : MonoBehaviour
{
    /// <summary>
    /// Damage dealt per second (unused in this version but kept for flexibility).
    /// </summary>
    public int damagePerSecond = 10;

    /// <summary>
    /// Audio clip to be played when the player dies.
    /// </summary>
    public AudioClip deathBGM;

    /// <summary>
    /// Reference to the AudioSource used to play sounds.
    /// </summary>
    private AudioSource audioSource;

    /// <summary>
    /// Called when another collider enters the trigger zone.
    /// Instantly kills the player and plays the death audio.
    /// </summary>
    
    /// <param name="other">The collider that entered the hazard area.</param>
    private void OnTriggerEnter(Collider other)
    {

        PlayerBehaviour player = other.GetComponent<PlayerBehaviour>();
        if (player != null)
        {
            // Instantly kills the player
            player.TakeDamage(player.maxHealth);

            // Play death sound once
            if (deathBGM != null && audioSource != null)
            {
                audioSource.clip = deathBGM;
                audioSource.Play();
            }

        }

    }

    /// <summary>
    /// Unity Start method.
    /// Initializes the audio source component.
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
    /// Unity Update method (currently unused).
    /// </summary>
    void Update()
    {
        
    }
}
