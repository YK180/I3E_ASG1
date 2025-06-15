/*
* Author: Tan Ye Kai
* Date: 15/6/2025
* Description: PlayerBehaviour Script
*/

using UnityEngine;
using TMPro;
using System.Collections;
using System;

/// <summary>
/// Controls player behavior such as coin collection, health, interaction, death, and respawn.
/// </summary>
public class PlayerBehaviour : MonoBehaviour
{
    /// <summary>
    /// Current number of coins collected.
    /// </summary>
    public int coinCount = 0;
    
    /// <summary>
    /// UI text to display the current coin count.
    /// </summary>
    public TextMeshProUGUI CoinText;

    /// <summary>
    /// Total number of collectibles needed to win the game.
    /// </summary>
    public int totalCollectibles = 5;

   /// <summary>
   /// UI text to show when the player wins the game.
   /// </summary>
    public TextMeshProUGUI winMessageText;

    /// <summary>
    /// Maximum distance to interact with objects.
    /// </summary>
    public float interactDistance = 3f;      

    /// <summary>
    /// Key used for interacting with objects (default: E).
    /// </summary>
    public KeyCode interactKey = KeyCode.E;  

    /// <summary>
    /// True if the player has collected the keycard.
    /// </summary>
    public bool hasKeycard = false;

    /// <summary>
    /// Maximum health of the player.
    /// </summary>
    public int maxHealth = 100;

    /// <summary>
    /// UI text to display interaction messages.
    /// </summary>
    public TextMeshProUGUI interactionMessageText;

    /// <summary>
    /// UI text to display the player's health.
    /// </summary>
    public TextMeshProUGUI healthText;

    /// <summary>
    /// UI text to display the death message.
    /// </summary>
    public TextMeshProUGUI deathMessageText;

    /// <summary>
    /// UI panel shown when the player dies.
    /// </summary>
    public GameObject deathScreenPanel;

    /// <summary>
    /// Sound effect played when a coin is collected.
    /// </summary>
    public AudioClip coinSound;

    /// <summary>
    /// Background music played upon winning.
    /// </summary>
    public AudioClip victoryBGM;

    /// <summary>
    /// Sound effect played when the player dies.
    /// </summary>
    public AudioClip deathSound;

    /// <summary>
    /// Audio source component used for sound playback.
    /// </summary>
    public AudioSource audioSource;

    /// <summary>
    /// The player's current health during gameplay.
     /// </summary>
    private int currentHealth;

    /// <summary>
    /// The starting position of the player used for respawn after death.
    /// </summary>
    private Vector3 startPosition;

    /// <summary>
    /// The initial local position of the main camera, used to reset camera after respawn.
    /// </summary>
    private Vector3 initialCameraLocalPos;

    /// <summary>
    /// The initial local rotation of the main camera, used to reset camera orientation after respawn.
    /// </summary>
    private Quaternion initialCameraLocalRot;

    /// <summary>
    /// Cached reference to the main camera's transform for position and rotation reset.
    /// </summary>
    private Transform mainCameraTransform;

    /// <summary>
    /// Reference to the MouseLook script for controlling camera rotation, disabled on death and reset on respawn.
    /// </summary>
    private MouseLook mouseLookScript;

    /// <summary>
    /// Initializes player state, UI, and camera.
    /// </summary>
    void Start()
    {
        UpdateCoinUI();

        // Hide the win message at the start
        if (winMessageText != null)
            winMessageText.gameObject.SetActive(false);

        currentHealth = maxHealth;
        UpdateHealthUI();

        if (deathMessageText != null)
        {
            deathMessageText.text = "";
            deathMessageText.gameObject.SetActive(false);
        }


        startPosition = transform.position;

        if (Camera.main != null)
        {
            mainCameraTransform = Camera.main.transform;
            initialCameraLocalPos = mainCameraTransform.localPosition;
            initialCameraLocalRot = mainCameraTransform.localRotation;

            mouseLookScript = mainCameraTransform.GetComponent<MouseLook>();
        }

        audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Checks for interaction input and handles object interactions.
    /// </summary>
    void Update()
    {
        if (Camera.main == null) return;
        if (Input.GetKeyDown(interactKey))
        {
            RaycastHit hit;

            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, interactDistance))
            {
                Door door = hit.collider.GetComponent<Door>();
                if (door != null)
                {
                    if (door.requiresKeycard && !hasKeycard)
                    {
                        // Show UI message
                        StartCoroutine(ShowMessage("Door is locked. Requires keycard.", 2f));
                    }
                    else
                    {
                        door.ToggleDoor();
                    }
                    return;
                }

                // Check for Keycard interaction
                Keycard keycard = hit.collider.GetComponent<Keycard>();
                if (keycard != null)
                {
                    hasKeycard = true;
                    Destroy(keycard.gameObject);
                    StartCoroutine(ShowMessage("Keycard collected!", 2f));
                    return;
                }


            }
        }

    }

    /// <summary>
    /// Reduces player's health and checks for death.
    /// </summary>
    /// <param name="amount">Amount of damage to apply.</param>
    public void TakeDamage(float amount)
    {
        currentHealth -= Mathf.RoundToInt(amount);
        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            Die();
        }

    }

    /// <summary>
    /// Handles player death sequence including sound, UI, and disabling control.
    /// </summary>
    void Die()
    {

        Debug.Log("Player has died");

        if (audioSource != null && deathSound != null)
        {
            // Play death sound
            audioSource.PlayOneShot(deathSound);
        }


        // Show red death screen and "You died!" message
        if (deathScreenPanel != null)
            deathScreenPanel.SetActive(true);

        if (deathMessageText != null)
        {
            deathMessageText.gameObject.SetActive(true);
            deathMessageText.text = "You died!";
        }

        // Hide UI elements like health and coin count
        if (healthText != null)
            healthText.gameObject.SetActive(false);

        if (CoinText != null)
            CoinText.gameObject.SetActive(false);

        // Freeze player movement and camera look
        PlayerMovement movement = GetComponent<PlayerMovement>();
        if (movement != null)
            movement.enabled = false;

        if (mouseLookScript != null)
            mouseLookScript.enabled = false;

        StartCoroutine(RespawnPlayer());
        
    }

    /// <summary>
    /// Handles player respawn logic including resetting health, position, camera, and UI.
    /// </summary>
    IEnumerator RespawnPlayer()
    {

        yield return new WaitForSeconds(2f);

        // Reset position
        transform.position = startPosition + Vector3.up * 0.5f;

        // Reset velocity
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {

            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

        }

        // Reset health
        currentHealth = maxHealth;
        UpdateHealthUI();

        // Reset player scale and jump
        PlayerMovement movement = GetComponent<PlayerMovement>();

        if (movement != null)
        {
            // Slightly lower the player to ensure grounded raycast hits
            transform.position += Vector3.down * 0.05f;
            movement.ResetJumpState();
            
        }

        // Wait one frame to ensure visuals and movement are settled
        yield return new WaitForEndOfFrame();

        // Hide death message
        if (deathMessageText != null)
        {

            deathMessageText.text = "";
            deathMessageText.gameObject.SetActive(false);

        }

        // Reset camera position
        Transform cam = Camera.main.transform;
        cam.localPosition = initialCameraLocalPos;
        cam.localRotation = initialCameraLocalRot;

        MouseLook look = cam.GetComponent<MouseLook>();
        if (look != null)
            look.ResetLook();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (mainCameraTransform != null)
        {

            mainCameraTransform.localPosition = initialCameraLocalPos;
            mainCameraTransform.localRotation = initialCameraLocalRot;

            if (mouseLookScript != null)
                mouseLookScript.ResetLook();

        }

        // Re-enable movement and look scripts after respawn
        if (movement != null)
            movement.enabled = true;

        if (mouseLookScript != null)
            mouseLookScript.enabled = true;

        // Restore UI
        if (deathScreenPanel != null)
            deathScreenPanel.SetActive(false);

        if (deathMessageText != null)
        {
            deathMessageText.text = "";
            deathMessageText.gameObject.SetActive(false);
        }

        if (healthText != null)
            healthText.gameObject.SetActive(true);

        if (CoinText != null)
            CoinText.gameObject.SetActive(true);

    }

    /// <summary>
    /// Updates the health UI text to reflect current health.
    /// </summary>
    void UpdateHealthUI()
    {
        if (healthText != null)
        {
            healthText.text = "Health: " + currentHealth;
        }
    }

    /// <summary>Displays a message on screen for a limited duration.</summary>
    /// <param name="message">Message to show.</param>
    /// <param name="duration">Duration in seconds to show the message.</param>
    IEnumerator ShowMessage(string message, float duration)
    {

        interactionMessageText.text = message;
        interactionMessageText.gameObject.SetActive(true);

        yield return new WaitForSeconds(duration);

        interactionMessageText.gameObject.SetActive(false);

    }

    /// <summary>
    /// Adds coins to the player's total and checks for win condition.
    /// </summary>
    /// <param name="amount">Number of coins collected.</param>
    public void CollectCoin(int amount)
    {
        coinCount += amount;
        UpdateCoinUI();

        audioSource.PlayOneShot(coinSound);

        // Check if player won
        if (coinCount >= totalCollectibles)
        {
            WinGame();
        }
    }

    /// <summary>
    /// Handles win condition: displays message and plays victory music.
    /// </summary>
    void WinGame()
    {
        if (winMessageText != null)
        {
            winMessageText.gameObject.SetActive(true);
            winMessageText.text = "Congratulations! You collected all coins!";
        }

        
        Debug.Log("Player has won the game!");

        // Stop current sound
        audioSource.Stop();

        // Play BGM set 
        if (victoryBGM != null)
        {
            audioSource.clip = victoryBGM;
            audioSource.loop = false;
            audioSource.Play();
        }

    }

    /// <summary>
    /// Updates the coin count UI text.
    /// </summary>
    void UpdateCoinUI()
    {
        if (CoinText != null)
        {
            CoinText.text = "Coins: " + coinCount + " / " + totalCollectibles;
        }
    }


}
